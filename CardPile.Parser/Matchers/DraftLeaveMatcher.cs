using CardPile.Draft;
using CardPile.Parser.Matchers;

namespace CardPile.Parser;

public class DraftLeaveMatcher : ILogMatcher
{
    public event EventHandler<DraftLeaveEvent>? DraftLeaveEvent;

    public bool Match(string line)
    {
        if (!line.StartsWith(SCENE_CHANGE_NEEDLE))
        {
            return false;
        }
        
        var e = ParseDraftEventInfo(line, SCENE_CHANGE_NEEDLE);
        if(e == null)
        {
            return false;  
        }

        var draftLeaveEvent = DraftLeaveEvent;
        if(draftLeaveEvent != null)
        {
            draftLeaveEvent(this, e);
        }

        return true;        
    }

    private DraftLeaveEvent? ParseDraftEventInfo(string line, string needle)
    {
        dynamic? requestData = MatcherHelpers.ParseRequestUnchecked(line, needle);
        var sourceScene = requestData?.fromSceneName?.Value;
        if (sourceScene == null)
        {
            return null;
        }

        if (sourceScene != SOURCE_SCENE_NAME_NEEDLE)
        {
            return null;
        }

        return new();
    }

    private static string SCENE_CHANGE_NEEDLE = "[UnityCrossThreadLogger]==> LogBusinessEvents";
    private static string SOURCE_SCENE_NAME_NEEDLE = "Draft";
}
