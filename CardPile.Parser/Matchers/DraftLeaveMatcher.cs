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

        DraftLeaveEvent?.Invoke(this, e);

        return true;        
    }

    private static DraftLeaveEvent? ParseDraftEventInfo(string line, string needle)
    {
        dynamic data = MatcherHelpers.ParseUnchecked(line, needle);
        var sourceScene = data.fromSceneName?.Value;
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

    private static readonly string SCENE_CHANGE_NEEDLE = "[UnityCrossThreadLogger]Client.SceneChange";
    private static readonly string SOURCE_SCENE_NAME_NEEDLE = "Draft";
}
