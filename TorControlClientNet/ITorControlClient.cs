using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorControlClientNet
{
    public interface ITorControlClient
    {
        event EventHandler OnConnect;
        event EventHandler OnDisconnect;
        event EventHandler OnBadAuthentication;
        event EventHandler OnSuccessfullAuthentication;
        event EventHandler OnCommandOk;
        event EventHandler OnAsyncEvent;
        event EventHandler OnCommandData;

        void Connect();

        void StartListener(string password);

        void Disconnect();

        void SendCommand(TorCommands command, string keyword);
    }
}
