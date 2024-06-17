namespace UnitySpy.ProcessFacade
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A MacOS specific facade over a process that provides access to its memory space.
    /// </summary>
    public abstract class ProcessFacadeMacOS : ProcessFacade
    {
        private readonly Process process;

        public ProcessFacadeMacOS(Process process)
        {
            this.process = process;
        }

        public Process Process => this.process;
    }
}