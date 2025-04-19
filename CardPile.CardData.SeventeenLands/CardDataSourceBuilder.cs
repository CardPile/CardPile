using CardPile.CardData.Parameters;
using CardPile.CardData.Settings;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

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
        SeventeenLandsCardDataSourceProvider.ClearOldData();
        SeventeenLandsCardDataSourceProvider.LoadFilters();
    }

    public static void Init()
    {
        // NOOP - runs static constructor
    }

    public CardDataSourceBuilder()
    {
        setParameter = new(SET_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.SetList);
        eventTypeParameter = new(EVENT_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.EventTypeList);
        userTypeParameter = new(USER_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.UserTypeList);
        colorParameter = new(COLOR_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.ColorList);
        deckTypeParameter = new(DECK_TYPE_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.GetDeckTypeList());
        rarityParameter = new(RARITY_PARAMETER_NAME, SeventeenLandsCardDataSourceProvider.GetRarityList());
        startDateParameter = new(START_DATE_PARAMETER_NAME, GetCurrentSetStartDate());
        endDateParameter = new(END_DATE_PARAMETER_NAME, DateTime.Now);

        currentSetStartDateOffsetInDaysSetting = new("Current set start date offset (days)", Configuration.Instance.CurrentSetStartDateOffsetInDays, 0);
        winRateColorsSetting = new("Deck win rate color combinations", ColorsCombinationNamesToOptions(Configuration.Instance.WinRateColorsToShow));
        winRateParticipationCutoffSetting = new("Deck mategame participation cutoff (%)", Configuration.Instance.WinRateParticipationCutoff, 0.0m, 100.0m);
        rankColorsSetting = new("Rank color combinations", ColorsCombinationNamesToOptions(Configuration.Instance.RankColorsToShow));
        maxDisplayedRankSetting = new("Max rank to show", Configuration.Instance.MaxRankToShow, 0);

        deqDampingSampleSetting = new("DEq dampling sample", Configuration.Instance.DEqDampingSample, 0);
        deqAtaBetaSetting = new("DEq ATA Beta", Configuration.Instance.DEqAtaBeta, -1.0m, 1.0m, 0.05m);  // TODO
        deqP1P1ValueSetting = new("DEq P1P1 Value", Configuration.Instance.DEqP1P1Value, 0.0m, 1.0m, 0.05m);  // TODO
        deqArchetypeDecaySetting = new("DEq Archetype decay", Configuration.Instance.DEqArchetypeDecay, 0.0m, 1.0m, 0.05m);
        deqLossFactorSetting = new("DEq Loss factor", Configuration.Instance.DEqLossFactor, 0.0m, 0.6m, 0.05m);
        deqSampleDecaySetting = new("DEq Sample decay", Configuration.Instance.DEqSampleDecay, 0.0m, 1.0m, 0.05m);
        deqFutureProjectionDaysSetting = new("DEq future projection days", Configuration.Instance.DEqFutureProjectionDays, 0);
        deqColorsSetting = new("DEq color combination", ColorsCombinationNamesToOptions(Configuration.Instance.DEqColors));

        Settings =
        [
            currentSetStartDateOffsetInDaysSetting,
            winRateColorsSetting,
            winRateParticipationCutoffSetting,
            rankColorsSetting,
            maxDisplayedRankSetting,
            deqDampingSampleSetting,
            deqAtaBetaSetting,
            deqP1P1ValueSetting,
            deqArchetypeDecaySetting,
            deqLossFactorSetting,
            deqSampleDecaySetting,
            deqFutureProjectionDaysSetting,
            deqColorsSetting,
        ];

        Parameters =
        [
            setParameter,
            eventTypeParameter,
            userTypeParameter,
            // colorParameter,  // TODO: Client side filtering
            deckTypeParameter,
            // rarityParameter,  // TODO: Client side filtering
            startDateParameter,
            endDateParameter,
        ];

        setParameter.PropertyChanged += OnSetParameterPropertyChanged;
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
        await SaveConfiguration();

        var rankColors = GetRankColors();

        var cardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, deckTypeParameter.Value, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);

        var wuCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WU_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wbCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WB_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wrCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WR_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var ubCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UB_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var urCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UR_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var ugCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var brCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.BR_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var bgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.BG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var rgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.RG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);

        var wubCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WUB_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wurCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WUR_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wugCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WUG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wbrCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WBR_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wbgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WBG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var wrgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.WRG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var ubrCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UBR_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var ubgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.UBG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var urgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.URG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);
        var brgCardDataSource = new RawCardDataSource(await SeventeenLandsCardDataSourceProvider.LoadCardDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, userTypeParameter.Value, SeventeenLandsCardDataSourceProvider.BRG_COLORS_DECK_TYPE, startDateParameter.Value, endDateParameter.Value), startDateParameter.Value, endDateParameter.Value, rankColors, maxDisplayedRankSetting.Value);

        var winData = new WinDataSource(await SeventeenLandsCardDataSourceProvider.LoadWinDataAsync(cancelation, setParameter.Value, eventTypeParameter.Value, startDateParameter.Value, endDateParameter.Value, true), (float)winRateParticipationCutoffSetting.Value);

        var winRateColors = OptionsToColors(winRateColorsSetting.Options);
        var deqCalculator = GetDEqCalculator(cardDataSource);

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
                                  deqCalculator);
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

        Configuration.Instance.CurrentSetStartDateOffsetInDays = currentSetStartDateOffsetInDaysSetting.Value;
        Configuration.Instance.WinRateColorsToShow = winRateColorsToShow;
        Configuration.Instance.WinRateParticipationCutoff = winRateParticipationCutoffSetting.Value;
        Configuration.Instance.RankColorsToShow = rankColorsToShow;
        Configuration.Instance.MaxRankToShow = maxDisplayedRankSetting.Value;

        List<string> deqColorsToShow = [];
        foreach (var option in deqColorsSetting.Options)
        {
            if (option.Value)
            {
                deqColorsToShow.Add(option.Name);
            }
        }

        Configuration.Instance.DEqDampingSample = deqDampingSampleSetting.Value;
        Configuration.Instance.DEqAtaBeta = deqAtaBetaSetting.Value;
        Configuration.Instance.DEqP1P1Value = deqP1P1ValueSetting.Value;
        Configuration.Instance.DEqArchetypeDecay = deqArchetypeDecaySetting.Value;
        Configuration.Instance.DEqLossFactor = deqLossFactorSetting.Value;
        Configuration.Instance.DEqSampleDecay = deqSampleDecaySetting.Value;
        Configuration.Instance.DEqFutureProjectionDays = deqFutureProjectionDaysSetting.Value;
        Configuration.Instance.DEqColors = deqColorsToShow;

        await Configuration.Instance.Save();
    }

    private void OnSetParameterPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (setParameter.Value == SeventeenLandsCardDataSourceProvider.SetList.First())
        {
            startDateParameter.Value = GetCurrentSetStartDate();
            endDateParameter.Value = DateTime.Now;
        }
        else
        {
            startDateParameter.Value = SeventeenLandsCardDataSourceProvider.StartDates.First(x => x.Key == setParameter.Value).Value;
            endDateParameter.Value = DateTime.Now;
        }
    }

    private DateTime GetCurrentSetStartDate()
    {
        var setStartDate = SeventeenLandsCardDataSourceProvider.StartDates.First(x => x.Key == setParameter.Value).Value;
        var adjustedStartDate = DateTime.Now.AddDays(-Configuration.Instance.CurrentSetStartDateOffsetInDays);
        if (adjustedStartDate < setStartDate)
        {
            adjustedStartDate = setStartDate;
        }
        return adjustedStartDate;
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

    private DEqCalculator GetDEqCalculator(RawCardDataSource cardDataSource)
    {
        return new DEqCalculator(cardDataSource,
                                 OptionsToColors(deqColorsSetting.Options),
                                 deqDampingSampleSetting.Value,
                                 (double)deqAtaBetaSetting.Value,
                                 (double)deqP1P1ValueSetting.Value,
                                 (double)deqArchetypeDecaySetting.Value,
                                 (double)deqLossFactorSetting.Value,
                                 (double)deqSampleDecaySetting.Value,
                                 deqFutureProjectionDaysSetting.Value);
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
    private const string DECK_TYPE_PARAMETER_NAME = "CardPile.Deck type";
    private const string RARITY_PARAMETER_NAME = "Rarity";
    private const string START_DATE_PARAMETER_NAME = "Start date";
    private const string END_DATE_PARAMETER_NAME = "End date";

    private readonly ParameterOptions setParameter;
    private readonly ParameterOptions eventTypeParameter;
    private readonly ParameterOptions userTypeParameter;
    private readonly ParameterOptions colorParameter;
    private readonly ParameterOptions deckTypeParameter;
    private readonly ParameterOptions rarityParameter;
    private readonly ParameterDate startDateParameter;
    private readonly ParameterDate endDateParameter;

    private readonly SettingNumber currentSetStartDateOffsetInDaysSetting;
    private readonly SettingMultipleOptions winRateColorsSetting;
    private readonly SettingDecimal winRateParticipationCutoffSetting;
    private readonly SettingMultipleOptions rankColorsSetting;
    private readonly SettingNumber maxDisplayedRankSetting;

    private readonly SettingNumber deqDampingSampleSetting;
    private readonly SettingDecimal deqAtaBetaSetting;
    private readonly SettingDecimal deqP1P1ValueSetting;
    private readonly SettingDecimal deqArchetypeDecaySetting;
    private readonly SettingDecimal deqLossFactorSetting;
    private readonly SettingDecimal deqSampleDecaySetting;
    private readonly SettingNumber deqFutureProjectionDaysSetting;
    private readonly SettingMultipleOptions deqColorsSetting;
}
