using Microsoft.AspNetCore.SignalR.Client;

namespace sharedCore;

public abstract class ConnectionOrchestratorBase<TInitializer>
    where TInitializer : InitializerBase
{
    protected readonly HubConnection[] _connections;
    protected readonly TInitializer _initializer;
    private readonly string _group;

    public ConnectionOrchestratorBase(TInitializer initializer,
        string group)
    {
        _initializer = initializer;
        _group = group;
        _connections = new HubConnection[initializer.Clients];
    }

    public virtual Task RunAsync(CancellationToken token)
    {
        return CreateAllConnections(token);
    }

    public abstract Task RegisterConnectionEvents(HubConnection connection, CancellationToken token);

    protected async Task CreateAllConnections(CancellationToken token)
    {
        Task[] tasks = new Task[_connections.Length];
        for (int i = 0; i < _connections.Length; i++)
        {
            var connection = CreateConnection(i);
            _connections[i] = connection;
            tasks[i] = StartConnectionAndRegisterAsync(connection, token);
        }
        await Task.WhenAll(tasks);

        Console.WriteLine("Created and Started all connections...");
        Console.WriteLine("--------------------------------------");
    }

    private async Task StartConnectionAndRegisterAsync(HubConnection connection, CancellationToken token)
    {
        await StartConnectionAsync(connection, token);
        await RegisterConnectionEvents(connection, token);
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

    private async Task StartConnectionAsync(HubConnection connection, CancellationToken token)
    {
        await connection.StartAsync(cancellationToken: token);
    }
}