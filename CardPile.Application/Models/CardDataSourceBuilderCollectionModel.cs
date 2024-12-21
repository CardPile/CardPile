using CardPile.Application.Services;
using CardPile.CardData.Dummy;
using CardPile.CardData.SeventeenLands;
using CardPile.CardData.Spreadsheet;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace CardPile.Application.Models;

internal class CardDataSourceBuilderCollectionModel : ReactiveObject, ICardDataSourceBuilderCollectionService
{
    static CardDataSourceBuilderCollectionModel()
    {
        CardData.Dummy.CardDataSourceBuilder.Init();
        CardData.SeventeenLands.CardDataSourceBuilder.Init();
        CardData.Spreadsheet.CardDataSourceBuilder.Init();
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

    private static readonly CardDataSourceBuilderModel DummyCardDataSourceBuilderModel = new CardDataSourceBuilderModel(new CardData.Dummy.CardDataSourceBuilder());
    private static readonly CardDataSourceBuilderModel SeventeenLandsCardDataSourceBuilderModel = new CardDataSourceBuilderModel(new CardData.SeventeenLands.CardDataSourceBuilder());
    private static readonly CardDataSourceBuilderModel SpreadsheetCardDataSourceBuilderModel = new CardDataSourceBuilderModel(new CardData.Spreadsheet.CardDataSourceBuilder());

    private ICardDataSourceBuilderService currentCardDataSourceBuilder;
}
