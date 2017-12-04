using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorControlClientNet
{
    public class TorEventArgs : EventArgs
    {
        public string EventName { get; set; }

        public List<string> Values { get; set; }
    }
}
