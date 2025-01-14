using CardPile.CardData.Importance;

namespace CardPile.CardData;

public interface ICardMetric
{
    public ICardMetricDescription Description { get; }

    public bool HasValue { get; }

    public string TextValue { get; }

    public ImportanceLevel Importance { get; }

    public IList<ICardRank> Ranks { get; }
};
