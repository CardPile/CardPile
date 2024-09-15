namespace CardPile.CardData.Dummy;

public class DummyCardDataSource : ICardDataSource
{
    public string Name => "Dummy";

    public ICardData? GetDataForCard(int cardNumber)
    {
        if (firstCardNumber == null)
        {
            firstCardNumber = cardNumber;
        }
        else if (secondCardNumber == null)
        {
            secondCardNumber = cardNumber;
        }
        else if (thirdCardNumber == null)
        {
            thirdCardNumber = cardNumber;
        }

        if (cardNumber == firstCardNumber)
        {
            var metricA = DummyCardDataSourceBuilder.MetricADesc.NewMetric<float>(75.1f);
            var metricB = DummyCardDataSourceBuilder.MetricBDesc.NewMetric<int>(4);
            var metricC = DummyCardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.16f);
            var metricD = DummyCardDataSourceBuilder.MetricDDesc.NewMetric("SB A+");
            var metricE = DummyCardDataSourceBuilder.MetricEDesc.NewMetricWithSort(metricB, metricA, metricC, metricD);
            return new DummyCardData("Card A",
                                     cardNumber,
                                     [Color.White],
                                     null,
                                     metricA,
                                     metricB,
                                     metricC,
                                     metricD,
                                     metricE);
        }
        else if(cardNumber == secondCardNumber)
        {
            var metricA = DummyCardDataSourceBuilder.MetricADesc.NewMetric<float>(73.1f);
            var metricB = DummyCardDataSourceBuilder.MetricBDesc.NewMetric<int>(5);
            var metricC = DummyCardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.14f);
            var metricD = DummyCardDataSourceBuilder.MetricDDesc.NewMetric("SYN D+");
            var metricE = DummyCardDataSourceBuilder.MetricEDesc.NewMetricWithSort(metricB, metricA, metricC, metricD);
            return new DummyCardData("Card B",
                                     cardNumber,
                                     [Color.Green],
                                     null,
                                     metricA,
                                     metricB,
                                     metricC,
                                     metricD,
                                     metricE);
        }
        else if (cardNumber == thirdCardNumber)
        {
            var metricA = DummyCardDataSourceBuilder.MetricADesc.NewMetric<float>(79.1f);
            var metricB = DummyCardDataSourceBuilder.MetricBDesc.NewMetric<int>(6);
            var metricC = DummyCardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.10f);
            var metricD = DummyCardDataSourceBuilder.MetricDDesc.NewMetric("BA C+");
            var metricE = DummyCardDataSourceBuilder.MetricEDesc.NewMetricWithSort(metricB, metricA, metricC, metricD);
            return new DummyCardData("Card C",
                                     cardNumber,
                                     [Color.Red],
                                     null,
                                     metricA,
                                     metricB,
                                     metricC,
                                     metricD,
                                     metricE);
        }
        else
        {
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
                return new DummyCardData(cardNameFromArena, cardNumber, colors, url, null, null, null, null, null);
            }

            return null;
        }
    }

    int? firstCardNumber = null;
    int? secondCardNumber = null;
    int? thirdCardNumber = null;
}
