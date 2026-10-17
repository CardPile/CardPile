using NLog;

namespace CardPile.Synergies;

public class Synergy
{
    public Synergy(SynergyStrength strength, string reason)
    {
        Strength = strength;
        Reason = reason;
    }

    public SynergyStrength Strength { get; init; }

    public string Reason { get; init; }

    internal static Synergy? ParseSynergy(string desc)
    {
        if (string.IsNullOrWhiteSpace(desc))
        {
            logger.Warn("Cannot parse synergy - the synergy description is null or whitesapce");
            return null;
        }

        if (desc.StartsWith("no"))
        {
            return new Synergy(SynergyStrength.None, string.Empty);
        }

        var parts = desc.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
        if (parts.Count != 2)
        {
            logger.Warn("Cannot parse synergy - it does not consist of two parts separated with \"|\". It is {desc}", desc);
            return null;
        }

        if (!Enum.TryParse(parts[0], true, out SynergyStrength strength))
        {
            logger.Warn("Cannot parse synergy - the strength description is invalid. It is {strengthDesc}", parts[0]);
            return null;
        }

        return new Synergy(strength, parts[1]);
    }

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
