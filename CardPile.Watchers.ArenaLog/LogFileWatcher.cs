using NLog;
using System.Text;

namespace CardPile.Watchers.ArenaLog;

public class LogFileWatcher
{
    public event EventHandler<NewLineEvent>? NewLineEvent;

    public LogFileWatcher()
    {}

    public LogFileWatcher(string path, bool processExisting)
    {
        Watch(path, processExisting);
    }

    public string? FilePath
    {
        get => filePath;
    }

    public void Watch(string path, bool processExisting)
    {
        filePath = path;
        if(processExisting)
        {
            lastLength = 0;
        }
        else
        {
            var fileInfo = new FileInfo(filePath);
            if(fileInfo.Exists)
            {
                lastLength = fileInfo.Length;
            }
        }
    }

    public void Poll()
    {
        try
        {
            if (filePath == null)
            {
                return;
            }

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return;
            }

            if (fileInfo.Length == lastLength)
            {
                return;
            }

            if (fileInfo.Length < lastLength)
            {
                var prevFileInfo = BuildPrevFilepath(fileInfo);
                if (prevFileInfo.Exists && prevFileInfo.Length > lastLength)
                {
                    ProcessLines(prevFileInfo);
                }

                lastLength = 0;
            }

            ProcessLines(fileInfo);
        }
        catch (Exception e)
        {
            logger.Error(e);
            throw;
        }
    }

    private static FileInfo BuildPrevFilepath(FileInfo fileInfo)
    {
        var extension = Path.GetExtension(fileInfo.FullName);
        var pathWithoutExtentsion = Path.GetFileNameWithoutExtension(fileInfo.FullName);
        var prevFilepath = pathWithoutExtentsion + "-prev." + extension;
        return new FileInfo(prevFilepath);
    }

    private void ProcessLines(FileInfo fileInfo)
    {
        var newLineEvent = NewLineEvent;
        if (newLineEvent == null)
        {
            return;
        }

        var buffer = new byte[fileInfo.Length - lastLength];
        using (FileStream fs = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            fs.Seek(lastLength, SeekOrigin.Begin);
            fs.Read(buffer, 0, buffer.Length);
        }

        var newLineByteCount = Encoding.UTF8.GetByteCount(Environment.NewLine);
        var contents = Encoding.UTF8.GetString(buffer);

        int start = 0;
        for (int i = 0; i < contents.Length;)
        {
            if( string.Compare(contents, i, Environment.NewLine, 0, Environment.NewLine.Length) == 0 )
            {
                var line = contents.Substring(start, i - start);
                lastLength += Encoding.UTF8.GetByteCount(line) + newLineByteCount;

                var e = new NewLineEvent(line);
                newLineEvent(this, e);

                i += Environment.NewLine.Length;
                start = i;
            }
            else
            {
                ++i;
            }
        }
    }

    private string? filePath = null;
    private long lastLength = 0;

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
