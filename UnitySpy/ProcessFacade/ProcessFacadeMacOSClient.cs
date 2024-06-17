namespace UnitySpy.ProcessFacade
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A MacOS specific facade over a process that provides access to its memory space
    /// through a server running with root privileges.
    /// </summary>
    public class ProcessFacadeMacOSClient : ProcessFacadeMacOS, IDisposable
    {
        private readonly ProcessFacadeClient client;

        private bool isDisposed;

        public ProcessFacadeMacOSClient(Process process)
            : base(process)
        {
            this.client = new ProcessFacadeClient(process.Id);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.client.Dispose();
            }

            this.isDisposed = true;
        }

        public override void ReadProcessMemory(
            byte[] buffer,
            IntPtr processAddress,
            bool allowPartialRead = false,
            int? size = default)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("the buffer parameter cannot be null");
            }

            this.client.ReadProcessMemory(buffer, processAddress, size ?? buffer.Length);
        }

        public override ModuleInfo GetModule(string moduleName)
            => this.client.GetModuleInfo(moduleName);
    }
}