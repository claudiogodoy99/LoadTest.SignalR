using Microsoft.AspNetCore.SignalR.Client;
using sharedCore;

namespace clientSignalR;

public class ConnectionOrchestrator : MonitoredConnectionOrchestratorBase<Initializer>
{
    private readonly CancellationToken cancellationToken;

    public ConnectionOrchestrator(Initializer initializer,
        CancellationTokenSource cancellationTokenSource,
        MessageAnalyticsBase messageAnalytics) : base(initializer, "xpto", cancellationTokenSource, messageAnalytics)
    {
        cancellationToken = cancellationTokenSource.Token;
        if (initializer.Reconnect)
        {
            RegisterReconnectionTask();
        }
    }

    public override Task RegisterConnectionEvents(int slot, HubConnection connection)
    {
        var subscription = connection.On<string>("addFullMessage", (message) =>
        {
            var s = message.AsSpan();
            long.TryParse(s.Slice(0, s.IndexOf(' ')), out long ticks);
            var sent = new DateTime(ticks);
            var received = DateTime.UtcNow;
            _messageAnalytics.RegisterMessage(sent, received);
        });
        TaskCompletionSource source = new TaskCompletionSource();
        cancellationToken.Register(() =>
        {
            subscription.Dispose();
            source.SetResult();
        });
        return source.Task;
    }

    private void RegisterReconnectionTask()
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await CloseAllConnection();
                await CreateAllConnections();
            }
        }, cancellationToken: _cancellationToken.Token);
    }



}