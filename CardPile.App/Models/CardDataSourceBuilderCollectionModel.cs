using CardPile.App.Services;
using CardPile.CardData.Dummy;
using CardPile.CardData.SeventeenLands;
using CardPile.CardData.Spreadsheet;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace CardPile.App.Models;

internal class CardDataSourceBuilderCollectionModel : ReactiveObject, ICardDataSourceBuilderCollectionService
{
    static CardDataSourceBuilderCollectionModel()
    {
        DummyCardDataSourceBuilder.Init();
        SeventeenLandsCardDataSourceBuilder.Init();
        SpreadsheetCardDataSourceBuilder.Init();
    }

    internal CardDataSourceBuilderCollectionModel()
    {
        currentCardDataSourceBuilder = AvailableCardDataSourceBuilders.First();
    }

    // public List<ICardDataSourceBuilderService> AvailableCardDataSourceBuilders { get; } = [DummyCardDataSourceBuilderModel, SeventeenLandsCardDataSourceBuilderModel, SpreadsheetCardDataSourceBuilderModel];
    public List<ICardDataSourceBuilderService> AvailableCardDataSourceBuilders { get; } = [SeventeenLandsCardDataSourceBuilderModel, SpreadsheetCardDataSourceBuilderModel];

    public ICardDataSourceBuilderService CurrentCardDataSourceBuilder
    {
        get => currentCardDataSourceBuilder;
        set => this.RaiseAndSetIfChanged(ref currentCardDataSourceBuilder, value);
    }

    internal static void Init()
    {
        // NOOP - runs static constructor
    }

    private static readonly CardDataSourceBuilderModel DummyCardDataSourceBuilderModel = new CardDataSourceBuilderModel(new DummyCardDataSourceBuilder());
    private static readonly CardDataSourceBuilderModel SeventeenLandsCardDataSourceBuilderModel = new CardDataSourceBuilderModel(new SeventeenLandsCardDataSourceBuilder());
    private static readonly CardDataSourceBuilderModel SpreadsheetCardDataSourceBuilderModel = new CardDataSourceBuilderModel(new SpreadsheetCardDataSourceBuilder());

    private ICardDataSourceBuilderService currentCardDataSourceBuilder;
}
