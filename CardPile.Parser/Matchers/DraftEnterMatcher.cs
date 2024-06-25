using CardPile.Draft;
using CardPile.Parser.Matchers;

namespace CardPile.Parser;

public class DraftEnterMatcher : ILogMatcher
{
    public event EventHandler<DraftEnterEvent>? DraftEnterEvent;

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

        DraftEnterEvent?.Invoke(this, e);

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

        if (destinationScene != DESTINATION_SCENE_NAME_NEEDLE)
        {
            return null;
        }

        return new DraftEnterEvent();
    }

    private static string SCENE_CHANGE_NEEDLE = "[UnityCrossThreadLogger]==> LogBusinessEvents";
    private static string DESTINATION_SCENE_NAME_NEEDLE = "Draft";
}
