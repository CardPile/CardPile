using System.Reflection.Metadata.Ecma335;

namespace CardPile.Parser;

public class MatcherDispatcher
{
    public void Dispatch(string line)
    {
        foreach (ILogMatcher matcher in logMatchers)
        {
            if (matcher.Match(line))
            {
                return;
            }
        }
    }

    public void Connect(LogFileWatcher watcher)
    {
        watcher.NewLineEvent += (source, e) => Dispatch(e.Line);
    }

    public T AddMatcher<T>() where T : ILogMatcher, new()
    {
        var matcher = new T();
        logMatchers.Add(matcher);
        return matcher;
    }

    public void RemoveMatcher<T>() where T : ILogMatcher
    {
        logMatchers.RemoveAll(matcher => matcher.GetType() == typeof(T));
    }

    public T? GetMatcher<T>() where T : class, ILogMatcher
    {
        return logMatchers.Find(matcher => matcher.GetType() == typeof(T)) as T;
    }

    private List<ILogMatcher> logMatchers = new List<ILogMatcher>();
}
