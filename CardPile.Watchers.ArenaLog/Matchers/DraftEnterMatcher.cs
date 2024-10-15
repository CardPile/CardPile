using CardPile.Draft;

namespace CardPile.Watchers.ArenaLog.Matchers;

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

    private static DraftEnterEvent? ParseDraftEventInfo(string line, string needle)
    {
        dynamic data = MatcherHelpers.ParseUnchecked(line, needle);
        var destinationScene = data.toSceneName?.Value;
        if (destinationScene == null)
        {
            return null;
        }

        if (destinationScene != DESTINATION_SCENE_NAME_NEEDLE)
        {
            return null;
        }

        return new();
    }

    private static readonly string SCENE_CHANGE_NEEDLE = "[UnityCrossThreadLogger]Client.SceneChange";
    private static readonly string DESTINATION_SCENE_NAME_NEEDLE = "Draft";
}
