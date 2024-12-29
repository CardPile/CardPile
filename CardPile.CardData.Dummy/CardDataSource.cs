using CardPile.CardData.Formatting;
using CardPile.CardData.Metrics;
using CardPile.Draft;

namespace CardPile.CardData.Dummy;

public class CardDataSource : ICardDataSource
{
    public CardDataSource()
    {
        firstStatistic = new Statistic<float>("Stat 1", 7.5f, new PercentFormatter());
        secondStatistic = new Statistic<int>("Stat 2", 3);
    }

    public string Name => "Dummy";

    public ICardData? GetDataForCard(int cardNumber, DraftState state)
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
            var metricA = CardDataSourceBuilder.MetricADesc.NewMetric<float>(75.1f);
            var metricB = CardDataSourceBuilder.MetricBDesc.NewMetric<int>(4);
            var metricC = CardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.16f);
            var metricD = CardDataSourceBuilder.MetricDDesc.NewMetric("SB A+");
            var metricE = CardDataSourceBuilder.MetricEDesc.NewMetricWithSort(metricB, metricA, metricC, metricD);
            return new CardData("Card A",
                                     cardNumber,
                                     Type.Artifact,
                                     7,
                                     Color.White,
                                     null,
                                     metricA,
                                     metricB,
                                     metricC,
                                     metricD,
                                     metricE);
        }
        else if(cardNumber == secondCardNumber)
        {
            var metricA = CardDataSourceBuilder.MetricADesc.NewMetric<float>(73.1f);
            var metricB = CardDataSourceBuilder.MetricBDesc.NewMetric<int>(5);
            var metricC = CardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.14f);
            var metricD = CardDataSourceBuilder.MetricDDesc.NewMetric("SYN D+");
            var metricE = CardDataSourceBuilder.MetricEDesc.NewMetricWithSort(metricB, metricA, metricC, metricD);
            return new CardData("Card B",
                                     cardNumber,
                                     Type.Creature,
                                     33,
                                     Color.Green,
                                     null,
                                     metricA,
                                     metricB,
                                     metricC,
                                     metricD,
                                     metricE);
        }
        else if (cardNumber == thirdCardNumber)
        {
            var metricA = CardDataSourceBuilder.MetricADesc.NewMetric<float>(79.1f);
            var metricB = CardDataSourceBuilder.MetricBDesc.NewMetric<int>(6);
            var metricC = CardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.10f);
            var metricD = CardDataSourceBuilder.MetricDDesc.NewMetric("BA C+");
            var metricE = CardDataSourceBuilder.MetricEDesc.NewMetricWithSort(metricB, metricA, metricC, metricD);
            return new CardData("Card C",
                                     cardNumber,
                                     Type.Dungeon,
                                     1,
                                     Color.Red,
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

                var type = CardInfo.Arena.GetCardTypeFromId(cardNumber);
                var manaValue = CardInfo.Arena.GetCardManaValueFromId(cardNumber);
                var colors = CardInfo.Arena.GetCardColorsFromId(cardNumber);
                return new CardData(cardNameFromArena, cardNumber, type, manaValue, colors, url, null, null, null, null, null);
            }

            return null;
        }
    }

    public List<ICardDataSourceStatistic> Statistics { get => [firstStatistic, secondStatistic]; }

    int? firstCardNumber = null;
    int? secondCardNumber = null;
    int? thirdCardNumber = null;

    Statistic<float> firstStatistic;
    Statistic<int> secondStatistic;
}
