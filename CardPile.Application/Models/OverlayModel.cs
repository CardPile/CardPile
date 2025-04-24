using CardPile.Application.Services;
using NLog;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace CardPile.Application.Models;

internal class OverlayModel : ReactiveObject, IOverlayService, IDisposable
{
    public OverlayModel()
    {
        overlayMonitorWorker = new BackgroundWorker();
        overlayMonitorWorker.DoWork += OverlayMonitor;
        overlayMonitorWorker.RunWorkerAsync();
    }

    ~OverlayModel()
    {
        Dispose(false);
    }

    private void OverlayMonitor(object? sender, DoWorkEventArgs e)
    {
        if (sender is not BackgroundWorker worker)
        {
            Logger.Error("OverlayMonitor did not receive a BackgroundWorker as a sender");
            return;
        }

        try
        {
            // TODO: Might not be needed if overlay detects we are down and shutdowns by itself
            while (!worker.CancellationPending)
            {
                CheckOverlayStatus();
                ProcessMessages();
                Thread.Sleep(MonitorSleepSpan);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Exception in overlay worker: {exception}", ex);
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (isDisposed)
        {
            return;
        }

        if (disposing)
        {
            pipeServer?.Dispose();
        }

        isDisposed = true;
    }

    private void CheckOverlayStatus()
    {
        if (overlayProcess != null && overlayProcess.HasExited)
        {
            overlayProcess = null;
            if(pipeServer != null)
            {
                pipeServer.Dispose();
                pipeServer = null;
            }
        }

        if (overlayProcess != null && pipeServer != null)
        {
            return;
        }

        Process? mtgaProcess = Process.GetProcessesByName(MTGA_PROCESS_NAME).FirstOrDefault();
        if (mtgaProcess == null)
        {
            return;
        }

        pipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

        overlayProcess = new Process();
        overlayProcess.StartInfo.FileName = OVERLAY_EXECUTABLE_NAME;
        overlayProcess.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
        overlayProcess.StartInfo.UseShellExecute = false;
        overlayProcess.Start();

        pipeServer.DisposeLocalCopyOfClientHandle();
    }

    private void ProcessMessages()
    {
        if(pipeServer == null)
        {
            return;
        }

        try
        {
            using (StreamWriter sw = new StreamWriter(pipeServer))
            {
                sw.AutoFlush = true;
                sw.WriteLine("Hello from pipes");
                pipeServer.WaitForPipeDrain();
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Exception processing messages: {exception}", ex);
        }
    }

    private readonly TimeSpan MonitorSleepSpan = TimeSpan.FromMilliseconds(100);

    // private readonly string MTGA_PROCESS_NAME = "MTGA";
    private readonly string MTGA_PROCESS_NAME = "notepad";

    private readonly string OVERLAY_EXECUTABLE_NAME = "CardPile.Overlay.Windows.exe";

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private Process? overlayProcess = null;

    // TODO: Move to names pipes for async operations
    private AnonymousPipeServerStream? pipeServer = null;

    private bool isDisposed = false;

    private BackgroundWorker overlayMonitorWorker;
}
