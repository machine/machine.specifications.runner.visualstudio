using System;
using System.Diagnostics;

namespace Machine.Specifications.Runner.VisualStudio.Execution
{
    public class RunStats
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public DateTimeOffset Start { get; }

        public DateTimeOffset End { get; private set; }

        public TimeSpan Duration => stopwatch.Elapsed;

        public RunStats()
        {
            stopwatch.Start();
            Start = DateTime.Now;
        }

        public void Stop()
        {
            stopwatch.Stop();
            End = Start + stopwatch.Elapsed;
        }
    }
}
