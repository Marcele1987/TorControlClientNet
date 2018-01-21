using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorControlClientNet
{
    public class TorControlLog
    {
        public string EventName { get; set; }

        public string Value { get; set; }

        public double ValueNum { get; set; }

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
