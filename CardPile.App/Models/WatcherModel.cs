using Avalonia.Threading;
using CardPile.App.Services;
using CardPile.Draft;
using CardPile.Parser;
using ReactiveUI;
using System;
using System.IO;
using NLog;

namespace CardPile.App.Models;

internal class WatcherModel : ReactiveObject, IWatcherService
{
    internal WatcherModel()
    {
        // string logFilePath = @"D:\Programming\GitHub\CardPile\Test data\Logs\Player_fed.log";

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if(string.IsNullOrEmpty(localAppData) )
        {
            throw new InvalidOperationException("LocalAppData is null. Cannot resolve path to Player.log");
        }
        localAppData = localAppData.Replace("Roaming", "LocalLow");  // There is a beter way to do it, but this is simpler
        string logFilePath = Path.Combine(localAppData, "Wizards Of The Coast", "MTGA", "Player.log");

        logWatcher = new LogFileWatcher(logFilePath, false);

        memoryWatcher = new MemoryWatcher();

        dispatcher = new MatcherDispatcher();
        dispatcher.Connect(logWatcher);
        dispatcher.AddMatcher<DraftEnterMatcher>().DraftStartEvent += DraftStartHandler;

        logWatcherTimerHandle = null;
        memoryWatcherTimerHandle = null;

        StartLogWatcherTimer();
    }

    public event EventHandler<DraftEnterEvent>? DraftStartEvent;
    public event EventHandler<DraftChoiceEvent>? DraftChoiceEvent;
    public event EventHandler<DraftPickEvent>? DraftPickEvent;
    public event EventHandler<DraftLeaveEvent>? DraftLeaveEvent;

    private void StartLogWatcherTimer()
    {
        if (logWatcherTimerHandle != null)
        {
            throw new InvalidOperationException("The timer is already started.");
        }

        logWatcherTimerHandle = DispatcherTimer.Run(() =>
        {
            logWatcher.Poll();
            return true;
        }, TimeSpan.FromMilliseconds(250));
    }

    private void StartMemoryWatcherTimer()
    {
        if (memoryWatcherTimerHandle != null)
        {
            throw new InvalidOperationException("The timer is already started.");
        }

        memoryWatcherTimerHandle = DispatcherTimer.Run(() =>
        {
            memoryWatcher.Poll();
            return true;
        }, TimeSpan.FromMilliseconds(250));
    }

    private void StopMemoryWatcherTimer()
    {
        if (memoryWatcherTimerHandle == null)
        {
            throw new InvalidOperationException("The timer is already stopped.");
        }

        memoryWatcherTimerHandle.Dispose();
        memoryWatcherTimerHandle = null;
    }

    private bool IsMemoryWatcherTimerRunning()
    {
        return memoryWatcherTimerHandle != null;
    }

    private void DraftStartHandler(object? sender, DraftEnterEvent e)
    {
        logger.Info($"Entered the draft.");

        dispatcher.RemoveMatcher<DraftEnterMatcher>();
        dispatcher.AddMatcher<DraftChoiceMatcher>().DraftChoiceEvent += DraftChoiceHandler;
        dispatcher.AddMatcher<DraftPickMatcher>().DraftPickEvent += DraftPickHandler;
        dispatcher.AddMatcher<DraftLeaveMatcher>().DraftLeaveEvent += DraftLeaveHandler;

        StartMemoryWatcherTimer();
        memoryWatcher.DraftChoiceEvent += DraftChoiceHandler;

        OnRaiseDraftStartEvent(e);
    }

    private void DraftChoiceHandler(object? sender, DraftChoiceEvent e)
    {
        logger.Info($"Draft {e.DraftId} - P{e.PackNumber}P{e.PickNumber}. Available cards [{string.Join(",", e.CardsInPack)}]");

        if(IsMemoryWatcherTimerRunning())
        {
            memoryWatcher.DraftChoiceEvent -= DraftChoiceHandler;
            StopMemoryWatcherTimer();
        }

        OnRaiseDraftChoiceEvent(e);
    }

    private void DraftPickHandler(object? sender, DraftPickEvent e)
    {
        logger.Info($"Draft {e.EventName}/{e.DraftId} - P{e.PackNumber}P{e.PickNumber}. Picked {e.CardPicked} from [{string.Join(",", e.CardsInPack)}]");

        OnRaiseDraftPickEvent(e);
    }

    private void DraftLeaveHandler(object? sender, DraftLeaveEvent e)
    {
        logger.Info($"Left the draft");

        dispatcher.RemoveMatcher<DraftChoiceMatcher>();
        dispatcher.RemoveMatcher<DraftPickMatcher>();
        dispatcher.RemoveMatcher<DraftLeaveMatcher>();
        dispatcher.AddMatcher<DraftEnterMatcher>().DraftStartEvent += DraftStartHandler;

        OnRaiseDraftLeaveEvent(e);
    }

    private void OnRaiseDraftStartEvent(DraftEnterEvent e)
    {
        DraftStartEvent?.Invoke(this, e);
    }

    private void OnRaiseDraftChoiceEvent(DraftChoiceEvent e)
    {
        DraftChoiceEvent?.Invoke(this, e);
    }

    private void OnRaiseDraftPickEvent(DraftPickEvent e)
    {
        DraftPickEvent?.Invoke(this, e);
    }

    private void OnRaiseDraftLeaveEvent(DraftLeaveEvent e)
    {
        DraftLeaveEvent?.Invoke(this, e);
    }

    private LogFileWatcher logWatcher;
    private MemoryWatcher memoryWatcher;
    private MatcherDispatcher dispatcher;

    private IDisposable? logWatcherTimerHandle;
    private IDisposable? memoryWatcherTimerHandle;

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
