internal class LogParser
{
    readonly FileSystemWatcher watcher;
    readonly string logPath = AppDomain.CurrentDomain.BaseDirectory + "\\zapret\\blockcheck";
    long lastPosition = 0;
    bool expectNextLine = false;

    public event Action<string> NewLogMessage;

    public LogParser()
    {
        try
        {
            watcher = new FileSystemWatcher
            {
                Path = logPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            watcher.Changed += OnFileChanged;
        }
        catch (Exception ex)
        {
            NewLogMessage?.Invoke($"FileWatcher error: {ex.Message}");
        }
    }

    void OnFileChanged(object sender, FileSystemEventArgs e) => ReadFile();

    void ReadFile()
    {
        try
        {
            using var fs = new FileStream(logPath + "\\blockcheck.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            if (fs.Length > lastPosition)
            {
                fs.Seek(lastPosition, SeekOrigin.Begin);
                using var reader = new StreamReader(fs);
                var content = reader.ReadToEnd();

                if (!string.IsNullOrEmpty(content))
                {
                    ParseLogFile(content);
                    lastPosition = fs.Position;
                }
            }
        }
        catch (Exception ex)
        {
            NewLogMessage?.Invoke($"File read error: {ex.Message}");
        }
    }

    void ParseLogFile(string content)
    {
        string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (expectNextLine)
            {
                var processedLine = line.TrimStart();

                var winwsIndex = processedLine.IndexOf("winws");

                if (winwsIndex != -1)
                {
                    var startIndex = winwsIndex + "winws".Length;

                    if (startIndex < processedLine.Length && processedLine[startIndex] == ' ')
                        startIndex++;

                    var result = processedLine.Substring(startIndex);

                    NewLogMessage?.Invoke(result);
                }

                expectNextLine = false;
            }
            else if (line.Contains("!!!!! AVAILABLE !!!!!"))
            {
                expectNextLine = true;
            }
        }
    }
}
