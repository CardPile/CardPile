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
                            RawCardDataSource wubCardData,
                            RawCardDataSource wurCardData,
                            RawCardDataSource wugCardData,
                            RawCardDataSource wbrCardData,
                            RawCardDataSource wbgCardData,
                            RawCardDataSource wrgCardData,
                            RawCardDataSource ubrCardData,
                            RawCardDataSource ubgCardData,
                            RawCardDataSource urgCardData,
                            RawCardDataSource brgCardData,
                            List<Color> winRateColors,
                            WinDataSource winData,
                            DEqCalculator calculator)
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
        archetypeCardData[Color.WUB] = wubCardData;
        archetypeCardData[Color.WUR] = wurCardData;
        archetypeCardData[Color.WUG] = wugCardData;
        archetypeCardData[Color.WBR] = wbrCardData;
        archetypeCardData[Color.WBG] = wbgCardData;
        archetypeCardData[Color.WRG] = wrgCardData;
        archetypeCardData[Color.UBR] = ubrCardData;
        archetypeCardData[Color.UBG] = ubgCardData;
        archetypeCardData[Color.URG] = urgCardData;
        archetypeCardData[Color.BRG] = brgCardData;

        archetypeWinData = winData;
        deqCaculator = calculator;

        foreach (var archetypeEntry in archetypeCardData)
        {
            var gihWrMean = Mean(archetypeEntry.Value, (data) => (data.EverDrawnGameCount ?? 0) > CARD_EVER_DRAWN_CUTOFF ? data.EverDrawnWinRate : null);
            var gihWrStdDev = (float)Math.Sqrt(Variance(archetypeEntry.Value, gihWrMean, (data) => (data.EverDrawnGameCount ?? 0) > CARD_EVER_DRAWN_CUTOFF ? data.EverDrawnWinRate : null));
            archetypeGihWrDistribution[archetypeEntry.Key] = new Normal(gihWrMean, gihWrStdDev);
        }

        var avgGihWr = new Statistic<float>("Avg GIH WR", (float)archetypeGihWrDistribution[Color.None].Mean, new PercentFormatter());
        Statistics = [avgGihWr];

        winRateColors.Sort((a, b) => -Comparer<float?>.Default.Compare(archetypeWinData.GetWinPercentage((Color)(int)a), archetypeWinData.GetWinPercentage((Color)(int)b))); 
        
        foreach (Color pair in winRateColors)
        {
            float? winRate = archetypeWinData.GetWinPercentage((Color)(int)pair);
            if(!winRate.HasValue)
            {
                continue;
            }

            var colorPairWinRateName = $"{ColorsUtil.ToSymbols(pair)} WR";
            var colorPairWinRateStatistic = new Statistic<float>(colorPairWinRateName, winRate.Value, new PercentFormatter());
            Statistics.Add(colorPairWinRateStatistic);
        }
    }

    public string Name => "17Lands";

    public ICardData? GetDataForCard(int cardNumber, DraftState? state = null)
    {
        RawCardData? rawCardData = archetypeCardData[Color.None].GetDataForCard(cardNumber);
        var type = CardInfo.Arena.GetCardTypeFromId(cardNumber);
        var manaValue = CardInfo.Arena.GetCardManaValueFromId(cardNumber);
        if (rawCardData != null)
        {
            Dictionary<Color, float?> pairGameWinRateImprovement = [];
            foreach (Color pair in ColorsUtil.ColorPairs())
            {
                float? winRate = archetypeWinData.GetWinPercentage((Color)(int)pair);
                if (!winRate.HasValue)
                {
                    pairGameWinRateImprovement[pair] = null;
                    continue;
                }

                float? gpWr = archetypeCardData[pair].GetDataForCard(rawCardData.ArenaCardId)?.WinRate;
                if(!gpWr.HasValue)
                {
                    pairGameWinRateImprovement[pair] = null;
                    continue;
                }

                pairGameWinRateImprovement[pair] = gpWr.Value - winRate.Value;
            }

            Dictionary<Color, float?> tripleGameWinRateImprovement = [];
            foreach (Color pair in ColorsUtil.ColorTriples())
            {
                float? winRate = archetypeWinData.GetWinPercentage((Color)(int)pair);
                if (!winRate.HasValue)
                {
                    tripleGameWinRateImprovement[pair] = null;
                    continue;
                }

                float? gpWr = archetypeCardData[pair].GetDataForCard(rawCardData.ArenaCardId)?.WinRate;
                if (!gpWr.HasValue)
                {
                    tripleGameWinRateImprovement[pair] = null;
                    continue;
                }

                tripleGameWinRateImprovement[pair] = gpWr.Value - winRate.Value;
            }

            ICardMetric[] colorPairsWinRateInHandMetrics = PrepareForCompositeMetric
            (
                new MetricComparer<float>(),
                CardData.WUWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WU].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WU].Mean), archetypeCardData[Color.WU].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.WBWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WB].Mean), archetypeCardData[Color.WB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.WRWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WR].Mean), archetypeCardData[Color.WR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.WGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WG].Mean), archetypeCardData[Color.WG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.UBWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.UB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.UB].Mean), archetypeCardData[Color.UB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.URWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.UR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.UR].Mean), archetypeCardData[Color.UR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.UGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.UG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.UG].Mean), archetypeCardData[Color.UG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.BRWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.BR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.BR].Mean), archetypeCardData[Color.BR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.BGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.BG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.BG].Mean), archetypeCardData[Color.BG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.RGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.RG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.RG].Mean), archetypeCardData[Color.RG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? [])
            );
            ICardMetric[] colorPairsWinRateImprovementMetrics = PrepareForCompositeMetric
            (
                new MetricComparer<float>(),
                CardData.WUWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.WU], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.WBWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.WB], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.WRWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.WR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.WGWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.WG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.UBWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.UB], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.URWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.UR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.UGWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.UG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.BRWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.BR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.BGWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.BG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.RGWinRateImprovementMetricDesc.NewMetric(pairGameWinRateImprovement[Color.RG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), [])
            );
            ICardMetric[] colorTriplesWinRateInHandMetrics = PrepareForCompositeMetric
            (
                new MetricComparer<float>(),
                CardData.WUBWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WUB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WUB].Mean), archetypeCardData[Color.WUB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.WURWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WUR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WUR].Mean), archetypeCardData[Color.WUR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.WUGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WUG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WUG].Mean), archetypeCardData[Color.WUG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.WBRWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WBR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WBR].Mean), archetypeCardData[Color.WBR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.WBGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WBG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WBG].Mean), archetypeCardData[Color.WBG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.WRGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.WRG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.WRG].Mean), archetypeCardData[Color.WRG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.UBRWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.UBR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.UBR].Mean), archetypeCardData[Color.UBR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.UBGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.UBG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.UBG].Mean), archetypeCardData[Color.UBG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.URGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.URG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.URG].Mean), archetypeCardData[Color.URG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? []),
                CardData.BRGWinRateInHandMetricDesc.NewMetric(archetypeCardData[Color.BRG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.BRG].Mean), archetypeCardData[Color.BRG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRateRanks ?? [])
            );
            ICardMetric[] colorTriplesWinRateImprovementMetrics = PrepareForCompositeMetric
            (
                new MetricComparer<float>(),
                CardData.WUBWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.WUB], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.WURWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.WUR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.WUGWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.WUG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.WBRWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.WBR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.WBGWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.WBG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.WRGWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.WRG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.UBRWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.UBR], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.UBGWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.UBG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.URGWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.URG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), []),
                CardData.BRGWinRateImprovementMetricDesc.NewMetric(tripleGameWinRateImprovement[Color.BRG], ImportanceCalculators.AboveThreshold(CARD_WR_IMPROVEMENT_CRITICAL_THRESHOLD, CARD_WR_IMPROVEMENT_HIGH_THRESHOLD, CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD), [])
            );

            var (deq, deqRanks) = deqCaculator.GetDEqAndRanks(rawCardData.ArenaCardId);
            var deqGrade = deqCaculator.GetDEqGrade(rawCardData.ArenaCardId);

            return new CardData(rawCardData.Name,
                                rawCardData.ArenaCardId,
                                type,
                                manaValue,
                                rawCardData.Colors,
                                rawCardData.Url,
                                CardData.SeenMetricDesc.NewMetric(rawCardData.SeenCount, rawCardData.SeenCountRanks),
                                CardData.AverageLastSeenAtMetricDesc.NewMetric(rawCardData.AvgSeen, state != null ? ImportanceCalculators.BelowOffset(3f, 1f, () => state.LastPick) : ImportanceCalculators.Regular<float>(), rawCardData.AvgSeenRanks),
                                CardData.PickedMetricDesc.NewMetric(rawCardData.PickCount, rawCardData.PickCountRanks),
                                CardData.AveragePickedAtMetricDesc.NewMetric(rawCardData.AvgPick, state != null ? ImportanceCalculators.BelowOffset(3f, 1f, () => state.LastPick) : ImportanceCalculators.Regular<float>(), rawCardData.AvgPickRanks),
                                CardData.NumberOfGamesPlayedMetricDesc.NewMetric(rawCardData.GameCount, rawCardData.GameCountRanks),
                                CardData.PlayRateMetricDesc.NewMetric(rawCardData.PlayRate, rawCardData.PlayRateRanks),
                                CardData.WinRateWhenMaindeckedMetricDesc.NewMetric(rawCardData.WinRate, rawCardData.WinRateRanks),
                                CardData.NumberOfGamesInOpeningHandMetricDesc.NewMetric(rawCardData.OpeningHandGameCount, rawCardData.OpeningHandGameCountRanks),
                                CardData.WinRateInOpeningHandMetricDesc.NewMetric(rawCardData.OpeningHandWinRate, rawCardData.OpeningHandWinRateRanks),
                                CardData.NumberOfGamesDrawnTurn1OrLaterMetricDesc.NewMetric(rawCardData.DrawnGameCount, rawCardData.DrawnGameCountRanks),
                                CardData.WinRateWhenDrawnTurn1OrLaterMetricDesc.NewMetric(rawCardData.DrawnWinRate, rawCardData.DrawnWinRateRanks),
                                CardData.NumberOfGamesInHandMetricDesc.NewMetric(rawCardData.EverDrawnGameCount, rawCardData.EverDrawnGameCountRanks),
                                CardData.WinRateInHandMetricDesc.NewMetric(rawCardData.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (value) => value - (float)archetypeGihWrDistribution[Color.None].Mean), rawCardData.EverDrawnWinRateRanks),
                                CardData.ColorPairsWinRateInHandMetricDesc.NewMetric(colorPairsWinRateInHandMetrics),
                                CardData.ColorPairsWinRateImprovementMetricDesc.NewMetric(colorPairsWinRateImprovementMetrics),
                                CardData.ColorTriplesWinRateInHandMetricDesc.NewMetric(colorTriplesWinRateInHandMetrics),
                                CardData.ColorTriplesWinRateImprovementMetricDesc.NewMetric(colorTriplesWinRateImprovementMetrics),
                                CardData.NumberOfGamesNotSeenMetricDesc.NewMetric(rawCardData.NeverDrawnGameCount, rawCardData.NeverDrawnGameCountRanks),
                                CardData.WinRateNotSeenMetricDesc.NewMetric(rawCardData.NeverDrawnWinRate, rawCardData.NeverDrawnWinRateRanks),
                                CardData.WinRateImprovementWhenDrawnMetricDesc.NewMetric(rawCardData.DrawnImprovementWinRate, rawCardData.DrawnImprovementWinRateRanks),
                                CardData.DEqMetricDesc.NewMetric(deq, deqRanks),
                                CardData.DEqGradeMetricDesc.NewMetric(deqGrade)
                                );
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
            
            var colors = CardInfo.Arena.GetCardColorsFromId(cardNumber);
            return new CardData(cardNameFromArena, cardNumber, type, manaValue, colors, url);
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
    private const float CARD_WR_IMPROVEMENT_REGULAR_THRESHOLD = -0.01f;

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

    private ICardMetric[] PrepareForCompositeMetric(IComparer<ICardMetric> comparer, params ICardMetric[] metrics)
    {
        Array.Sort(metrics, comparer);
        Array.Reverse(metrics);
        return metrics;
    }

    private readonly Dictionary<Color, RawCardDataSource> archetypeCardData = [];
    private readonly Dictionary<Color, Normal> archetypeGihWrDistribution = [];
    private readonly WinDataSource archetypeWinData;
    private readonly DEqCalculator deqCaculator;
}
