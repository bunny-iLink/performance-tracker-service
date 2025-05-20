using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemoryMetricsController : ControllerBase
    {
        private readonly MemoryMetricsStore _store;

        public MemoryMetricsController(MemoryMetricsStore store)
        {
            _store = store;
        }

        [HttpGet("usedmemory")]
        public IActionResult GetMemoryUsage()
        {
            var metrics = _store.GetMetricsUsed();
            return Ok(metrics);
        }

        [HttpGet("freememory")]
        public IActionResult GetFreeMemory()
        {
            var metrics = _store.GetMetricsFree();
            return Ok(metrics);
        }
    }
}