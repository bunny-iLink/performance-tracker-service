using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CpuMetrics
{
    public class CpuMetrics
    {
        public double UsagePercent;
    }

    public class CpuMetricsClient
    {
        public CpuMetrics GetCpuMetrics()
        {
            if (IsUnix())
            {
                return GetUnixCpuMetrics();
            }

            return GetWindowsCpuMetrics();
        }

        private bool IsUnix()
        {
            var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
                         RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            return isUnix;
        }

        private CpuMetrics GetWindowsCpuMetrics()
        {
            var info = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C typeperf \"\\Processor(_Total)\\% Processor Time\" -sc 1",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(info);
            if (process == null)
                throw new InvalidOperationException("Failed to start process.");

            var output = process.StandardOutput.ReadToEnd();
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length >= 3)
            {
                var usageString = lines[2].Split(',')[1].Replace("\"", "").Trim();
                if (double.TryParse(usageString, out var usage))
                {
                    return new CpuMetrics { UsagePercent = Math.Round(usage, 2) };
                }
            }

            throw new Exception("Failed to parse CPU usage.");
        }


        private CpuMetrics GetUnixCpuMetrics()
        {
            var info = new ProcessStartInfo("/bin/bash")
            {
                Arguments = "-c \"top -bn1 | grep '%Cpu' || top -l 1 | grep 'CPU usage'\"",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            using var process = Process.Start(info);
            if (process == null)
                throw new InvalidOperationException("Failed to start process.");

            var output = process.StandardOutput.ReadToEnd();

            double usage = 0.0;

            if (output.Contains("Cpu(s):")) // Linux format
            {
                var idlePart = output.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .FirstOrDefault(x => x.Contains("id"));
                if (!string.IsNullOrEmpty(idlePart))
                {
                    var idleStr = new string(idlePart.Where(c => char.IsDigit(c) || c == '.').ToArray());
                    if (double.TryParse(idleStr, out var idle))
                    {
                        usage = 100 - idle;
                    }
                }
            }
            else if (output.Contains("CPU usage:")) // macOS format
            {
                var idlePart = output.Split(',').FirstOrDefault(x => x.Contains("idle"));
                if (!string.IsNullOrEmpty(idlePart))
                {
                    var idleStr = new string(idlePart.Where(c => char.IsDigit(c) || c == '.').ToArray());
                    if (double.TryParse(idleStr, out var idle))
                    {
                        usage = 100 - idle;
                    }
                }
            }

            return new CpuMetrics { UsagePercent = Math.Round(usage, 2) };
        }
    }
}