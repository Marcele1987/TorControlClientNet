using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorControlClientNet
{
    public interface ITorControlClientNet
    {
        event EventHandler OnConnect;
        event EventHandler OnDisconnect;
        event EventHandler OnBadAuthentication;
        event EventHandler OnSuccessfullAuthentication;

        void Connect();

        void StartListener(string password);

        void Disconnect();
    }
}
