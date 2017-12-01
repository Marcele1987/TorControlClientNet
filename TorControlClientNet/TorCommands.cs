using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorControlClientNet
{
    public enum TorCommands
    {
        AUTHENTICATE,
        GETCONF,
        GETINFO,
        SETEVENTS,
        QUIT
    }
}
