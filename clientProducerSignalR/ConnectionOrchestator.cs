
namespace clientProducerSignalR;
using Microsoft.AspNetCore.SignalR.Client;
using sharedCore;
using System.Threading;

public class ConnectionOrchestrator : ConnectionOrchestratorBase<Initializer>
{
    public ConnectionOrchestrator(Initializer initializer, CancellationTokenSource cancellationToken)
        : base(initializer, "producer", cancellationToken)
    {
    }

    public override Task RegisterConnectionEvents(int slot, HubConnection connection)
    {
        return Task.Run(async () =>
        {
            var bytes = new char[1000];
            bytes.AsSpan().Fill(' ');
            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                for (int i = 0; i <= _initializer.Mps; i++)
                {
                    DateTime.UtcNow.Ticks.TryFormat(bytes, out _);
                    await connection.SendAsync("SendNotificationToGroup", $"XPTO|{i}", new string(bytes), _cancellationToken.Token);
                }
            }
        }, cancellationToken: _cancellationToken.Token);
    }
}