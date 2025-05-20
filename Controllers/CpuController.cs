using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CPUMetricsController : ControllerBase
    {
        private readonly CPUMetricsStore _cpuStore;

        public CPUMetricsController(CPUMetricsStore cpuStore)
        {
            _cpuStore = cpuStore;
        }

        [HttpGet("cpuusage")]
        public IActionResult GetCpuMetrics()
        {
            var metrics = _cpuStore.GetMetricsCpu();
            return Ok(metrics);
        }
    }
}