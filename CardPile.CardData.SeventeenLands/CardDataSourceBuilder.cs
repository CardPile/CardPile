using CardPile.CardData.Parameters;
using CardPile.CardData.Settings;

namespace CardPile.CardData.SeventeenLands;

public class CardDataSourceBuilder : ICardDataSourceBuilder
{
    public const string COLORLESS_RANK_COLOR_OPTION_NAME = "Colorless";
    public const string ONE_COLOR_RANK_COLOR_OPTION_NAME = "One color";
    public const string TWO_COLOR_RANK_COLOR_OPTION_NAME = "Two colors";
    public const string THREE_COLOR_RANK_COLOR_OPTION_NAME = "Three colors";
    public const string FOUR_COLOR_RANK_COLOR_OPTION_NAME = "Four colors";
    public const string FIVE_COLOR_RANK_COLOR_OPTION_NAME = "Five colors";

    static CardDataSourceBuilder()
    {
        CardDEqProvider.ClearOldData();
        SeventeenLandsCardDataSourceProvider.ClearOldData();
        SeventeenLandsCardDataSourceProvider.LoadFilters();
    }

    public static void Init()
    {
        // NOOP - runs static constructor
    }

    public CardDataSourceBuilder()
    {
        var currentSet = SeventeenLandsCardDataSourceProvider.SetList.First();

        setParameter = new(SET_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.SetList, currentSet);
        eventTypeParameter = new(EVENT_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.FormatsByExpansion[currentSet]);
        userTypeParameter = new(USER_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.UserTypeList);
        colorParameter = new(COLOR_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.GetColorList());
        deckTypeParameter = new(DECK_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.ColorList);
        rarityParameter = new(RARITY_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.GetRarityList());
        timePeriodParameter = new(TIME_PERIOD_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.TimePeriods[currentSet], SeventeenLandsCardDataSourceProvider.LAST_TWO_WEEKS_TIME_PERIOD_TYPE);

        winRateColorsSetting = new("Deck win rate color combinations", ColorsCombinationNamesToOptions(Configuration.Instance.WinRateColorsToShow));
        winRateParticipationCutoffSetting = new("Deck metagame participation cutoff (%)", Configuration.Instance.WinRateParticipationCutoff, 0.0m, 100.0m);
        rankColorsSetting = new("Rank color combinations", ColorsCombinationNamesToOptions(Configuration.Instance.RankColorsToShow));
        maxDisplayedRankSetting = new("Max rank to show", Configuration.Instance.MaxRankToShow, 0);
        
        Settings =
        [
            winRateColorsSetting,
            winRateParticipationCutoffSetting,
            rankColorsSetting,
            maxDisplayedRankSetting
        ];

        Parameters =
        [
            setParameter,
            eventTypeParameter,
            userTypeParameter,
            // colorParameter,  // TODO: Client side filtering
            deckTypeParameter,
            // rarityParameter,  // TODO: Client side filtering
            timePeriodParameter
        ];

        setParameter.PropertyChanged += OnSetParameterPropertyChanged;
    }

    public string Name => "17Lands";

    public List<ICardDataSourceSetting> Settings { get; init; }

    public List<ICardDataSourceParameter> Parameters { get; init; }

    public List<ICardMetricDescription> MetricDescriptions => CardData.MetricDescriptions;

    public ICardDataSource Build()
    {
        return Task.Run(() => BuildAsync(CancellationToken.None)).Result;
    }

    public async Task<ICardDataSource> BuildAsync(CancellationToken cancelation)
    {
        await SaveConfiguration();

        var rankColors = GetRankColors();

        var cardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, deckTypeParameter.Value, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);

