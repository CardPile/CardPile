﻿namespace CardPile.CardData.Dummy;

public class DummyCardDataSource : ICardDataSource
{
    public string Name => "Dummy";

    public ICardData? GetDataForCard(int cardNumber)
    {
        if(cardNumber == 90752)
        {
            return new DummyCardData("Card A", 90752, null, DummyCardDataSourceBuilder.MetricADesc.NewMetric<float>(75.1f), DummyCardDataSourceBuilder.MetricBDesc.NewMetric<int>(4), DummyCardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.16f));
        }
        else if(cardNumber == 90629)
        {
            return new DummyCardData("Card B", 90629, null, DummyCardDataSourceBuilder.MetricADesc.NewMetric<float>(73.1f), DummyCardDataSourceBuilder.MetricBDesc.NewMetric<int>(5), DummyCardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.14f));
        }
        else if (cardNumber == 7)
        {
            return new DummyCardData("Card C", cardNumber, null, DummyCardDataSourceBuilder.MetricADesc.NewMetric<float>(79.1f), DummyCardDataSourceBuilder.MetricBDesc.NewMetric<int>(6), DummyCardDataSourceBuilder.MetricCDesc.NewMetric<float>(0.10f));
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
                return new DummyCardData(cardNameFromArena, cardNumber, url, null, null, null);
            }

            return null;
        }
    }
}
