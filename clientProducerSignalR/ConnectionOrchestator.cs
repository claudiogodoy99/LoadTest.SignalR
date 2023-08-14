
namespace clientProducerSignalR;
using Microsoft.AspNetCore.SignalR.Client;
using sharedCore;
using System.Diagnostics;
using System.Threading;

public class ConnectionOrchestrator : ConnectionOrchestratorBase<Initializer>
{
    private readonly int messageSize;
    private readonly int consumerClients;
    private readonly int mps;

    public ConnectionOrchestrator(Initializer initializer, CancellationTokenSource cancellationToken)
        : base(initializer, "producer", cancellationToken)
    {
        messageSize = initializer.MessageSize;
        consumerClients = initializer.ConsumerClients;
        mps = initializer.Mps / initializer.Clients;
    }

    public override Task RegisterConnectionEvents(int slot, HubConnection connection)
    {
        return Task.Run(async () =>
        {
            var bytes = new char[messageSize];
            bytes.AsSpan().Fill(' ');
            TimeSpan oneSecond = TimeSpan.FromSeconds(1);
            while (!_cancellationToken.IsCancellationRequested)
            {
                var timestamp = Stopwatch.GetTimestamp();
                for (int i = 0; i <= mps; i++)
                {
                    DateTime.UtcNow.Ticks.TryFormat(bytes, out _);
                    await connection.SendAsync("SendNotificationToGroup", $"XPTO|{i % consumerClients}", new string(bytes), _cancellationToken.Token);
                }
                TimeSpan interval = Stopwatch.GetElapsedTime(timestamp);
                if (interval <= oneSecond)
                    await Task.Delay(oneSecond - interval, _cancellationToken.Token);
            }
        }, cancellationToken: _cancellationToken.Token);
    }
}