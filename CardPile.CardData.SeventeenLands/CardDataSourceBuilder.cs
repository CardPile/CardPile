using CardPile.CardData.Parameters;

namespace CardPile.CardData.SeventeenLands;

public class CardDataSourceBuilder : ICardDataSourceBuilder
{
    static CardDataSourceBuilder()
    {
        SeventeenLandsCardDataSourceProvider.ClearOldData();
        SeventeenLandsCardDataSourceProvider.LoadFilters();
    }

    public static void Init()
    {
        // NOOP - runs static constructor
    }

    public CardDataSourceBuilder()
    {
        Settings = [];
        Parameters =
        [
            SetParameter,
            EventTypeParameter,
            UserTypeParameter,
            // ColorParameter,  // TODO: Client side filtering
            DeckTypeParameter,
            // RarityParameter,  // TODO: Client side filtering
            StartDateParameter,
            EndDateParameter,
        ];
    }

    public string Name => "17Lands";

    public List<ICardDataSourceSetting> Settings { get; init; }

    public List<ICardDataSourceParameter> Parameters { get; init; }

    public List<ICardMetricDescription> MetricDescriptions { get => CardData.MetricDescriptions; }

    public ICardDataSource Build()
    {
        return Task.Run(() => BuildAsync(CancellationToken.None)).Result;
    }

    public async Task<ICardDataSource> BuildAsync(CancellationToken cancelation)
    {
        var cardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, DeckTypeParameter.Value, StartDateParameter.Value, EndDateParameter.Value));

        var wuCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WU_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var wbCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WB_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var wrCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WR_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var wgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WG_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var ubCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UB_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var urCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UR_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var ugCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UG_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var brCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.BR_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var bgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.BG_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));
        var rgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, UserTypeParameter.Value, SeventeenLandsCardDataSourceProvider.RG_COLORS_DECK_TYPE, StartDateParameter.Value, EndDateParameter.Value));

        var winData = new WinDataSource(await SeventeenLandsCardDataSourceProvider.LoadWinDataAsync(cancelation, SetParameter.Value, EventTypeParameter.Value, StartDateParameter.Value, EndDateParameter.Value, true));

        return new CardDataSource(cardDataSource,
                                  wuCardDataSource,
                                  wbCardDataSource,
                                  wrCardDataSource,
                                  wgCardDataSource,
                                  ubCardDataSource,
                                  urCardDataSource,
                                  ugCardDataSource,
                                  brCardDataSource,
                                  bgCardDataSource,
                                  rgCardDataSource,
                                  winData);
    }

    private const string SET_PARAMETER_NAME = "Set";
    private const string EVENT_TYPE_PARAMETER_NAME = "Event type";
    private const string USER_TYPE_PARAMETER_NAME = "User type";
    private const string COLOR_PARAMETER_NAME = "Color";
    private const string DECK_TYPE_PARAMETER_NAME = "Deck type";
    private const string RARITY_PARAMETER_NAME = "Rarity";
    private const string START_DATE_PARAMETER_NAME = "Start date";
    private const string END_DATE_PARAMETER_NAME = "End date";

    private const int StartDataOffsetDays = 21;

    private readonly ParameterOptions SetParameter = new(SET_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.SetList);
    private readonly ParameterOptions EventTypeParameter = new(EVENT_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.EventTypeList);
    private readonly ParameterOptions UserTypeParameter = new(USER_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.UserTypeList);
    private readonly ParameterOptions ColorParameter = new(COLOR_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.ColorList);
    private readonly ParameterOptions DeckTypeParameter = new(DECK_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.GetDeckTypeList());
    private readonly ParameterOptions RarityParameter = new(RARITY_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.GetRarityList());
    private readonly ParameterDate StartDateParameter = new(START_DATE_PARAMETER_NAME, DateTime.Now.AddDays(-StartDataOffsetDays));
    private readonly ParameterDate EndDateParameter = new(END_DATE_PARAMETER_NAME, DateTime.Now);
}
