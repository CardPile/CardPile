namespace CardPile.Draft;

public class DraftChoiceEvent : DraftEvent
{
    public DraftChoiceEvent(Guid draftId, int packNumber, int pickNumber, List<int> cardsInPack)
    {
        DraftId = draftId;
        PackNumber = packNumber;
        PickNumber = pickNumber;
        CardsInPack = cardsInPack;
    }

    public Guid DraftId { get; set; }

    public int PackNumber { get; set; }

    public int PickNumber { get; set; }

    public List<int> CardsInPack { get; set; }
}
