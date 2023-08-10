
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR.Client;

namespace sharedCore;

public abstract class ConnectionOrchestratorBase
{
    protected readonly ConcurrentBag<HubConnection> _connections = new();
    protected readonly InitializerBase _initializer;
    protected readonly CancellationTokenSource _cancellationToken;

    protected readonly MessageAnalyticsBase _messageAnalytics;

    public ConnectionOrchestratorBase(InitializerBase initializer, 
        CancellationTokenSource cancellationToken,
        MessageAnalyticsBase messageAnalytics)
    {
        _initializer = initializer;
        _cancellationToken = cancellationToken;
        _messageAnalytics = messageAnalytics;

        CreateAllConnections();
    }


    public abstract void RegisterConnectionEvents(HubConnection connection);

    protected async void CreateAllConnections()
    {
        for (int i = 0; i < _initializer.Clients; i++)
        {
            var connection = CreateConnection(i);
            RegisterConnectionEvents(connection);
            await StartAsync(connection);

            _connections.Add(connection);
        }

        Console.WriteLine("Created and Started all connections...");
        Console.WriteLine("--------------------------------------");
    }

    public async Task CloseAllConnection()
    {
        foreach (var connection in _connections)
        {
            await connection.StopAsync();
            await connection.DisposeAsync();
        }
    }

    private HubConnection CreateConnection(int dataId) => new HubConnectionBuilder()
          .WithUrl($"{_initializer.WithUrl}?endpoint=xpto&dataId={dataId}")
          .Build();

    private async Task StartAsync(HubConnection connection)
    {
        await connection.StartAsync(cancellationToken: _cancellationToken.Token);
    }
}