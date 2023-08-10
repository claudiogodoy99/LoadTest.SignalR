namespace sharedCore;

public class MessageAnalyticsBase
{

    private readonly SemaphoreSlim _semaphoreSlim = new(1);
    private int _messages = 0;
    public List<DateTime> _messagesTime = new();

    public double GetTotalMessages()
    {
        _semaphoreSlim.Wait();
        int totalMessage = _messages;
        _semaphoreSlim.Release();

        return totalMessage;
    }

    public MessageAnalyticsBase(CancellationToken token)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await LogAnalytics();
            }
        }, token);
    }

    private async Task LogAnalytics()
    {
        double messagesPerSecond = await GetMessagesPerSecond();

        Console.WriteLine("Analytics:");
        Console.WriteLine($"- Total messages: {_messages}");
        Console.WriteLine($"- Messages per second: {messagesPerSecond}");
         Console.WriteLine($"---------------------------------------------");
        Console.WriteLine($"- Threads in use: {ThreadPool.ThreadCount}");
        Console.WriteLine($"- PendingWorkItemCount: {ThreadPool.PendingWorkItemCount}");
        Console.WriteLine($"---------------------------------------------");
        Console.WriteLine();
    }


    public async Task RegisterMessage(DateTime time)
    {
        await _semaphoreSlim.WaitAsync();
        _messages++;
        _messagesTime.Add(time);
        _semaphoreSlim.Release();
    }

    public async Task<double> GetMessagesPerSecond()
    {
        double messagesPerSecond = 0;
        await _semaphoreSlim.WaitAsync();

        if (_messagesTime.Count > 1)
        {
            TimeSpan timeSpan = _messagesTime.Max() - _messagesTime.Min();
            messagesPerSecond = _messages / timeSpan.TotalSeconds;
        }

        _semaphoreSlim.Release();
        return messagesPerSecond;
    }

    public double CalculateMediaInterval()
    {
        TimeSpan totalInterval = TimeSpan.Zero;
        for (int i = 1; i < _messagesTime.Count; i++)
        {
            totalInterval += _messagesTime[i] - _messagesTime[i - 1];
        }

        TimeSpan averageInterval = TimeSpan.FromTicks(totalInterval.Ticks / (_messagesTime.Count - 1));

        return averageInterval.TotalMilliseconds;
    }

}