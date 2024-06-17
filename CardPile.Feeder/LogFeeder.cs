using CardPile.Parser;

namespace CardPile.Feeder;

internal class LogFeeder
{
    internal LogFeeder(string source, string destination)
    {
        this.source = source;
        this.destination = destination;

        this.logWatcher = new LogFileWatcher();
        this.dispatcher = new MatcherDispatcher();

        this.logWatcher.NewLineEvent += NewLineHandler;
        this.dispatcher.AddMatcher<DraftEnterMatcher>().DraftStartEvent += DraftStartHandler;
        this.dispatcher.AddMatcher<DraftChoiceMatcher>().DraftChoiceEvent += DraftChoiceHandler;
        this.dispatcher.AddMatcher<DraftPickMatcher>().DraftPickEvent += DraftPickHandler;
        this.dispatcher.AddMatcher<DraftEndMatcher>().DraftEndEvent += DraftEndHandler;

        this.currentLine = string.Empty;
        this.currentLineMatched = false;

        this.chunks = [];

        this.currentChunkIndex = 0;
    }

    internal void Chunk()
    {
        logWatcher.Watch(source, true);
        logWatcher.Poll();
    }

    internal void FeedNextChunk()
    {
        if (currentChunkIndex == 0)
        {
            ClearDestination();
        }

        if(currentChunkIndex < chunks.Count)
        {
            using var file = File.Open(destination, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using var outputFile = new StreamWriter(file);
            
            string lastLine = string.Empty;
            foreach (var line in chunks[currentChunkIndex])
            {
                outputFile.WriteLine(line);
                lastLine = line;
            }
            Console.WriteLine(lastLine);
            currentChunkIndex++;
        }
    }
    
    internal void ClearDestination()
    {
        if (File.Exists(destination))
        {
            File.Delete(destination);
        }
    }

    internal bool HasMoreChunks()
    {
        return currentChunkIndex < chunks.Count;
    }

    internal int CurrentChunk()
    {
        return currentChunkIndex;
    }

    internal int ChunkCount()
    {
        return chunks.Count;
    }

    internal void ResetChunks()
    {
        currentChunkIndex = 0;
    }

    private void NewLineHandler(object? sender, NewLineEvent e)
    {
        currentLine = e.Line;
        currentLineMatched = false;
        dispatcher.Dispatch(currentLine);

        if (chunks.Count == 0)
        {
            chunks.Add([]);
        }

        chunks.Last().Add(currentLine);

        if (currentLineMatched)
        {
            chunks.Add([]);
        }
    }

    private void DraftStartHandler(object? sender, Draft.DraftEnterEvent e)
    {
        currentLineMatched = true;
    }

    private void DraftChoiceHandler(object? sender, Draft.DraftChoiceEvent e)
    {
        currentLineMatched = true;
    }

    private void DraftPickHandler(object? sender, Draft.DraftPickEvent e)
    {
        currentLineMatched = true;
    }
    private void DraftEndHandler(object? sender, Draft.DraftEndEvent e)
    {
        currentLineMatched = true;
    }

    private string source;
    private string destination;

    private LogFileWatcher logWatcher;
    private MatcherDispatcher dispatcher;

    private string currentLine;
    private bool currentLineMatched;

    private List<List<string>> chunks;

    private int currentChunkIndex;
}
