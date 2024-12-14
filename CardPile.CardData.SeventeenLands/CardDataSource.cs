using CardPile.CardData.Formatting;
using CardPile.CardData.Importance;
using CardPile.CardData.Metrics;
using CardPile.Draft;
using MathNet.Numerics.Distributions;

namespace CardPile.CardData.SeventeenLands;

public class CardDataSource : ICardDataSource
{
    internal CardDataSource(RawCardDataSource cardData,
                            RawCardDataSource wuCardData,
                            RawCardDataSource wbCardData,
                            RawCardDataSource wrCardData,
                            RawCardDataSource wgCardData,
                            RawCardDataSource ubCardData,
                            RawCardDataSource urCardData,
                            RawCardDataSource ugCardData,
                            RawCardDataSource brCardData,
                            RawCardDataSource bgCardData,
                            RawCardDataSource rgCardData,
                            WinDataSource winData)
    {
        archetypeCardData[ColorPair.None] = cardData;
        archetypeCardData[ColorPair.WU] = wuCardData;
        archetypeCardData[ColorPair.WB] = wbCardData;
        archetypeCardData[ColorPair.WR] = wrCardData;
        archetypeCardData[ColorPair.WG] = wgCardData;
        archetypeCardData[ColorPair.UB] = ubCardData;
        archetypeCardData[ColorPair.UR] = urCardData;
        archetypeCardData[ColorPair.UG] = ugCardData;
        archetypeCardData[ColorPair.BR] = brCardData;
        archetypeCardData[ColorPair.BG] = bgCardData;
        archetypeCardData[ColorPair.RG] = rgCardData;

        archetypeWinData = winData;

        foreach (var archetypeEntry in archetypeCardData)
        {
            var gihWrMean = Mean(archetypeEntry.Value, (RawCardData data) => (data.EverDrawnGameCount ?? 0) > CARD_EVER_DRAWN_CUTOFF ? data.EverDrawnWinRate : null);
            var gihWrStdDev = (float)Math.Sqrt(Variance(archetypeEntry.Value, gihWrMean, (RawCardData data) => (data.EverDrawnGameCount ?? 0) > CARD_EVER_DRAWN_CUTOFF ? data.EverDrawnWinRate : null));
            archetypeGihWrDistribution[archetypeEntry.Key] = new Normal(gihWrMean, gihWrStdDev);
        }

        var avgGihWr = new Statistic<float>("Avg GIH WR", (float)archetypeGihWrDistribution[ColorPair.None].Mean, new PercentFormatter());
        Statistics = [avgGihWr];

        foreach (ColorPair pair in Enum.GetValues<ColorPair>().Cast<ColorPair>())
        {
            float? winRate = archetypeWinData.GetWinPercentage((Color)(int)pair);
            if(!winRate.HasValue)
            {
                continue;
            }

            var colroPairWinRateName = string.Format("{0} WR", Enum.GetName(pair));
            var colorPairWinRateStatistic = new Statistic<float>(colroPairWinRateName, winRate.Value, new PercentFormatter());
            Statistics.Add(colorPairWinRateStatistic);
        }
    }

    public string Name => "17Lands";

