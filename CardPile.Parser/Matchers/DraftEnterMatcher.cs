using CardPile.Draft;
using CardPile.Parser.Matchers;

namespace CardPile.Parser;

public class DraftEnterMatcher : ILogMatcher
{
    public event EventHandler<DraftEnterEvent>? DraftStartEvent;

    public bool Match(string line)
    {
        if (!line.StartsWith(DRAFT_ENTER_NEEDLE))
        {
            return false;
        }
        
        var e = ParseDraftEventInfo(line, DRAFT_ENTER_NEEDLE);
        if(e == null)
        {
            return false;  
        }

        var draftStartEvent = DraftStartEvent;
        if(draftStartEvent != null)
        {
            draftStartEvent(this, e);
        }

        return true;        
    }

    private DraftEnterEvent? ParseDraftEventInfo(string line, string needle)
    {
        dynamic? requestData = MatcherHelpers.ParseRequestUnchecked(line, needle);
        var destinationScene = requestData?.toSceneName?.Value;
        if (destinationScene == null)
        {
            return null;
        }

        if (!destinationScene.StartsWith(DRAFT_ENTER_SCENE_NAME_NEEDLE))
        {
            return null;
        }

        return new DraftEnterEvent();
    }

    private static string DRAFT_ENTER_NEEDLE = "[UnityCrossThreadLogger]==> LogBusinessEvents";
    private static string DRAFT_ENTER_SCENE_NAME_NEEDLE = "Draft";
}
