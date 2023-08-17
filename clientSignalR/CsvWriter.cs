using sharedCore;

namespace clientSignalR;

public class CsvWriter
{

    public readonly MessageAnalyticsBase _messageAnalytics;
    public readonly Initializer _initializer;

    public CsvWriter(MessageAnalyticsBase messageAnalytics, Initializer initializer)
    {
        _messageAnalytics = messageAnalytics;
        _initializer = initializer;
        EnsurePathExists();
    }

    private void EnsurePathExists()
    {
        if (!Directory.Exists(_initializer.Path))
        {
            Console.Write($"Path {_initializer.Path} not found, creating directory... ");
            Directory.CreateDirectory(_initializer.Path);
            Console.WriteLine("OK!");
        }
    }

    public async Task RegisterTest()
    {
        using StreamWriter outputFile = new(Path.Combine(_initializer.Path, "ResultsConsumer.txt"), append: true);

        if (outputFile.BaseStream.Length == 0)
            await outputFile.WriteLineAsync("Date ;Clients Count; TotalMessages ; MessagePerSecond ; AverageLatency ; Duration ; Reconnect ; Comments");

        var stats = _messageAnalytics.GetTotalStatistics();
        await outputFile.WriteLineAsync($"{DateTime.Now.ToString("dd:MM:yyyy hh:mm:ss")} ;{_initializer.Clients} ; {stats.Count} ; {stats.MessagesPerSecond} ; {stats.AvgLatency} ; {_initializer.Duration} ; {_initializer.Reconnect} ; {_initializer.Comments} ");
    }

}