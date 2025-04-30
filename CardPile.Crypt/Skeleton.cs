namespace CardPile.Crypt;

public class Skeleton
{
    internal Skeleton(string name, string set)
    {
        Name = name;
        Set = set;
    }

    public string Name { get; init; }

    public string Set { get; init; }

    public List<CardGroup> Groups { get; } = [];

    internal bool CanBeSatisfied()
    {
        foreach (var group in Groups)
        {
            if(!group.CanBeSatisfied())
            {
                return false;
            }
        }

        return true;
    }
}
