using MemoryMetrics;

public class MemoryMetricsCollector : BackgroundService
{
    private readonly MemoryMetricsStore _store;
    private readonly ILogger<MemoryMetricsCollector> _logger;


    public MemoryMetricsCollector(MemoryMetricsStore store, ILogger<MemoryMetricsCollector> logger)
    {
        _store = store;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = new MemoryMetricsClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            var memoryMetrics = client.GetMemoryMetrics();
            _store.AddUsed(memoryMetrics.Used);
            _store.AddFree(memoryMetrics.Free);
            _logger.LogInformation("Collected memory usage: {Used}", memoryMetrics.Used);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}


