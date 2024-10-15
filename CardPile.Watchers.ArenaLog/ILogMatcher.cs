namespace CardPile.Watchers.ArenaLog;

public interface ILogMatcher
{
    abstract bool Match(string line);
}
