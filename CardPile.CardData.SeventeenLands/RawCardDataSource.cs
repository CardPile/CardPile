using CardPile.CardData.Ranking;
using System.Collections;

namespace CardPile.CardData.SeventeenLands;

internal class RawCardDataSource : IEnumerable<KeyValuePair<int, RawCardData>>
{
    internal RawCardDataSource(List<RawCardData> cardData, List<Color> rankColors, int maxRank)
    {
        List<MetricRankCalculator> calculatorInfos = new List<MetricRankCalculator>()
        {
            new((lhs, rhs) =>  Comparer<float?>.Default.Compare(rhs.WinRate, lhs.WinRate), c => c.WinRate != null, (c, r) => c.WinRateRanks.Add(r)),
            new((lhs, rhs) =>  Comparer<float?>.Default.Compare(rhs.OpeningHandWinRate, lhs.OpeningHandWinRate), c => c.OpeningHandWinRate != null, (c, r) => c.OpeningHandWinRateRanks.Add(r)),
            new((lhs, rhs) =>  Comparer<float?>.Default.Compare(rhs.DrawnWinRate, lhs.DrawnWinRate), c => c.DrawnWinRate != null, (c, r) => c.DrawnWinRateRanks.Add(r)),
            new((lhs, rhs) =>  Comparer<float?>.Default.Compare(rhs.EverDrawnWinRate, lhs.EverDrawnWinRate), c => c.EverDrawnWinRate != null, (c, r) => c.EverDrawnWinRateRanks.Add(r)),
            new((lhs, rhs) => -Comparer<float?>.Default.Compare(rhs.NeverDrawnWinRate, lhs.NeverDrawnWinRate), c => c.NeverDrawnWinRate != null, (c, r) => c.NeverDrawnWinRateRanks.Add(r)),
            new((lhs, rhs) =>  Comparer<float?>.Default.Compare(rhs.DrawnImprovementWinRate, lhs.DrawnImprovementWinRate), c => c.DrawnImprovementWinRate != null, (c, r) => c.DrawnImprovementWinRateRanks.Add(r)),
        };

        var importanceCalculator = (int rank) => rank < 10 ? Importance.ImportanceLevel.Critical : (rank < 20) ? Importance.ImportanceLevel.High : (rank >= 30) ? Importance.ImportanceLevel.Low : Importance.ImportanceLevel.Regular;

        foreach (var info in calculatorInfos)
        {
            void rankAdder(RawCardData c, int r, string rarity = "")
            {
                if(r < maxRank)
                {
                    info.RankAssigner(c, new Rank(rarity, r, importanceCalculator(r)));
                }
            }

            void colorCombinationRankAdder(RawCardData c, int r, string color, string tag = "")
            {
                if (r < maxRank)
                {
                    info.RankAssigner(c, new Rank(color + (!string.IsNullOrEmpty(tag) ? " " + tag : ""), r, importanceCalculator(r)));
                }
            }

            CalculateCardRanks(cardData, x => info.NullChecker(x),                                                         info.Comparison, (c, r) => rankAdder(c, r, "whole set"));
            CalculateCardRanks(cardData, x => info.NullChecker(x) && x.Rarity == Rarity.Common,                            info.Comparison, (c, r) => rankAdder(c, r, "commons"));
            CalculateCardRanks(cardData, x => info.NullChecker(x) && x.Rarity == Rarity.Uncommon,                          info.Comparison, (c, r) => rankAdder(c, r, "uncommons"));
            CalculateCardRanks(cardData, x => info.NullChecker(x) && x.Rarity == Rarity.Rare || x.Rarity == Rarity.Mythic, info.Comparison, (c, r) => rankAdder(c, r, "rares+"));

            foreach (var colorCombination in rankColors)
            {
                var colorCombinationLabel = ColorsUtil.ToWUBRG(colorCombination);

                CalculateCardRanks(cardData, x => info.NullChecker(x) && (x.Colors & colorCombination) == x.Colors,                                                         info.Comparison, (c, r) => colorCombinationRankAdder(c, r, colorCombinationLabel));
                CalculateCardRanks(cardData, x => info.NullChecker(x) && (x.Colors & colorCombination) == x.Colors && x.Rarity == Rarity.Common,                            info.Comparison, (c, r) => colorCombinationRankAdder(c, r, colorCombinationLabel, "commons"));
                CalculateCardRanks(cardData, x => info.NullChecker(x) && (x.Colors & colorCombination) == x.Colors && x.Rarity == Rarity.Uncommon,                          info.Comparison, (c, r) => colorCombinationRankAdder(c, r, colorCombinationLabel, "uncommons"));
                CalculateCardRanks(cardData, x => info.NullChecker(x) && (x.Colors & colorCombination) == x.Colors && x.Rarity == Rarity.Rare || x.Rarity == Rarity.Mythic, info.Comparison, (c, r) => colorCombinationRankAdder(c, r, colorCombinationLabel, "rares+"));
            }
        }

        cardDataSet = cardData.ToDictionary(x => x.ArenaCardId, x => x);
    }

    public RawCardData? GetDataForCard(int cardNumber)
    {
        return cardDataSet.TryGetValue(cardNumber, out RawCardData? cardData) ? cardData : null;
    }

    public IEnumerator<KeyValuePair<int, RawCardData>> GetEnumerator()
    {
        return cardDataSet.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return cardDataSet.GetEnumerator();
    }

    private class MetricRankCalculator
    {
        public MetricRankCalculator(Comparison<RawCardData> comparison, Func<RawCardData, bool> nullChecker, Action<RawCardData, Rank> rankAssigner)
        {
            Comparison = comparison;
            NullChecker = nullChecker;
            RankAssigner = rankAssigner;
        }

        public Comparison<RawCardData> Comparison { get; set; }

        public Func<RawCardData, bool> NullChecker { get; set; }

        public Action<RawCardData, Rank> RankAssigner { get; set; }
    }

    private static void CalculateCardRanks(List<RawCardData> cardData, Func<RawCardData, bool> filter, Comparison<RawCardData> metricComparison, Action<RawCardData, int> rankAssigner)
    {
        var filteredCardList = cardData.Where(x => filter(x)).ToList();
        filteredCardList.Sort(metricComparison);
        foreach (var (card, rank) in filteredCardList.Select((value, i) => (value, i)))
        {
            rankAssigner(card, rank);
        }
    }

    private readonly Dictionary<int, RawCardData> cardDataSet = [];
}
