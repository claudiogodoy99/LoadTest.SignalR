using Microsoft.AspNetCore.SignalR.Client;
using sharedCore;

namespace clientSignalR;

public sealed class ConnectionOrchestrator : MonitoredConnectionOrchestratorBase<Initializer>
{
    private bool reconnect;

    public ConnectionOrchestrator(Initializer initializer,
        MessageAnalyticsBase messageAnalytics) : base(initializer, "xpto", messageAnalytics)
    {
        reconnect = initializer.Reconnect;
    }

    public override Task RunAsync(CancellationToken token)
    {
        if (reconnect)
            _ = RegisterReconnectionTask(token);
        return base.RunAsync(token);
    }

    public override Task RegisterConnectionEvents(HubConnection connection, CancellationToken token)
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
        token.Register(() =>
        {
            subscription.Dispose();
            source.SetResult();
        });
        return source.Task;
    }

    private Task RegisterReconnectionTask(CancellationToken token)
    {
        return Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await CloseAllConnection();
                await CreateAllConnections(token);
            }
        }, cancellationToken: token);
    }



}