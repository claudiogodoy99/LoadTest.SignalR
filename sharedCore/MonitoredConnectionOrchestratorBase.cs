namespace sharedCore;

public abstract class MonitoredConnectionOrchestratorBase<TInitializer> : ConnectionOrchestratorBase<TInitializer>
    where TInitializer : InitializerBase
{
    protected readonly MessageAnalyticsBase _messageAnalytics;

    public MonitoredConnectionOrchestratorBase(TInitializer initializer,
        string group,
        CancellationTokenSource cancellationToken,
        MessageAnalyticsBase messageAnalytics)
        : base(initializer, group, cancellationToken)
    {
        _messageAnalytics = messageAnalytics;
    }
}