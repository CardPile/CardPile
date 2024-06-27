using CardPile.Draft;
using CardPile.Parser.Matchers;
using NGuid;

namespace CardPile.Parser;

public class DraftPickMatcher : ILogMatcher
{
    public event EventHandler<DraftPickEvent>? DraftPickEvent;

    public bool Match(string line)
    {
        DraftPickEvent? eventData = null;
        if (line.StartsWith(PREMIERE_DRAFT_PICK_PREFIX_NEEDLE) && line.Contains(PREMIERE_DRAFT_PICK_DRAFT_ID_NEEDLE))
        {
            eventData = ParsePremiereDraftEventInfo(line, PREMIERE_DRAFT_PICK_PREFIX_NEEDLE);
        }
        else if (line.StartsWith(QUICK_DRAFT_PICK_PREFIX_NEEDLE))
        {
            eventData = ParseQuickDraftEventInfo(line, QUICK_DRAFT_PICK_PREFIX_NEEDLE);
        }

        if (eventData != null)
        {
            DraftPickEvent?.Invoke(this, eventData);
            return true;
        }

        return false;
    }

    private static DraftPickEvent? ParsePremiereDraftEventInfo(string line, string needle)
    {
        dynamic? requestData = MatcherHelpers.ParseRequestUnchecked(line, needle);

        var draftId = requestData?.DraftId?.Value;
        var packNumber = requestData?.Pack?.Value;
        var pickNumber = requestData?.Pick?.Value;
        var cardPicked = requestData?.GrpId?.Value;

        if (draftId == null || packNumber == null || pickNumber == null || cardPicked == null ) 
        { 
            return null; 
        }

        return new DraftPickEvent(new Guid(draftId), (int)packNumber, (int)pickNumber, (int)cardPicked);
    }

    private static DraftPickEvent? ParseQuickDraftEventInfo(string line, string needle)
    {
        dynamic? requestData = MatcherHelpers.ParseRequestUnchecked(line, needle);

        var eventName = requestData?.EventName?.Value;
        var packNumber = requestData?.PickInfo?.PackNumber?.Value;
        var pickNumber = requestData?.PickInfo?.PickNumber?.Value;
        var cardPickedString = requestData?.PickInfo?.CardId?.Value;

        if (eventName == null || packNumber == null || pickNumber == null || cardPickedString == null)
        {
            return null;
        }

        if( !int.TryParse(cardPickedString, out int cardPicked))
        {
            return null;
        }

        // Generate a name-based guid using the event name since we don't have an actual guid here
        var draftId = GuidHelpers.CreateFromName(MatcherHelpers.CARD_PILE_DRAFT_NAMESPACE_GUID, eventName);

        return new DraftPickEvent(draftId, (int)packNumber + 1, (int)pickNumber + 1, cardPicked);
    }

    private static readonly string PREMIERE_DRAFT_PICK_PREFIX_NEEDLE = "[UnityCrossThreadLogger]==> Event_PlayerDraftMakePick";
    private static readonly string PREMIERE_DRAFT_PICK_DRAFT_ID_NEEDLE = "DraftId";
    private static readonly string QUICK_DRAFT_PICK_PREFIX_NEEDLE = "[UnityCrossThreadLogger]==> BotDraft_DraftPick";
}
