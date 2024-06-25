using CardPile.Draft;
using CardPile.Parser.Matchers;

namespace CardPile.Parser;

public class DraftPickMatcher : ILogMatcher
{
    public event EventHandler<DraftPickEvent>? DraftPickEvent;

    public bool Match(string line)
    {
        if (!line.StartsWith(PREMIERE_DRAFT_PICK_PREFIX_NEEDLE))
        {
            return false;
        }

        if(!line.Contains(PREMIERE_DRAFT_PICK_DRAFT_ID_NEEDLE))
        {
            return false;
        }

        var e = ParseDraftEventInfo(line, PREMIERE_DRAFT_PICK_PREFIX_NEEDLE);
        if (e == null)
        {
            return false;
        }

        DraftPickEvent?.Invoke(this, e);

        return true;
    }

    private DraftPickEvent? ParseDraftEventInfo(string line, string needle)
    {
        dynamic? requestData = MatcherHelpers.ParseRequestUnchecked(line, needle);

        var draftId = requestData?.DraftId?.Value;
        var eventId = requestData?.EventId?.Value;
        var packNumber = requestData?.PackNumber?.Value;
        var pickNumber = requestData?.PickNumber?.Value;
        var cardsInPack = requestData?.CardsInPack?.ToObject<IList<int>>();
        var cardPicked = requestData?.PickGrpId?.Value;

        if (draftId == null || eventId == null || packNumber == null || pickNumber == null || cardsInPack == null || cardPicked == null ) 
        { 
            return null; 
        }

        return new DraftPickEvent(new Guid(draftId), eventId!, (int)packNumber, (int)pickNumber, cardsInPack!, (int)cardPicked);
    }

    private static string PREMIERE_DRAFT_PICK_PREFIX_NEEDLE = "[UnityCrossThreadLogger]==> LogBusinessEvents";
    private static string PREMIERE_DRAFT_PICK_DRAFT_ID_NEEDLE = "DraftId";
}
