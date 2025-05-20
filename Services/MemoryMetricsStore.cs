public class MemoryMetricsStore
{
    private readonly Queue<double> _usedMemoryQueue = new();
    private readonly Queue<double> _freeMemoryQueue = new();
    private readonly object _lock = new();
    private const int MaxQueueSize = 60;

    public void AddUsed(double value)
    {
        lock (_lock)
        {
            if (_usedMemoryQueue.Count >= MaxQueueSize)
            {
                _usedMemoryQueue.Dequeue();
            }

            _usedMemoryQueue.Enqueue(value);
        }
    }

    public void AddFree(double value)
    {
        lock (_lock)
        {
            if (_freeMemoryQueue.Count >= MaxQueueSize)
            {
                _freeMemoryQueue.Dequeue();
            }

            _freeMemoryQueue.Enqueue(value);
        }
    }

    public IReadOnlyCollection<double> GetMetricsUsed()
    {
        lock (_lock)
        {
            return _usedMemoryQueue.ToList();
        }
    }

    public IReadOnlyCollection<double> GetMetricsFree()
    {
        lock (_lock)
        {
            return _freeMemoryQueue.ToList();
        }
    }
}