    public ICardData? GetDataForCard(int cardNumber, DraftState state)
    {
        RawCardData? rawCardData = archetypeCardData[ColorPair.None].GetDataForCard(cardNumber);
        if (rawCardData != null)
        {
            Dictionary<ColorPair, float?> gameWinRateImprovement = [];
            foreach (ColorPair pair in Enum.GetValues<ColorPair>().Cast<ColorPair>())
            {
                float? winRate = archetypeWinData.GetWinPercentage((Color)(int)pair);
                if (!winRate.HasValue)
                {
                    gameWinRateImprovement[pair] = null;
                    continue;
                }

                float? gihWr = archetypeCardData[pair].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate;
                if(!gihWr.HasValue)
                {
                    gameWinRateImprovement[pair] = null;
                    continue;
                }

                gameWinRateImprovement[pair] = gihWr.Value - winRate.Value;
            }

            return new CardData(rawCardData.Name,
                                rawCardData.ArenaCardId,
                                rawCardData.Colors,
                                rawCardData.Url,
                                CardData.SeenMetricDesc.NewMetric(rawCardData.SeenCount),
                                CardData.AverageLastSeenAtMetricDesc.NewMetric(rawCardData.AvgSeen, ImportanceCalculators.BelowOffset(3f, 1f, () => state.LastPick)),
                                CardData.PickedMetricDesc.NewMetric(rawCardData.PickCount),
                                CardData.AveragePickedAtMetricDesc.NewMetric(rawCardData.AvgPick, ImportanceCalculators.BelowOffset(3f, 1f, () => state.LastPick)),
                                CardData.NumberOfGamesPlayedMetricDesc.NewMetric(rawCardData.GameCount),
                                CardData.PlayRateMetricDesc.NewMetric(rawCardData.PlayRate),
                                CardData.WinRateWhenMaindeckedMetricDesc.NewMetric(rawCardData.WinRate),
                                CardData.NumberOfGamesInOpeningHandMetricDesc.NewMetric(rawCardData.OpeningHandGameCount),
                                CardData.WinRateInOpeningHandMetricDesc.NewMetric(rawCardData.OpeningHandWinRate),
                                CardData.NumberOfGamesDrawnTurn1OrLaterMetricDesc.NewMetric(rawCardData.DrawnGameCount),
                                CardData.WinRateWhenDrawnTurn1OrLaterMetricDesc.NewMetric(rawCardData.DrawnWinRate),
                                CardData.NumberOfGamesInHandMetricDesc.NewMetric(rawCardData.EverDrawnGameCount),
                                CardData.WinRateInHandMetricDesc.NewMetric(rawCardData.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.None].Mean)),
                                CardData.ColorsWinRateInHandMetricDesc.NewMetric(
                                    CardData.WUWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.WU].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.WU].Mean)),
                                    CardData.WBWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.WB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.WB].Mean)),
                                    CardData.WRWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.WR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.WR].Mean)),
                                    CardData.WGWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.WG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.WG].Mean)),
                                    CardData.UBWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.UB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.UB].Mean)),
                                    CardData.URWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.UR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.UR].Mean)),
                                    CardData.UGWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.UG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.UG].Mean)),
                                    CardData.BRWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.BR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.BR].Mean)),
                                    CardData.BGWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.BG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.BG].Mean)),
                                    CardData.RGWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.RG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.RG].Mean))
                                ),
                                CardData.ColorsWinRateImprovementMetricDesc.NewMetric(
                                    CardData.WUWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.WU], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.WBWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.WB], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.WRWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.WR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.WGWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.WG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.UBWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.UB], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.URWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.UR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.UGWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.UG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.BRWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.BR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.BGWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.BG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.RGWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[ColorPair.RG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD))
                                ),
                                CardData.NumberOfGamesNotSeenMetricDesc.NewMetric(rawCardData.NeverDrawnGameCount),
                                CardData.WinRateNotSeenMetricDesc.NewMetric(rawCardData.NeverDrawnWinRate),
                                CardData.WinRateImprovementWhenDrawnMetricDesc.NewMetric(rawCardData.DrawnImprovementWinRate));
        }

        string? cardNameFromArena = CardInfo.Arena.GetCardNameFromId(cardNumber);
        if (cardNameFromArena != null)
        {
            var (expansion, collectorNumber) = CardInfo.Arena.GetCardExpansionAndCollectorNumberFromId(cardNumber);
            string? url = null;
            if (expansion != null && collectorNumber != null)
            {
                url = CardInfo.Scryfall.GetImageUrlFromExpansionAndCollectorNumber(expansion, collectorNumber);
            }

            var colors = CardInfo.Arena.GetCardColorsFromId(cardNumber) ?? [];
            return new CardData(cardNameFromArena, cardNumber, colors, url);
        }

        return null;
    }
    
    public List<ICardDataSourceStatistic> Statistics { get; init; }

    private const int CARD_EVER_DRAWN_CUTOFF = 500;

    private const float CARD_GIH_WR_CRITICAL_THRESHOLD = 0.05f;
    private const float CARD_GIH_WR_HIGH_THRESHOLD = 0.015f;
    private const float CARD_GIH_WR_REGULAR_THRESHOLD = -0.2f;

    private const float CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD = 0.03f;
    private const float CARD_WR_IMPROVEMENT_HIGH_THRESHOLD = 0.01f;
    private const float CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD = -0.1f;

    private float Mean(RawCardDataSource source, Func<RawCardData, float?> selector)
    {
        float sum = 0f;
        int count = 0;

        foreach (var card in source)
        {
            var value = selector(card.Value);
            if(value == null)
            {
                continue;
            }
            sum += value.Value;
            count++;
        }

        if (count == 0)
        {
            return 0f;
        }

        return sum / count;
    }

    private float Variance(RawCardDataSource source, float mean, Func<RawCardData, float?> selector)
    {
        float sum = 0f;
        int count = 0;

        foreach (var card in source)
        {
            var value = selector(card.Value);
            if(value == null)
            {
                continue;
            }
            sum += (value.Value - mean) * (value.Value - mean);
            count++;
        }

        if (count == 0 || count == 1)
        {
            return 1f;
        }

        return sum / (count - 1);
    }

    private Dictionary<ColorPair, RawCardDataSource> archetypeCardData = [];
    private Dictionary<ColorPair, Normal> archetypeGihWrDistribution = [];
    private WinDataSource archetypeWinData;
}
