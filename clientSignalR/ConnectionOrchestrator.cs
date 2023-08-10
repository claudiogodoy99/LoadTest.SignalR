
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR.Client;
using sharedCore;

namespace clientSignalR;

public class ConnectionOrchestrator : ConnectionOrchestratorBase
{
    public ConnectionOrchestrator(Initializer initializer,
        CancellationTokenSource cancellationToken, 
        MessageAnalyticsBase messageAnalytics) : base(initializer, cancellationToken, messageAnalytics) {
        if (initializer.Reconnect)
        {
            RegisterReconnectionTask();
        }
    }

    public override void RegisterConnectionEvents(HubConnection connection)
    {
        connection.On<string>("addFullMessage", async (message) =>
        {
            var time = DateTime.Now;
            await _messageAnalytics.RegisterMessage(time);
        });
    }

    private void RegisterReconnectionTask()
    {
        Task.Run(async () =>
                   {
                       while (true)
                       {
                           await Task.Delay(TimeSpan.FromSeconds(10));
                           await CloseAllConnection();
                           _connections.Clear();
                           CreateAllConnections();
                       }
                   }, cancellationToken: _cancellationToken.Token);
    }



}