using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Windows;

namespace CardPile.Overlay.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IDisposable
{
    private void ApplicationStartup(object sender, StartupEventArgs e)
    {
        if(e.Args.Length == 0)
        {
            App.Current.Shutdown();
        }

        PipeStream pipeClient = new AnonymousPipeClientStream(PipeDirection.In, e.Args[0]);

        pipeReader = new StreamReader(pipeClient);

        pipeReaderWorker = new BackgroundWorker();
        pipeReaderWorker.DoWork += ProcessMessages;
        pipeReaderWorker.RunWorkerAsync();
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
            pipeReaderWorker?.CancelAsync();
            pipeReaderWorker?.Dispose();
            pipeReader?.Dispose();
            pipeClient?.Dispose();
        }

        isDisposed = true;
    }

    private async void ProcessMessages(object? sender, DoWorkEventArgs e)
    {
        if (sender is not BackgroundWorker worker)
        {
            MessageBox.Show("ProcessMessages did not receive a BackgroundWorker as a sender");
            return;
        }

        try
        {
            // TODO: Might not be needed if overlay detects we are down and shutdowns by itself
            while (!worker.CancellationPending)
            {
                if(pipeReader == null)
                {
                    continue;
                }

                var text = await pipeReader.ReadToEndAsync();

                if(string.IsNullOrEmpty(text))
                {
                    continue;
                }

                MessageBox.Show(text);

                App.Current.Dispatcher.Invoke(() =>
                {
                    if (Window.GetWindow(App.Current.MainWindow) is OverlayWindow overlayWindow)
                    {
                        overlayWindow.TestText.Text = text;
                    }
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(string.Format("Exception in overlay worker: {0}", ex));
            throw;
        }
    }

    private PipeStream? pipeClient = null;

    private StreamReader? pipeReader = null;

    private BackgroundWorker? pipeReaderWorker = null;

    private bool isDisposed = false;
}
