using System;
using System.Collections.Generic;

namespace TorControlClientNet.Entities
{
    public class TorEventArgs : EventArgs
    {
        public string EventName { get; set; }

        public List<string> Values { get; set; }
    }
}
