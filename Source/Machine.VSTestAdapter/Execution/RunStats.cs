using System;

namespace Machine.VSTestAdapter.Execution
{
    public class RunStats
    {
        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; private set; }

        public TimeSpan Duration
        {
            get
            {
                return this.EndTime - this.StartTime;
            }
        }

        public RunStats()
        {
            this.StartTime = DateTime.Now;
        }

        public void Stop()
        {
            this.EndTime = DateTime.Now;
        }
    }
}