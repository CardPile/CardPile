using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using NLog;
using System.Runtime.Versioning;

namespace CardPile.CardInfo;

public class Arena
{
    static Arena()
    {
        string? cardDatabaseLocation = default;
        if (OperatingSystem.IsWindows())
        {
            var arenaInstallDirectory = GetArenaInstallDirectory() ?? throw new InvalidOperationException("Cannot locate Magic: The Gathering Arena installation");
            var arenaDataSubdirectory = Path.Combine(arenaInstallDirectory, "MTGA_Data");
            var cardDatabaseFiles = Directory.GetFiles(arenaDataSubdirectory, "Raw_CardDatabase_*.mtga", SearchOption.AllDirectories);
            if (cardDatabaseFiles.Length == 0)
            {
                throw new InvalidOperationException("Cannot locate Magic: The Gathering Arena card database");
            }
            if (cardDatabaseFiles.Length > 1)
            {
                throw new InvalidOperationException("Found multiple Magic: The Gathering Arena card databases");
            }
            cardDatabaseLocation = cardDatabaseFiles.First();
        }

        if (cardDatabaseLocation == null)
        {
            throw new InvalidOperationException("Cannot locate Magic: The Gathering Arena card database");
        }

        LoadCardDatabase(cardDatabaseLocation);
    }

    public static void Init()
    {
        // NOOP
    }

    public static string? GetCardNameFromId(int cardId)
    {
        if(cardIdToName.TryGetValue(cardId, out var name))
        {
            return name;
        }
        return null;
    }

    public static (string?, string?) GetCardExpansionAndCollectorNumberFromId(int cardId)
    {
        if (cardIdToExpansionAndCollectorNumber.TryGetValue(cardId, out var expansionAndCollectorNumber))
        {
            return expansionAndCollectorNumber;
        }
        return (null, null);
    }

    [SupportedOSPlatform("windows")]
    private static string? GetArenaInstallDirectory()
    {
        const string SteamNeedle = "Magic: The Gathering Arena";

        foreach(var (rootRegKey, searchLocation) in searchLocations)
        {
            RegistryKey? regKey = rootRegKey.OpenSubKey(searchLocation);
            if (regKey == null)
            {
                continue;
            }

            foreach(var subSearchLocation in regKey.GetSubKeyNames())
            {
                RegistryKey? rsk = regKey.OpenSubKey(subSearchLocation);
                string? displayName = rsk?.GetValue("DisplayName") as string;
                if (displayName == SteamNeedle)
                {
                    string? installLocation = rsk?.GetValue("InstallLocation") as string;
                    if(!string.IsNullOrWhiteSpace(installLocation))
                    {
                        return installLocation;
                    }
                }
            }
        }    

        return default;
    }

    private static void LoadCardDatabase(string arenaCardDatabasePath)
    {
        using var connection = new SqliteConnection($"Data Source={arenaCardDatabasePath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT GrpId, ExpansionCode, CollectorNumber, enUS FROM Cards LEFT JOIN Localizations ON Cards.TitleId == Localizations.LocId WHERE Localizations.Formatted = 2";
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var grpId = reader.GetInt32(0);
            var expansion = reader.GetString(1);
            var collectorNumber = reader.GetString(2);
            var name = reader.GetString(3);

            cardIdToName.Add(grpId, name);
            cardIdToExpansionAndCollectorNumber.Add(grpId, (expansion, collectorNumber));
        }
    }

    [SupportedOSPlatform("windows")]
    private static readonly (RegistryKey, string)[] searchLocations =
    [
        (Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
        (Registry.LocalMachine, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"),
        (Registry.CurrentUser, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
        (Registry.CurrentUser, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
    ];

    private static Dictionary<int, string> cardIdToName = [];
    private static Dictionary<int, (string, string)> cardIdToExpansionAndCollectorNumber = [];

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
