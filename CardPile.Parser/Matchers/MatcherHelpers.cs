using Newtonsoft.Json.Linq;

namespace CardPile.Parser.Matchers;

internal class MatcherHelpers
{
    internal static readonly Guid CARD_PILE_DRAFT_NAMESPACE_GUID = new("D9A69F35-EFDA-474F-BED1-FC139AC3C952");

    internal static JObject? ParseRequest(string line, string needle)
    {
        dynamic? data = Parse(line, needle);
        var request = data?.request?.Value;
        if (request == null)
        {
            return null;
        }

        return JObject.Parse(request);
    }

    internal static JObject? ParseRequestUnchecked(string line, string needle)
    {
        dynamic data = ParseUnchecked(line, needle);
        var request = data.request?.Value;
        if (request == null)
        {
            return null;
        }

        return JObject.Parse(request);
    }

    internal static JObject? Parse(string line, string needle)
    {
        var index = line.IndexOf(needle, StringComparison.Ordinal);
        if (index == -1)
        {
            return null;
        }

        var tail = line[(index + needle.Length)..];

        return JObject.Parse(tail);
    }

    internal static JObject ParseUnchecked(string line, string needle)
    {
        var index = line.IndexOf(needle, StringComparison.Ordinal);

        var tail = line[(index + needle.Length)..];

        return JObject.Parse(tail);
    }
}
