namespace CardPile.Draft;

public class DraftPack
{
    public int PackNumber { get; set; } = 1;

    public int PickNumber { get; set; } = 1;

    public List<int> Cards { get; set; } = [];
}
