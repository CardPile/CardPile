namespace CardPile.Watchers.ArenaLog;

public class Util
{
    public static string? GetRepositoryRoot()
    {
        var startingDirectory = Environment.CurrentDirectory;
        for (var currentDir = startingDirectory; currentDir != null; currentDir = Directory.GetParent(currentDir)?.FullName)
        {
            var gitDir = Path.Combine(currentDir, ".git");
            if (Directory.Exists(gitDir))
            {
                return currentDir;
            }
        }

        return startingDirectory;
    }
}