using CardPile.App.Services;
using CardPile.CardData.Dummy;
using CardPile.CardData.SeventeenLands;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace CardPile.App.Models;

internal class CardDataSourceBuilderCollectionModel : ReactiveObject, ICardDataSourceBuilderCollectionService
{
    internal CardDataSourceBuilderCollectionModel()
    {
        currentCardDataSourceBuilder = AvailableCardDataSourceBuilders.First();
    }

    // public List<ICardDataSourceBuilderService> AvailableCardDataSourceBuilders { get; } = [DummyCardDataSourceBuilderModel, SeventeenLandsCardDataSourceBuilderModel];
    public List<ICardDataSourceBuilderService> AvailableCardDataSourceBuilders { get; } = [SeventeenLandsCardDataSourceBuilderModel];

    public ICardDataSourceBuilderService CurrentCardDataSourceBuilder
    {
        get => currentCardDataSourceBuilder;
        set => this.RaiseAndSetIfChanged(ref currentCardDataSourceBuilder, value);
    }

    private static readonly CardDataSourceBuilderModel DummyCardDataSourceBuilderModel = new CardDataSourceBuilderModel(new DummyCardDataSourceBuilder());
    private static readonly CardDataSourceBuilderModel SeventeenLandsCardDataSourceBuilderModel = new CardDataSourceBuilderModel(new SeventeenLandsCardDataSourceBuilder());

    private ICardDataSourceBuilderService currentCardDataSourceBuilder;
}
