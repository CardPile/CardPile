using CardPile.CardData;
using CardPile.CardData.Importance;
using System.Collections.Generic;

namespace CardPile.Application.ViewModels.Design;

public class CardMetricViewModelDesign
{
    internal class CardMetricDescriptionDesign : ICardMetricDescription
    {
        public string Name => "Name";

        public bool IsDefaultVisible => true;

        public bool IsDefaultMetric => false;

        public IComparer<ICardMetric> Comparer => null;
    }

    internal class CardRankDesign : ICardRank
    {
        public string Name => "Rank";

        public int Value => 1;

        public string TextValue => "One";

        public ImportanceLevel Importance => ImportanceLevel.High;
    }

    internal class CardMetricDesign : ICardMetric
    {
        public ICardMetricDescription Description => new CardMetricDescriptionDesign();

        public bool HasValue => true;

        public string TextValue => "Value";

        public ImportanceLevel Importance => ImportanceLevel.High;

        public IList<ICardRank> Ranks => [new CardRankDesign()];
    }

    public CardMetricViewModelDesign()
    {
        cardMetric = new CardMetricDesign();
    }

    internal ICardMetric Metric
    {
        get => cardMetric;
    }

    internal bool Visible
    {
        get => true;
    }

    private ICardMetric cardMetric;
}