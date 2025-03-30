namespace UnitySpy
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnitySpy.Detail;
    using UnitySpy.Offsets;
    using UnitySpy.ProcessFacade;
    using UnitySpy.Util;
    using ELFSharp.MachO;
    
    /// <summary>
    /// A factory that creates <see cref="IAssemblyImage"/> instances that provides access into a Unity application's
    /// managed memory.
    /// SEE: https://github.com/Unity-Technologies/mono.
    /// </summary>
    public static class AssemblyImageFactory
    {
        public static IAssemblyImage Create(UnityProcessFacade process, string assemblyName = "Assembly-CSharp")
        {
            if (process == null)
            {
                throw new ArgumentNullException(nameof(process), "The process parameter cannot be null");
            }

            var monoModule = process.GetMonoModule();
            IntPtr rootDomainFunctionAddress;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                rootDomainFunctionAddress = GetRootDomainFunctionAddressMachOFormat(monoModule);
            }
            else
            {
                var moduleDump = process.ReadModule(monoModule);
                rootDomainFunctionAddress = AssemblyImageFactory.GetRootDomainFunctionAddressPEFormat(moduleDump, monoModule, process.Is64Bits);
            }

            return AssemblyImageFactory.GetAssemblyImage(process, assemblyName, rootDomainFunctionAddress);
        }

        private static AssemblyImage GetAssemblyImage(UnityProcessFacade process, string name, IntPtr rootDomainFunctionAddress)
        {
            IntPtr domain;
            if (process.Is64Bits)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    if (RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64)
                    {
                        // Offsets taken by decompiling the 64 bits version of libmonobdwgc-2.0.dylib
                        //
                        // adrp	x8, 500 ; 0x30e000
                        // ldr	x0, [x8, #0xf40]
                        // ret
                        //
                        // The ARM64 spec allows us to recover the page offset from adrp (500 above)
                        // Also the offset used in ldr (#0xf40 above)
                        // 
                        // This allows us to calculate the location of root domain
                        IntPtr rootDomainFunctionAddressPage = rootDomainFunctionAddress & ~0xFFF;
                        
                        int adrpCommand = process.ReadInt32(rootDomainFunctionAddress);
                        IntPtr immhi = (adrpCommand & 0xFFFFE0) >> 5;
                        IntPtr immlo = (adrpCommand & 0x60000000) >> 29;
                        IntPtr pageOffset = (immhi << 2) | immlo;
                        pageOffset = (pageOffset & 0xFFFFF) | ((pageOffset & 0x100000) != 0 ? (IntPtr)0xFFF00000 : 0x0);
                        pageOffset = (pageOffset << 12);
                            
                        int ldrCommand = process.ReadInt32(rootDomainFunctionAddress + 4);
                        IntPtr imm = (ldrCommand & 0x3FFC00) >> 10;
                        IntPtr offset = (imm << 3);
                        
                        domain = process.ReadPtr(rootDomainFunctionAddressPage + pageOffset + offset);
                    }
                    else
                    {
                        // Offsets taken by decompiling the 64 bits version of libmonobdwgc-2.0.dylib
                        //
                        // push rbp
                        // mov rbp,rsp
                        // mov rax, [rip + 0x4250ba]
                        // pop rbp
                        // ret
                        //
                        // These five lines in Hex translate to
                        // 55
                        // 4889E5
                        // 488B05 BA5042 00
                        // 5D
                        // C3
                        //
                        // So wee need to offset the first seven bytes to get to the relative offset we need to add to rip
                        // rootDomainFunctionAddress + 7
                        //
                        // rip has the current value of the rootDoaminAddress plus the 4 bytes of the first two instructions
                        // plus the 7 bytes of the rip + offset instruction (mov rax, [rip + 0x4250ba]).
                        // then we need to add this offsets to get the domain starting address
                        int ripPlusOffsetOffset = 7;
                        int ripValueOffset = 11;  
                        int offset = process.ReadInt32(rootDomainFunctionAddress + ripPlusOffsetOffset) + ripValueOffset;
                        domain = process.ReadPtr(rootDomainFunctionAddress + offset);
                    }
                }
                else
                {
                    // Offsets taken by decompiling the 64 bits version of mono-2.0-bdwgc.dll
                    //
                    // mov rax, [rip + 0x46ad39]
                    // ret
                    //
                    // These two lines in Hex translate to
                    // 488B05 39AD46 00
                    // C3
                    //
                    // So wee need to offset the first three bytes to get to the relative offset we need to add to rip
                    // rootDomainFunctionAddress + 3
                    //
                    // rip has the current value of the rootDoaminAddress plus the 7 bytes of the first instruction (mov rax, [rip + 0x46ad39])
                    // then we need to add this offsets to get the domain starting address
                    int ripPlusOffsetOffset = 3;
                    int ripValueOffset = 7;
                    int offset = process.ReadInt32(rootDomainFunctionAddress + ripPlusOffsetOffset) + ripValueOffset;
                    domain = process.ReadPtr(rootDomainFunctionAddress + offset);                    
                }
            }
            else
            {
                var domainAddress = process.ReadPtr(rootDomainFunctionAddress + 1);
                //// pointer to struct of type _MonoDomain
                domain = process.ReadPtr(domainAddress);
            }
            
            //// pointer to array of structs of type _MonoAssembly
            var assemblyArrayAddress = process.ReadPtr(domain + process.MonoLibraryOffsets.ReferencedAssemblies);
            for (var assemblyAddress = assemblyArrayAddress;
                assemblyAddress != IntPtr.Zero;
                assemblyAddress = process.ReadPtr(assemblyAddress + process.SizeOfPtr))
            {
                var assembly = process.ReadPtr(assemblyAddress);
                var assemblyNameAddress = process.ReadPtr(assembly + (process.SizeOfPtr * 2));
                var assemblyName = process.ReadAsciiString(assemblyNameAddress);
                if (assemblyName == name)
                {
                    return new AssemblyImage(process, process.ReadPtr(assembly + process.MonoLibraryOffsets.AssemblyImage));
                }
            }

            throw new InvalidOperationException($"Unable to find assembly '{name}'");
        }

        private static IntPtr GetRootDomainFunctionAddressPEFormat(byte[] moduleDump, ModuleInfo monoModuleInfo, bool is64Bits)
        {
            // offsets taken from https://docs.microsoft.com/en-us/windows/desktop/Debug/pe-format
            // ReSharper disable once CommentTypo
            var startIndex = moduleDump.ToInt32(PEFormatOffsets.Signature); // lfanew

            var exportDirectoryIndex = startIndex + PEFormatOffsets.GetExportDirectoryIndex(is64Bits);
            var exportDirectory = moduleDump.ToInt32(exportDirectoryIndex);

            var numberOfFunctions = moduleDump.ToInt32(exportDirectory + PEFormatOffsets.NumberOfFunctions);
            var functionAddressArrayIndex = moduleDump.ToInt32(exportDirectory + PEFormatOffsets.FunctionAddressArrayIndex);
            var functionNameArrayIndex = moduleDump.ToInt32(exportDirectory + PEFormatOffsets.FunctionNameArrayIndex);
            var rootDomainFunctionAddress = IntPtr.Zero;
            for (var functionIndex = 0;
                functionIndex < numberOfFunctions * PEFormatOffsets.FunctionEntrySize;
                functionIndex += PEFormatOffsets.FunctionEntrySize)
            {
                var functionNameIndex = moduleDump.ToInt32(functionNameArrayIndex + functionIndex);
                var functionName = moduleDump.ToAsciiString(functionNameIndex);
                if (functionName == "mono_get_root_domain")
                {
                    rootDomainFunctionAddress = monoModuleInfo.BaseAddress
                        + moduleDump.ToInt32(functionAddressArrayIndex + functionIndex);

                    break;
                }
            }

            if (rootDomainFunctionAddress == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to find mono_get_root_domain function.");
            }

            return rootDomainFunctionAddress;
        }

        private static IntPtr GetRootDomainFunctionAddressMachOFormat(ModuleInfo monoModuleInfo)
        {
            var rootDomainFunctionAddress = IntPtr.Zero;

            if (MachOReader.TryLoad(monoModuleInfo.Path, out var macho) == MachOResult.FatMachO)
            {
                var results = MachOReader.LoadFat(new FileStream(monoModuleInfo.Path, FileMode.Open), true);
                var desiredMachineType = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? Machine.Arm64 : Machine.X86_64; 
                macho = results.First(x => x.Machine == desiredMachineType);
            }

            foreach (var symbolTable in macho.GetCommandsOfType<SymbolTable>())
            {
                foreach (var symbol in symbolTable.Symbols)
                {
                    if (symbol.Name == "_mono_get_root_domain")
                    {
                        rootDomainFunctionAddress = monoModuleInfo.BaseAddress + (IntPtr)symbol.Value;
                        break;
                    }
                }
            }
            
            if (rootDomainFunctionAddress == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to find mono_get_root_domain function.");
            }

            return rootDomainFunctionAddress;
        }
    }
}