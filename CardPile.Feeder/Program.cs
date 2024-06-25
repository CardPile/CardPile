using CardPile.Feeder;

class Program
{
    static void Main(string[] args)
    {
        const string DefaultSource = @"C:\Data\Programming\GitHub\CardPile\Test data\Logs\Player_20_06_2024.log";
        const string DefaultDestination = @"C:\Data\Programming\GitHub\CardPile\Test data\Logs\Player_fed.log";

        var source = DefaultSource;
        var destination = DefaultDestination;
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