using clientProducerSignalR;
using sharedCore;

namespace clientProducerSignalR;

public class CsvWriter
{

    public readonly MessageAnalyticsBase _messageAnalytics;
    public readonly Initializer _initializer;

    public CsvWriter(MessageAnalyticsBase messageAnalytics, Initializer initializer)
    {
        _messageAnalytics = messageAnalytics;
        _initializer = initializer;
    }


    public async Task RegisterTest()
    {
        using StreamWriter outputFile = new(Path.Combine(_initializer.Path, "ResultsProducer.txt"), append: true);

        if (outputFile.BaseStream.Length == 0)
            await outputFile.WriteLineAsync("Date ;Clients Count; TotalMessages ; MessagePerSecond ; MediaIntervalBetweenMessages ; Duration ; MPS ; Comments");

        await outputFile.WriteLineAsync($"{DateTime.Now.ToString("dd:MM:yyyy hh:mm:ss")} ;{_initializer.Clients} ; {_messageAnalytics.GetTotalMessages()} ; {await _messageAnalytics.GetMessagesPerSecond()} ; {_messageAnalytics.CalculateMediaInterval()} ; {_initializer.Duration} ; {_initializer.Mps} ; {_initializer.Comments} ");
    }

}