        var wuCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WU_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wbCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WB_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wrCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WR_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var ubCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UB_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var urCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UR_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var ugCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var brCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.BR_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var bgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.BG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var rgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.RG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);

        var wubCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WUB_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wurCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WUR_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wugCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WUG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wbrCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WBR_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wbgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WBG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wrgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WRG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var ubrCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UBR_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var ubgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UBG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var urgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.URG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var brgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.BRG_COLORS_DECK_TYPE, timePeriodParameter.Value), timePeriodParameter.Value, rankColors, maxDisplayedRankSetting.Value);


        var (startDate, endDate) = StartDateAndTimePeriodToEffectiveStartAndEndDate(SeventeenLandsCardDataSourceProvider.StartDates[setParameter.Value], timePeriodParameter.Value);
        var winData = new WinDataSource(await SeventeenLandsCardDataSourceProvider.LoadWinDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, startDate, endDate, true), (float)winRateParticipationCutoffSetting.Value);

        var winRateColors = OptionsToColors(winRateColorsSetting.Options);

        var deqSource = new RawCardDEqSource(await CardDEqProvider.LoadDEqDataAsync(cancelation, setParameter.Value));
        
        return new CardDataSource(setParameter.Value,
                                  cardDataSource,
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
                                  wubCardDataSource,
                                  wurCardDataSource,
                                  wugCardDataSource,
                                  wbrCardDataSource,
                                  wbgCardDataSource,
                                  wrgCardDataSource,
                                  ubrCardDataSource,
                                  ubgCardDataSource,
                                  urgCardDataSource,
                                  brgCardDataSource,
                                  winRateColors,
                                  winData,
                                  deqSource);
    }

    private async Task SaveConfiguration()
    {
        List<string> winRateColorsToShow = [];
        foreach (var option in winRateColorsSetting.Options)
        {
            if (option.Value)
            {
                winRateColorsToShow.Add(option.Name);
            }
        }

        List<string> rankColorsToShow = [];
        foreach (var option in rankColorsSetting.Options)
        {
            if (option.Value)
            {
                rankColorsToShow.Add(option.Name);
            }
        }

        Configuration.Instance.WinRateColorsToShow = winRateColorsToShow;
        Configuration.Instance.WinRateParticipationCutoff = winRateParticipationCutoffSetting.Value;
        Configuration.Instance.RankColorsToShow = rankColorsToShow;
        Configuration.Instance.MaxRankToShow = maxDisplayedRankSetting.Value;
        
        await Configuration.Instance.Save();
    }

    private void OnSetParameterPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (SeventeenLandsCardDataSourceProvider.FormatsByExpansion.TryGetValue(setParameter.Value, out var formats))
        {
            eventTypeParameter.Options = formats;
        }
        else
        {
            eventTypeParameter.Options = SeventeenLandsCardDataSourceProvider.FormatsByExpansion[string.Empty];
        }
    }

    private (DateTime, DateTime) StartDateAndTimePeriodToEffectiveStartAndEndDate(DateTime startDate, string timePeriod)
    {
        switch (timePeriod)
        {
            case SeventeenLandsCardDataSourceProvider.ALL_TIME_TIME_PERIOD_TYPE:
                return (startDate, DateTime.Now);
            case SeventeenLandsCardDataSourceProvider.ALL_EXCEPT_FIRST_WEEK_TIME_PERIOD_TYPE:
                return (startDate.AddDays(7), DateTime.Now);
            case SeventeenLandsCardDataSourceProvider.LATEST_EVENT_TIME_PERIOD_TYPE:
                // Hard to know when that started from what we have, so return whole period
                return (startDate, DateTime.Now);
            case SeventeenLandsCardDataSourceProvider.LAST_TWO_WEEKS_TIME_PERIOD_TYPE:
                return (DateTime.Now.AddDays(-14), DateTime.Now);
            case SeventeenLandsCardDataSourceProvider.LAST_WEEK_TIME_PERIOD_TYPE:
                return (DateTime.Now.AddDays(-7), DateTime.Now);
            case SeventeenLandsCardDataSourceProvider.LAST_DAY_TIME_PERIOD_TYPE:
                return (DateTime.Now.AddDays(-1), DateTime.Now);
            case SeventeenLandsCardDataSourceProvider.FIRST_WEEK_TIME_PERIOD_TYPE:
                return (startDate, startDate.AddDays(7));
            default:
                return (startDate, DateTime.Now);
        }
    }

    private List<Color> GetRankColors()
    {
        List<Color> rankColors = [];
        if (rankColorsSetting.Options.First(o => o.Name == COLORLESS_RANK_COLOR_OPTION_NAME).Value) rankColors.Add(Color.None);
        if (rankColorsSetting.Options.First(o => o.Name == ONE_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorSingles());
        if (rankColorsSetting.Options.First(o => o.Name == TWO_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorPairs());
        if (rankColorsSetting.Options.First(o => o.Name == THREE_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorTriples());
        if (rankColorsSetting.Options.First(o => o.Name == FOUR_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorQuadruples());
        if (rankColorsSetting.Options.First(o => o.Name == FIVE_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorQuintuples());

        return rankColors;
    }
    
    private static List<ICardDataSourceSettingOption> ColorsCombinationNamesToOptions(List<string> colorCombinationNames)
    {
        return
        [
            new SettingOption(COLORLESS_RANK_COLOR_OPTION_NAME,   colorCombinationNames.Contains(COLORLESS_RANK_COLOR_OPTION_NAME)),
            new SettingOption(ONE_COLOR_RANK_COLOR_OPTION_NAME,   colorCombinationNames.Contains(ONE_COLOR_RANK_COLOR_OPTION_NAME)),
            new SettingOption(TWO_COLOR_RANK_COLOR_OPTION_NAME,   colorCombinationNames.Contains(TWO_COLOR_RANK_COLOR_OPTION_NAME)),
            new SettingOption(THREE_COLOR_RANK_COLOR_OPTION_NAME, colorCombinationNames.Contains(THREE_COLOR_RANK_COLOR_OPTION_NAME)),
            new SettingOption(FOUR_COLOR_RANK_COLOR_OPTION_NAME,  colorCombinationNames.Contains(FOUR_COLOR_RANK_COLOR_OPTION_NAME)),
            new SettingOption(FIVE_COLOR_RANK_COLOR_OPTION_NAME,  colorCombinationNames.Contains(FIVE_COLOR_RANK_COLOR_OPTION_NAME)),
        ];
    }

    private static List<Color> OptionsToColors(List<ICardDataSourceSettingOption> options)
    {
        List<Color> rankColors = [];
        if (options.First(o => o.Name == COLORLESS_RANK_COLOR_OPTION_NAME).Value) rankColors.Add(Color.None);
        if (options.First(o => o.Name == ONE_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorSingles());
        if (options.First(o => o.Name == TWO_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorPairs());
        if (options.First(o => o.Name == THREE_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorTriples());
        if (options.First(o => o.Name == FOUR_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorQuadruples());
        if (options.First(o => o.Name == FIVE_COLOR_RANK_COLOR_OPTION_NAME).Value) rankColors.AddRange(ColorsUtil.ColorQuintuples());

        return rankColors;
    }

    private const string SET_PARAMETER_NAME = "Set";
    private const string EVENT_TYPE_PARAMETER_NAME = "Event type";
    private const string USER_TYPE_PARAMETER_NAME = "User type";
    private const string COLOR_PARAMETER_NAME = "Color";
    private const string DECK_TYPE_PARAMETER_NAME = "Deck type";
    private const string RARITY_PARAMETER_NAME = "Rarity";
    private const string TIME_PERIOD_PARAMETER_NAME = "Time period";

    private readonly ParameterOptions setParameter;
    private readonly ParameterOptions eventTypeParameter;
    private readonly ParameterOptions userTypeParameter;
    private readonly ParameterOptions colorParameter;
    private readonly ParameterOptions deckTypeParameter;
    private readonly ParameterOptions rarityParameter;
    private readonly ParameterOptions timePeriodParameter;

    private readonly SettingMultipleOptions winRateColorsSetting;
    private readonly SettingDecimal winRateParticipationCutoffSetting;
    private readonly SettingMultipleOptions rankColorsSetting;
    private readonly SettingNumber maxDisplayedRankSetting;
}
