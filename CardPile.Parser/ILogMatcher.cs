namespace CardPile.Parser;

public interface ILogMatcher
{
    abstract bool Match(string line);
}
