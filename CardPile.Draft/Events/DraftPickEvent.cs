namespace CardPile.Draft;

public class DraftPickEvent : DraftEvent
{
    public DraftPickEvent(Guid draftId, int packNumber, int pickNumber, List<int> cardsPicked)
    {
        DraftId = draftId;
        PackNumber = packNumber;
        PickNumber = pickNumber;
        CardsPicked = cardsPicked;
    }

    public Guid DraftId { get; set; }

    public int PackNumber { get; set; }

    public int PickNumber { get; set; }

    public List<int> CardsPicked {  get; set; }    
}
