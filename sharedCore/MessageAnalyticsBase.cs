namespace sharedCore;

public class MessageAnalyticsBase
{
    public DateTime _first;
    public DateTime _last;
    public int count;
    public long ticks;

    public int totalCount;
    public long totalTicks;

    public MessageAnalyticsBase(CancellationToken token)
    {
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


    public void RegisterMessage(DateTime sent, DateTime received)
    {
        Interlocked.Add(ref ticks, (received - sent).Ticks);
        Interlocked.Increment(ref count);
    }

    public (double MessagesPerSecond, int Count, TimeSpan AvgLatency) GetWindowStatistics()
    {
        var localTicks = Interlocked.Exchange(ref ticks, 0);
        var localCount = Interlocked.Exchange(ref count, 0);
        var elapsedSecondsFromLast = (DateTime.UtcNow - _last).TotalSeconds;

        var result = (localCount / elapsedSecondsFromLast, localCount, TimeSpan.FromTicks(localTicks / localCount));
        Interlocked.Add(ref totalTicks, localTicks);
        Interlocked.Add(ref totalCount, localCount);
 
        _last = DateTime.UtcNow;
        return result;
    }

    public (double MessagesPerSecond, int Count, TimeSpan AvgLatency) GetTotalStatistics()
    {
        return (totalCount / (_last - _first).TotalSeconds, totalCount, TimeSpan.FromTicks(totalTicks / totalCount));
    }
}