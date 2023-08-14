
namespace clientProducerSignalR;
using Microsoft.AspNetCore.SignalR.Client;
using sharedCore;
using System.Threading;

public class ConnectionOrchestrator : ConnectionOrchestratorBase<Initializer>
{
    private readonly int messageSize;
    private readonly int delay;

    public ConnectionOrchestrator(Initializer initializer, CancellationTokenSource cancellationToken)
        : base(initializer, "producer", cancellationToken)
    {
        messageSize = initializer.MessageSize;
        delay = initializer.Delay;
    }

    public override Task RegisterConnectionEvents(int slot, HubConnection connection)
    {
        return Task.Run(async () =>
        {
            var bytes = new char[messageSize];
            bytes.AsSpan().Fill(' ');
            while (!_cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(delay));
                for (int i = 0; i <= _initializer.Mps; i++)
                {
                    DateTime.UtcNow.Ticks.TryFormat(bytes, out _);
                    await connection.SendAsync("SendNotificationToGroup", $"XPTO|{i}", new string(bytes), _cancellationToken.Token);
                }
            }
        }, cancellationToken: _cancellationToken.Token);
    }
}