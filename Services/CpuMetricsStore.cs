public class CPUMetricsStore
{
    private readonly Queue<double> _cpuUsageQueue = new();
    private readonly object _lock = new();
    private const int MaxQueueSize = 60;

    public void AddCpuUsage(double value)
    {
        lock (_lock)
        {
            if (_cpuUsageQueue.Count >= MaxQueueSize)
            {
                _cpuUsageQueue.Dequeue();
            }

            _cpuUsageQueue.Enqueue(value);
        }
    }

    public IReadOnlyCollection<double> GetMetricsCpu()
    {
        lock (_lock)
        {
            return _cpuUsageQueue.ToList();
        }
    }
}