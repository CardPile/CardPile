using ExCSS;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json.Linq;
using System;

namespace CardPile.CardData.SeventeenLands;

internal class DEqCalculator
{
    internal DEqCalculator(RawCardDataSource cardDataSource,
                           List<Color> colorCombinations,
                           int dampingSample,
                           double ataBeta,
                           double p1p1Value,
                           double archetypeDecay,
                           double deqLossFactor,
                           double sampleDecay,
                           int futureProjectionDays)
    {
        var daysOfData = (int)(cardDataSource.EndDate - cardDataSource.StartDate).TotalDays;

        var metaRegressionFactor = deqLossFactor * (Math.Pow(archetypeDecay, futureProjectionDays + daysOfData) * (1.0 - Math.Pow(sampleDecay, daysOfData)) * (1.0 - archetypeDecay * sampleDecay) / (1.0 - sampleDecay) / (1.0 - Math.Pow(archetypeDecay * sampleDecay, daysOfData)) - 1.0);

        // Step 1 - win rate numerators and denominators
        Dictionary<int, (Color, double)> wrDenoms = [];
        Dictionary<int, (Color, double)> wrNums = [];

        foreach (var cardDataEntry in cardDataSource)
        {
            var cardNumber = cardDataEntry.Key;
            var cardData = cardDataEntry.Value;

            if (!cardData.PickCount.HasValue || !cardData.PlayRate.HasValue || !cardData.WinRate.HasValue)
            {
                continue;
            }

            var numPicked = cardData.PickCount.Value;
            var percentGP = (double)cardData.PlayRate.Value;
            var gpWr = (double)cardData.WinRate.Value;

            var wrNum = numPicked * percentGP * gpWr;
            var wrDenom = numPicked * percentGP;

            wrDenoms[cardNumber] = (cardData.Colors, wrDenom);
            wrNums[cardNumber] = (cardData.Colors, wrNum);
        }

        // Step 2 - win rate and marginal color combination win rates
        var averageWinRate = wrNums.Values.Select((v, k) => v.Item2).Sum() / wrDenoms.Values.Select((v, k) => v.Item2).Sum();

        Dictionary<Color, double> marginalColorCombinationWinRates = [];
        foreach (var colorCombination in colorCombinations)
        {
            var wrNumSum = wrNums.Values.Where((v, k) => v.Item1 == colorCombination).Select((v, k) => v.Item2).Sum();
            var wrDenomSum = wrDenoms.Values.Where((v, k) => v.Item1 == colorCombination).Select((v, k) => v.Item2).Sum();
            var marginalColorCombinationWinRate = wrNumSum / wrDenomSum - averageWinRate;
            if (Double.IsNaN(marginalColorCombinationWinRate))
            { 
                continue; 
            }

            marginalColorCombinationWinRates[colorCombination] = marginalColorCombinationWinRate;
        }

        // Step 3 - per card DEq
        foreach (var cardDataEntry in cardDataSource)
        {
            var cardNumber = cardDataEntry.Key;
            var cardData = cardDataEntry.Value;
            if(!cardData.AvgPick.HasValue || !cardData.PlayRate.HasValue)
            {
                continue;
            }

            if (!wrNums.TryGetValue(cardNumber, out var wrNumEntry) || !wrDenoms.TryGetValue(cardNumber, out var wrDenomEntry))
            {
                continue;
            }

            var percentGP = cardData.PlayRate;
            var ata = (double)cardData.AvgPick.Value;

            var wrNum = wrNumEntry.Item2;
            var wrDenom = wrDenomEntry.Item2;

            var marginalColorCombinationWinRate = marginalColorCombinationWinRates.GetValueOrDefault(cardData.Colors, 0.0);

            var mwr = (wrNum + dampingSample * ataBeta * (ata - 7) - averageWinRate * wrDenom) / (dampingSample + wrDenom);
            var pickEquiryBase = (14 - ata) * (14 - ata) / (13 * 13);
            var pickEquity = p1p1Value * pickEquiryBase;
            var biasAdj = (pickEquiryBase - 1) * marginalColorCombinationWinRate;
            var metaDecay = metaRegressionFactor * (biasAdj + marginalColorCombinationWinRate);

            var playedDeq = mwr + pickEquity + biasAdj + metaDecay;           
            var cardDeq = playedDeq * percentGP;

            deq[cardNumber] = (float)cardDeq;
        }
    }

    internal (float?, List<ICardRank>) GetDEqAndRanks(int cardNumber)
    {
        if(deq.TryGetValue(cardNumber, out var value))
        {
            return (value, []);
        }
        return (null, []);
    }

    internal string? GetDEqGrade(int cardNumber)
    {
        (string, float)[] CUTOFF_TABLE = [
            ("A+", 0.0525f),
            ("A", 0.045f),
            ("A-", 0.0375f),
            ("B+", 0.03f),
            ("B", 0.0225f),
            ("B-", 0.015f),
            ("C+", 0.0075f),
            ("C", 0.0f),
            ("C-", -0.0075f),
            ("D+", -0.015f),
            ("D", -0.0225f),
            ("D-", -0.03f),
            ("F", float.MinValue)
        ];

        if (!deq.TryGetValue(cardNumber, out var value))
        {
            return null;
        }

        foreach(var entry in CUTOFF_TABLE)
        {
            if(value > entry.Item2)
            {
                return entry.Item1;
            }
        }

        return null;
    }


    Dictionary<int, float> deq = [];
}
