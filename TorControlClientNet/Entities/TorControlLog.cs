using System;

namespace TorControlClientNet.Entities
{
    public class TorControlLog
    {
        public string EventName { get; set; }

        public string Value { get; set; }

        public double ValueNum { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}