namespace CardPile.Draft;

public class DraftPickEvent : DraftEvent
{
    public DraftPickEvent(Guid draftId, int packNumber, int pickNumber, int cardPicked)
    {
        DraftId = draftId;
        PackNumber = packNumber;
        PickNumber = pickNumber;
        CardPicked = cardPicked;
    }

    public Guid DraftId { get; set; }

    public int PackNumber { get; set; }

    public int PickNumber { get; set; }

    public int CardPicked {  get; set; }    
}
