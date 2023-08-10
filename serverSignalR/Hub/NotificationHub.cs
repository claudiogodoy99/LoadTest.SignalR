using Microsoft.AspNetCore.SignalR;

namespace serverSignalR
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;
        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        private static HttpContext GetContext(HttpContext? context)
        {
            if (context is not null)
            {
                return context;
            }
            throw new ArgumentNullException(nameof(context));
        }

        public override async Task OnConnectedAsync()
        {
            var context = GetContext(base.Context.GetHttpContext());

            if (context.Request.Query.ContainsKey("dataId") &&
                context.Request.Query.ContainsKey("endpoint"))
            {
                var grupo = GetGrupoFromHttpContext(context);
                await base.Groups.AddToGroupAsync(base.Context.ConnectionId, grupo);
            }

            await base.OnConnectedAsync();

            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var context = GetContext(base.Context.GetHttpContext());

            if (context.Request.Query.ContainsKey("dataId") &&
              context.Request.Query.ContainsKey("endpoint"))
            {
                var grupo = GetGrupoFromHttpContext(context);
                await base.Groups.RemoveFromGroupAsync(base.Context.ConnectionId, grupo);
            }

            await base.OnDisconnectedAsync(exception);

            _logger.LogInformation($"Client disconnected: {base.Context.ConnectionId}");
        }


        private string GetGrupoFromHttpContext(HttpContext cContext)
        {
            var context = GetContext(cContext);
            try
            {
                string? dataId = context.Request.Query["dataId"];
                string? endpoint = context.Request.Query["endpoint"];
                return endpoint?.ToUpper() + "|" + dataId?.ToUpper();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on HandleHttpContext");
                throw;
            }
        }

        public async Task SendNotificationToGroup(string grupo, string message)
        {
            await Clients.Group(grupo).SendAsync("addFullMessage", message);
        }
    }
}