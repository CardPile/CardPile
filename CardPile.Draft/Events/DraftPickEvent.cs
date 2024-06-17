namespace CardPile.Draft;

public class DraftPickEvent : DraftEvent
{
    public DraftPickEvent(Guid draftId, string eventName, int packNumber, int pickNumber, List<int> cardsInPack, int cardPicked)
    {
        DraftId = draftId;
        EventName = eventName;
        PackNumber = packNumber;
        PickNumber = pickNumber;
        CardsInPack = cardsInPack;
        CardPicked = cardPicked;
    }

    public Guid DraftId { get; set; }

    public string EventName { get; set; }

    public int PackNumber { get; set; }

    public int PickNumber { get; set; }

    public List<int> CardsInPack { get; set; }

    public int CardPicked {  get; set; }    
}
