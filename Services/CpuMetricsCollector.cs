using CpuMetrics;

public class CPUMetricsCollector : BackgroundService
{
    private readonly CPUMetricsStore _store;
    private readonly ILogger<CPUMetricsCollector> _logger;
    public CPUMetricsCollector(CPUMetricsStore store, ILogger<CPUMetricsCollector> logger)
    {
        _store = store;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = new CpuMetricsClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            var CPUMetrics = client.GetCpuMetrics();
            _store.AddCpuUsage(CPUMetrics.UsagePercent);
            _logger.LogInformation("Collected memory usage: {Used}", CPUMetrics.UsagePercent);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}