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
        archetypeCardData[Color.None] = cardData;
        archetypeCardData[Color.WU] = wuCardData;
        archetypeCardData[Color.WB] = wbCardData;
        archetypeCardData[Color.WR] = wrCardData;
        archetypeCardData[Color.WG] = wgCardData;
        archetypeCardData[Color.UB] = ubCardData;
        archetypeCardData[Color.UR] = urCardData;
        archetypeCardData[Color.UG] = ugCardData;
        archetypeCardData[Color.BR] = brCardData;
        archetypeCardData[Color.BG] = bgCardData;
        archetypeCardData[Color.RG] = rgCardData;

        archetypeWinData = winData;

        foreach (var archetypeEntry in archetypeCardData)
        {
            var gihWrMean = Mean(archetypeEntry.Value, (data) => (data.EverDrawnGameCount ?? 0) > CARD_EVER_DRAWN_CUTOFF ? data.EverDrawnWinRate : null);
            var gihWrStdDev = (float)Math.Sqrt(Variance(archetypeEntry.Value, gihWrMean, (data) => (data.EverDrawnGameCount ?? 0) > CARD_EVER_DRAWN_CUTOFF ? data.EverDrawnWinRate : null));
            archetypeGihWrDistribution[archetypeEntry.Key] = new Normal(gihWrMean, gihWrStdDev);
        }

        var avgGihWr = new Statistic<float>("Avg GIH WR", (float)archetypeGihWrDistribution[Color.None].Mean, new PercentFormatter());
        Statistics = [avgGihWr];

        foreach (Color pair in ColorsUtil.ColorPairs())
        {
            float? winRate = archetypeWinData.GetWinPercentage((Color)(int)pair);
            if(!winRate.HasValue)
            {
                continue;
            }

            var colorPairWinRateName = $"{ColorsUtil.ToEmoji(pair)} WR";
            var colorPairWinRateStatistic = new Statistic<float>(colorPairWinRateName, winRate.Value, new PercentFormatter());
            Statistics.Add(colorPairWinRateStatistic);
        }
    }

    public string Name => "17Lands";

    public ICardData? GetDataForCard(int cardNumber, DraftState state)
    {
        RawCardData? rawCardData = archetypeCardData[Color.None].GetDataForCard(cardNumber);
        if (rawCardData != null)
        {
            Dictionary<Color, float?> gameWinRateImprovement = [];
            foreach (Color pair in ColorsUtil.ColorPairs())
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
                                CardData.WinRateInHandMetricDesc.NewMetric(rawCardData.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.None].Mean)),
                                CardData.ColorsWinRateInHandMetricDesc.NewMetric(
                                    CardData.WUWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WU].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WU].Mean)),
                                    CardData.WBWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WB].Mean)),
                                    CardData.WRWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WR].Mean)),
                                    CardData.WGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WG].Mean)),
                                    CardData.UBWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.UB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.UB].Mean)),
                                    CardData.URWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.UR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.UR].Mean)),
                                    CardData.UGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.UG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.UG].Mean)),
                                    CardData.BRWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.BR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.BR].Mean)),
                                    CardData.BGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.BG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.BG].Mean)),
                                    CardData.RGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.RG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.RG].Mean))
                                ),
                                CardData.ColorsWinRateImprovementMetricDesc.NewMetric(
                                    CardData.WUWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.WU], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.WBWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.WB], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.WRWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.WR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.WGWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.WG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.UBWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.UB], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.URWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.UR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.UGWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.UG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.BRWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.BR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.BGWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.BG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD)),
                                    CardData.RGWinRateImprovementMetricDesc.NewMetric(gameWinRateImprovement[Color.RG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD))
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

    private readonly Dictionary<Color, RawCardDataSource> archetypeCardData = [];
    private readonly Dictionary<Color, Normal> archetypeGihWrDistribution = [];
    private readonly WinDataSource archetypeWinData;
}
