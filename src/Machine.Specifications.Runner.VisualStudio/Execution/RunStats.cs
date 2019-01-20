using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Machine.VSTestAdapter.Execution
{
    public class RunStats
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public DateTimeOffset Start { get; private set; }

        public DateTimeOffset End { get; private set; }

        public TimeSpan Duration
        {
            get
            {
                return this.stopwatch.Elapsed;
            }
        }

        public RunStats()
        {
            this.stopwatch.Start();
            this.Start = DateTime.Now;
        }

        public void Stop()
        {
            this.stopwatch.Stop();
            End = Start + stopwatch.Elapsed;
        }
    }
}
