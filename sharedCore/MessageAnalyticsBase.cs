namespace sharedCore;

public class MessageAnalyticsBase
{
    public DateTime _first;
    public DateTime _last;
    private readonly int[] _offsetMessages;
    public readonly List<(DateTime Sent, DateTime Received)>[] _messagesTimeSlots;

    public MessageAnalyticsBase(int clients, CancellationToken token)
    {
        _messagesTimeSlots = new List<(DateTime Sent, DateTime Received)>[clients];
        _offsetMessages = new int[clients];
        for (int i = 0; i < _messagesTimeSlots.Length; i++)
            _messagesTimeSlots[i] = new List<(DateTime Sent, DateTime Received)>(1 << 20);

        Task.Run(async () =>
        {
            _first = _last = DateTime.UtcNow;
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await LogAnalytics();
            }
        }, token);
    }

    private async Task LogAnalytics()
    {
        (double messagesPerSecond, int count, TimeSpan avgLatency) = GetWindowStatistics();
        Console.WriteLine("Analytics:");
        Console.WriteLine($"- Total messages: {count}");
        Console.WriteLine($"- Messages per second: {messagesPerSecond}");
        Console.WriteLine($"- Average latency: {avgLatency}");
        Console.WriteLine($"---------------------------------------------");
        Console.WriteLine($"- Threads in use: {ThreadPool.ThreadCount}");
        Console.WriteLine($"- PendingWorkItemCount: {ThreadPool.PendingWorkItemCount}");
        Console.WriteLine($"---------------------------------------------");
        Console.WriteLine();
    }


    public void RegisterMessage(int slot, DateTime sent, DateTime received)
    {
        var messagesTime = _messagesTimeSlots[slot];
        lock (messagesTime)
            messagesTime.Add((sent, received));
    }

    public (double MessagesPerSecond, int Count, TimeSpan AvgLatency) GetWindowStatistics()
    {
        int count = 0;
        TimeSpan sumLatency = TimeSpan.Zero;

        for (int i = 0; i < _offsetMessages.Length; i++)
        {
            var stats = GetStatistics(_messagesTimeSlots[i], ref _offsetMessages[i]);

            count += stats.Count;
            sumLatency += stats.SumLatency;
        }
        var result = (count / (DateTime.UtcNow - _last).TotalSeconds, count, sumLatency / count);
        _last = DateTime.UtcNow;
        return result;
    }

    private (int Count, TimeSpan SumLatency) GetStatistics(List<(DateTime Sent, DateTime Received)> messagesTime, ref int offset)
    {
        int count = 0;
        TimeSpan averageLatency = TimeSpan.Zero;
        lock (messagesTime)
        {
            count = messagesTime.Count - offset;
            if (count > 1)
            {
                TimeSpan latency = TimeSpan.Zero;
                for (int i = 0; i < count; i++)
                {
                    var message = messagesTime[offset + i];
                    latency += message.Received - message.Sent;
                }
                averageLatency = latency / count;
                offset = messagesTime.Count;
            }
        }
        return (count, averageLatency);
    }

    public (double MessagesPerSecond, int Count, TimeSpan AvgLatency) GetTotalStatistics()
    {
        int count = 0;
        TimeSpan sumLatency = TimeSpan.Zero;

        for (int i = 0; i < _offsetMessages.Length; i++)
        {
            int offset = 0;
            var stats = GetStatistics(_messagesTimeSlots[i], ref offset);

            count += stats.Count;
            sumLatency += stats.SumLatency;
        }
        return (count / (_first - _last).TotalSeconds, count, sumLatency / count);
    }
}