using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using NLog;
using System.Runtime.Versioning;

namespace CardPile.CardInfo;

public class Arena
{
    static Arena()
    {
        var arenaInstallDirectory = GetArenaInstallDirectory();
        var arenaDataSubdirectory = GetArenaDataSubdirectory(arenaInstallDirectory) ?? throw new InvalidOperationException("Cannot locate Magic: The Gathering Arena installation");
        var cardDatabaseFiles = Directory.GetFiles(arenaDataSubdirectory, "Raw_CardDatabase_*.mtga", SearchOption.AllDirectories);
        if (cardDatabaseFiles.Length == 0)
        {
            throw new InvalidOperationException("Cannot locate Magic: The Gathering Arena card database");
        }
        if (cardDatabaseFiles.Length > 1)
        {
            throw new InvalidOperationException("Found multiple Magic: The Gathering Arena card databases");
        }

        LoadCardDatabase(cardDatabaseFiles.First());
    }

    public static void Init()
    {
        // NOOP - runs the static constructor
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

    private static string? GetArenaInstallDirectory()
    {
        if (OperatingSystem.IsWindows())
        {
            const string SteamNeedle = "Magic: The Gathering Arena";
            const string StandaloneNeedle = "MTG Arena";

            string? installedLocation = default;
            Version? installedVersion = default;

            foreach (var (rootRegKey, searchLocation) in searchLocations)
            {
                RegistryKey? regKey = rootRegKey.OpenSubKey(searchLocation);
                if (regKey == null)
                {
                    continue;
                }

                foreach (var subSearchLocation in regKey.GetSubKeyNames())
                {
                    RegistryKey? rsk = regKey.OpenSubKey(subSearchLocation);
                    string? displayNameValue = rsk?.GetValue("DisplayName") as string;
                    if (displayNameValue == SteamNeedle)
                    {
                        string? installLocationValue = rsk?.GetValue("InstallLocation") as string;
                        if (string.IsNullOrWhiteSpace(installLocationValue))
                        {
                            continue;
                        }

                        // Assume the Steam version is the newest one, so just return it
                        return installLocationValue;
                    }
                    else if(displayNameValue == StandaloneNeedle)
                    {
                        string? installLocationValue = rsk?.GetValue("InstallLocation") as string;
                        if (string.IsNullOrWhiteSpace(installLocationValue))
                        {
                            continue;
                        }

                        string? displayVersionValue = rsk?.GetValue("DisplayVersion") as string;
                        if (string.IsNullOrWhiteSpace(displayVersionValue))
                        {
                            continue;
                        }

                        if (!Version.TryParse(displayVersionValue, out Version? version))
                        {
                            continue;
                        }

                        if(installedVersion == null || version > installedVersion)
                        {
                            installedLocation = installLocationValue;
                            installedVersion = version;
                        }
                    }
                }
            }

            return installedLocation;
        }

        return default;
    }

    private static string? GetArenaDataSubdirectory(string? arenaInstallDirectory)
    {
        if (OperatingSystem.IsWindows())
        {
            if(arenaInstallDirectory == null)
            {
                return default;
            }

            return Path.Combine(arenaInstallDirectory, "MTGA_Data");
        }
        else if(OperatingSystem.IsMacOS())
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if(string.IsNullOrEmpty(userProfile))
            {
                return default;
            }

            return Path.Combine(userProfile, "Library", "Application Support", "com.wizards.mtga");
        }
        else
        {
            throw new NotSupportedException($"Unsupported operating system {Environment.OSVersion}");
        }
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
