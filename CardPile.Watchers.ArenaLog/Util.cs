namespace CardPile.Watchers.ArenaLog;

public static class Util
{
    public static string GetRepositoryRoot()
    {
        var startingDirectory = Environment.CurrentDirectory;
        for (var currentDir = startingDirectory; currentDir != null; currentDir = Directory.GetParent(currentDir)?.FullName)
        {
            var gitPath = Path.Combine(currentDir, ".git");
            if (File.Exists(gitPath) || Directory.Exists(gitPath))
            {
                return currentDir;
            }
        }

        return startingDirectory;
    }
}