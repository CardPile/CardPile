using CardPile.App.Services;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using ReactiveUI;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace CardPile.App.Models;

internal class LogModel : ReactiveObject, ILogService
{
    internal LogModel()
    {
        logContents = string.Empty;

        var config = new NLog.Config.LoggingConfiguration();
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, new FileTarget(){ FileName = "${basedir}/CardPile.log"});
        
        var target = new Target(this);
        target.Layout = new SimpleLayout { Text = "${longdate} ${logger} [${uppercase:${level}}]: ${message} ${exception:format=tostring}" };
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, target);
        
        LogManager.Configuration = config;
    }

    public string LogContents
    {
        get => logContents;
        set => this.RaiseAndSetIfChanged(ref logContents, value);
    }

    private class Target : TargetWithLayout
    {
        public Target(LogModel parent)
        {
            this.Parent = parent;
        }

        private LogModel Parent { get; init; }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = RenderLogEvent(this.Layout, logEvent);
            Parent.LogContents = logMessage + Environment.NewLine + Parent.LogContents;
        }
    }

    private string logContents;
}
