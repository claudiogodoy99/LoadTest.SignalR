namespace sharedCore;

public abstract class MonitoredConnectionOrchestratorBase<TInitializer> : ConnectionOrchestratorBase<TInitializer>
    where TInitializer : InitializerBase
{
    protected readonly MessageAnalyticsBase _messageAnalytics;

    public MonitoredConnectionOrchestratorBase(TInitializer initializer,
        string group,
        MessageAnalyticsBase messageAnalytics)
        : base(initializer, group)
    {
        _messageAnalytics = messageAnalytics;
    }

    public override Task RunAsync(CancellationToken token)
    {
        return Task.WhenAll(
            _messageAnalytics.RunAsync(token),
            base.RunAsync(token));
    }
}