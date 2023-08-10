
namespace clientProducerSignalR;

using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using sharedCore;

public class ConnectionOrchestrator : ConnectionOrchestratorBase
{
    private new readonly Initializer _initializer;
    public ConnectionOrchestrator(Initializer initializer, CancellationTokenSource cancellationToken, MessageAnalyticsBase messageAnalytics) : base(initializer, cancellationToken, messageAnalytics)
    {
        _initializer = initializer;
    }

    public override void RegisterConnectionEvents(HubConnection connection)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                List<Task> analytics = new();

                var bytes = new byte[1000];

                for (int i = 0; i <= _initializer.Mps; i++)
                {
                    await connection.InvokeAsync("SendNotificationToGroup", $"XPTO|{i}", bytes);
                    analytics.Add(_messageAnalytics.RegisterMessage(DateTime.Now));
                }

                await Task.WhenAll(analytics);
            }
        }, cancellationToken: _cancellationToken.Token);
    }
}