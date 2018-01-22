using System;
using TorControlClientNet.Constants;

namespace TorControlClientNet.Interfaces
{
    public interface ITorControlClient
    {
        #region Members

        event EventHandler OnConnect;
        event EventHandler OnDisconnect;
        event EventHandler OnBadAuthentication;
        event EventHandler OnSuccessfullAuthentication;
        event EventHandler OnCommandOk;
        event EventHandler OnAsyncEvent;
        event EventHandler OnCommandData;

        void Connect();

        void Disconnect();

        void SendCommand(TorCommands command, string keyword);

        void Authenticate(string password);

        #endregion
    }
}