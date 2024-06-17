namespace UnitySpy.ProcessFacade
{
    using System;

    /// <summary>
    /// A Linux specific facade over a process that provides access to its memory space
    /// through a server running with /proc/$pid/mem read privileges.
    /// </summary>
    public class ProcessFacadeLinuxClient : ProcessFacadeLinux, IDisposable
    {
        private readonly ProcessFacadeClient client;

        private bool isDisposed;

        public ProcessFacadeLinuxClient(int processId)
            : base(processId)
        {
            this.client = new ProcessFacadeClient(processId);
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

        protected override void ReadProcessMemory(
            byte[] buffer,
            IntPtr processAddress,
            int length)
            => this.client.ReadProcessMemory(buffer, processAddress, length);
    }
}