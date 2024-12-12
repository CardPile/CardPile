using CardPile.CardData.Formatting;
using CardPile.CardData.Importance;
using CardPile.CardData.Metrics;
using CardPile.Draft;
using MathNet.Numerics.Distributions;

namespace CardPile.CardData.SeventeenLands;

public class SeventeenLandsCardDataSource : ICardDataSource
{
    internal SeventeenLandsCardDataSource(SeventeenLandsRawCardDataSource cardData,
                                          SeventeenLandsRawCardDataSource wuCardData,
                                          SeventeenLandsRawCardDataSource wbCardData,
                                          SeventeenLandsRawCardDataSource wrCardData,
                                          SeventeenLandsRawCardDataSource wgCardData,
                                          SeventeenLandsRawCardDataSource ubCardData,
                                          SeventeenLandsRawCardDataSource urCardData,
                                          SeventeenLandsRawCardDataSource ugCardData,
                                          SeventeenLandsRawCardDataSource brCardData,
                                          SeventeenLandsRawCardDataSource bgCardData,
                                          SeventeenLandsRawCardDataSource rgCardData)
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

        foreach (var archetypeEntry in archetypeCardData)
        {
            var gihWrMean = Mean(archetypeEntry.Value, (SeventeenLandsRawCardData data) => (data.EverDrawnGameCount ?? 0) > CARD_EVER_DRAWN_CUTOFF ? data.EverDrawnWinRate : null);
            var gihWrStdDev = (float)Math.Sqrt(Variance(archetypeEntry.Value, gihWrMean, (SeventeenLandsRawCardData data) => (data.EverDrawnGameCount ?? 0) > CARD_EVER_DRAWN_CUTOFF ? data.EverDrawnWinRate : null));
            archetypeGihWrDistribution[archetypeEntry.Key] = new Normal(gihWrMean, gihWrStdDev);
        }

        var avgGihWr = new Statistic<float>("Avg GIH WR", (float)archetypeGihWrDistribution[ColorPair.None].Mean, new PercentFormatter());
        Statistics = [avgGihWr];
    }

    public string Name => "17Lands";

    public ICardData? GetDataForCard(int cardNumber, DraftState state)
    {
        SeventeenLandsRawCardData? rawCardData = archetypeCardData[ColorPair.None].GetDataForCard(cardNumber);
        if (rawCardData != null)
        {
            return new SeventeenLandsCardData(rawCardData.Name,
                                              rawCardData.ArenaCardId,
                                              rawCardData.Colors,
                                              rawCardData.Url,
                                              SeventeenLandsCardData.SeenMetricDesc.NewMetric(rawCardData.SeenCount),
                                              SeventeenLandsCardData.AverageLastSeenAtMetricDesc.NewMetric(rawCardData.AvgSeen, ImportanceCalculators.BelowOffset(3f, 1f, () => state.LastPick)),
                                              SeventeenLandsCardData.PickedMetricDesc.NewMetric(rawCardData.PickCount),
                                              SeventeenLandsCardData.AveragePickedAtMetricDesc.NewMetric(rawCardData.AvgPick, ImportanceCalculators.BelowOffset(3f, 1f, () => state.LastPick)),
                                              SeventeenLandsCardData.NumberOfGamesPlayedMetricDesc.NewMetric(rawCardData.GameCount),
                                              SeventeenLandsCardData.PlayRateMetricDesc.NewMetric(rawCardData.PlayRate),
                                              SeventeenLandsCardData.WinRateWhenMaindeckedMetricDesc.NewMetric(rawCardData.WinRate),
                                              SeventeenLandsCardData.NumberOfGamesInOpeningHandMetricDesc.NewMetric(rawCardData.OpeningHandGameCount),
                                              SeventeenLandsCardData.WinRateInOpeningHandMetricDesc.NewMetric(rawCardData.OpeningHandWinRate),
                                              SeventeenLandsCardData.NumberOfGamesDrawnTurn1OrLaterMetricDesc.NewMetric(rawCardData.DrawnGameCount),
                                              SeventeenLandsCardData.WinRateWhenDrawnTurn1OrLaterMetricDesc.NewMetric(rawCardData.DrawnWinRate),
                                              SeventeenLandsCardData.NumberOfGamesInHandMetricDesc.NewMetric(rawCardData.EverDrawnGameCount),
                                              SeventeenLandsCardData.WinRateInHandMetricDesc.NewMetric(rawCardData.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.None].Mean)),
                                              SeventeenLandsCardData.ColorsWinRateInHandMetricDesc.NewMetric(
                                                  SeventeenLandsCardData.WUWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.WU].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.WU].Mean)),
                                                  SeventeenLandsCardData.WBWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.WB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.WB].Mean)),
                                                  SeventeenLandsCardData.WRWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.WR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.WR].Mean)),
                                                  SeventeenLandsCardData.WGWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.WG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.WG].Mean)),
                                                  SeventeenLandsCardData.UBWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.UB].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.UB].Mean)),
                                                  SeventeenLandsCardData.URWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.UR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.UR].Mean)),
                                                  SeventeenLandsCardData.UGWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.UG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.UG].Mean)),
                                                  SeventeenLandsCardData.BRWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.BR].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.BR].Mean)),
                                                  SeventeenLandsCardData.BGWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.BG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.BG].Mean)),
                                                  SeventeenLandsCardData.RGWinRateInHandMetricDesc.NewMetric(archetypeCardData[ColorPair.RG].GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(CARD_GIH_WR_CRITICAL_THRESHOLD, CARD_GIH_WR_HIGH_THRESHOLD, CARD_GIH_WR_REGULAR_THRESHOLD, (float value) => value - (float)archetypeGihWrDistribution[ColorPair.RG].Mean))
                                              ),
                                              SeventeenLandsCardData.NumberOfGamesNotSeenMetricDesc.NewMetric(rawCardData.NeverDrawnGameCount),
                                              SeventeenLandsCardData.WinRateNotSeenMetricDesc.NewMetric(rawCardData.NeverDrawnWinRate),
                                              SeventeenLandsCardData.WinRateImprovementWhenDrawnMetricDesc.NewMetric(rawCardData.DrawnImprovementWinRate));
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
            return new SeventeenLandsCardData(cardNameFromArena, cardNumber, colors, url);
        }

        return null;
    }
    
    public List<ICardDataSourceStatistic> Statistics { get; init; }

    private const int CARD_EVER_DRAWN_CUTOFF = 500;
    private const float CARD_GIH_WR_CRITICAL_THRESHOLD = 0.05f;
    private const float CARD_GIH_WR_HIGH_THRESHOLD = 0.015f;
    private const float CARD_GIH_WR_REGULAR_THRESHOLD = -0.2f;

    private float Mean(SeventeenLandsRawCardDataSource source, Func<SeventeenLandsRawCardData, float?> selector)
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

    private float Variance(SeventeenLandsRawCardDataSource source, float mean, Func<SeventeenLandsRawCardData, float?> selector)
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

    private Dictionary<ColorPair, SeventeenLandsRawCardDataSource> archetypeCardData = [];
    private Dictionary<ColorPair, Normal> archetypeGihWrDistribution = [];
}
