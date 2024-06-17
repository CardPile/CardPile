namespace CardPile.Parser;

public class NewLineEvent : EventArgs
{
    public NewLineEvent(string line)
    {
        Line = line;
    }

    public string Line { get; set; }
}
