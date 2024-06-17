namespace CardPile.Draft;

public class DraftEndEvent : DraftEvent
{
    public DraftEndEvent(string eventName)
    {
        EventName = eventName;
    }
    
    public string EventName { get; set; }
}
