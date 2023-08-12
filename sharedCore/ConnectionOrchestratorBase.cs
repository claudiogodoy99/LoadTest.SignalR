using Microsoft.AspNetCore.SignalR.Client;

namespace sharedCore;

public abstract class ConnectionOrchestratorBase<TInitializer>
    where TInitializer : InitializerBase
{
    protected readonly HubConnection[] _connections;
    protected readonly TInitializer _initializer;
    private readonly string _group;
    protected readonly CancellationTokenSource _cancellationToken;

    public ConnectionOrchestratorBase(TInitializer initializer,
        string group,
        CancellationTokenSource cancellationToken)
    {
        _initializer = initializer;
        _group = group;
        _connections = new HubConnection[initializer.Clients];
        _cancellationToken = cancellationToken;

    }

    public Task StartAsync()
    {
        return CreateAllConnections();
    }

    public abstract Task RegisterConnectionEvents(int slot, HubConnection connection);

    protected async Task CreateAllConnections()
    {
        Task[] tasks = new Task[_connections.Length];
        for (int i = 0; i < _connections.Length; i++)
        {
            var connection = CreateConnection(i);
            _connections[i] = connection;
            tasks[i] = StartConnectionAndRegisterAsync(i, connection);
        }
        await Task.WhenAll(tasks);

        Console.WriteLine("Created and Started all connections...");
        Console.WriteLine("--------------------------------------");
    }

    private async Task StartConnectionAndRegisterAsync(int i, HubConnection connection)
    {
        await StartConnectionAsync(connection);
        await RegisterConnectionEvents(i, connection);
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
          .WithUrl($"{_initializer.WithUrl}?endpoint={_group}&dataId={dataId}")
          .Build();

    private async Task StartConnectionAsync(HubConnection connection)
    {
        await connection.StartAsync(cancellationToken: _cancellationToken.Token);
    }
}