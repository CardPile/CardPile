using CardPile.CardData.Importance;
using CardPile.Draft;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        this.cardData = cardData;
        this.wuCardData = wuCardData;
        this.wbCardData = wbCardData;
        this.wrCardData = wrCardData;
        this.wgCardData = wgCardData;
        this.ubCardData = ubCardData;
        this.urCardData = urCardData;
        this.ugCardData = ugCardData;
        this.brCardData = brCardData;
        this.bgCardData = bgCardData;
        this.rgCardData = rgCardData;

        var gihWrMean = Mean(cardData, (SeventeenLandsRawCardData data) => (data.EverDrawnGameCount ?? 0) > 100 ? data.EverDrawnWinRate : null);
        var gihWrStdDev = (float)Math.Sqrt(Variance(cardData, gihWrMean, (SeventeenLandsRawCardData data) => (data.EverDrawnGameCount ?? 0 ) > 100 ? data.EverDrawnWinRate : null));
        gihWrDistribution = new Normal(gihWrMean, gihWrStdDev);
    }

    public string Name => "17Lands";

    public ICardData? GetDataForCard(int cardNumber, DraftState state)
    {
        SeventeenLandsRawCardData? rawCardData = cardData.GetDataForCard(cardNumber);
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
                                              SeventeenLandsCardData.WinRateInHandMetricDesc.NewMetric(rawCardData.EverDrawnWinRate, ImportanceCalculators.AboveThreshold(0.9f, 0.68f, 0.57f, (float value) => (float)gihWrDistribution.CumulativeDistribution(value))),
                                              SeventeenLandsCardData.ColorsWinRateInHandMetricDesc.NewMetric(
                                                  SeventeenLandsCardData.WUWinRateInHandMetricDesc.NewMetric(wuCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.WBWinRateInHandMetricDesc.NewMetric(wbCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.WRWinRateInHandMetricDesc.NewMetric(wrCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.WGWinRateInHandMetricDesc.NewMetric(wgCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.UBWinRateInHandMetricDesc.NewMetric(ubCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.URWinRateInHandMetricDesc.NewMetric(urCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.UGWinRateInHandMetricDesc.NewMetric(ugCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.BRWinRateInHandMetricDesc.NewMetric(brCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.BGWinRateInHandMetricDesc.NewMetric(bgCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate),
                                                  SeventeenLandsCardData.RGWinRateInHandMetricDesc.NewMetric(rgCardData.GetDataForCard(rawCardData.ArenaCardId)?.EverDrawnWinRate)
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

    SeventeenLandsRawCardDataSource cardData;
    SeventeenLandsRawCardDataSource wuCardData;
    SeventeenLandsRawCardDataSource wbCardData;
    SeventeenLandsRawCardDataSource wrCardData;
    SeventeenLandsRawCardDataSource wgCardData;
    SeventeenLandsRawCardDataSource ubCardData;
    SeventeenLandsRawCardDataSource urCardData;
    SeventeenLandsRawCardDataSource ugCardData;
    SeventeenLandsRawCardDataSource brCardData;
    SeventeenLandsRawCardDataSource bgCardData;
    SeventeenLandsRawCardDataSource rgCardData;

    Normal gihWrDistribution;
}
