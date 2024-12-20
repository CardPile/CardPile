using CardPile.Watchers.ArenaLog;

namespace CardPile.Feeder;

internal static class Program
{
    static void Main(string[] args)
    {
        string defaultSource = Path.Combine(Util.GetRepositoryRoot() ?? "\\", "Test data", "Logs", "Player_to_feed.log"); 
        string defaultDestination = Path.Combine(Util.GetRepositoryRoot() ?? "\\", "Test data", "Logs", "Player_fed.log");

        var source = defaultSource;
        var destination = defaultDestination;
        if (args.Length == 2)
        {
            source = args[0];
            destination = args[1];
        }

        var feeder = new LogFeeder(source, destination);
        feeder.Chunk();
        feeder.ClearDestination();

        while (feeder.HasMoreChunks())
        {
            Console.WriteLine($"Chunk {feeder.CurrentChunk()} of {feeder.ChunkCount()}. What to do? (Enter - next chunk, R - reset, Q - quit)");
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter)
            {
                feeder.FeedNextChunk();
            }
            else if(key.Key == ConsoleKey.R)
            {
                feeder.ResetChunks();
            }
            else if (key.Key == ConsoleKey.Q)
            {
                break;
            }
        }
    }
}