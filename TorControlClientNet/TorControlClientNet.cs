using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TorControlClientNet
{
    public class TorControlClientNet : ITorControlClientNet
    {
        private string _ip;
        private int _port;
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private CancellationTokenSource _cancellationTokenSource;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnBadAuthentication;
        public event EventHandler OnSuccessfullAuthentication;

        public TorControlClientNet(string ip,int port)
        {
            _ip = ip;
            _port = port;
        }

        public void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_ip, _port);

            _networkStream = _tcpClient.GetStream();

            OnConnect?.Invoke(this, new EventArgs());
        }

        public void Disconnect()
        {
            if (_tcpClient?.Connected ?? false && _cancellationTokenSource != null)
            {
                var data = System.Text.Encoding.ASCII.GetBytes("QUIT" + Environment.NewLine);
                if (_networkStream.CanWrite)
                {
                    _networkStream.Write(data, 0, data.Length);
                }
                else
                {
                    throw new Exception("NetworkStream closed");
                }

                _cancellationTokenSource.Cancel();
                _tcpClient.Close();
            }

            OnDisconnect?.Invoke(this, new EventArgs());
        }

 
        private bool isAuthenticating;

        public void StartListener(string password = "")
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => this.Read(_cancellationTokenSource.Token));

            var data = System.Text.Encoding.ASCII.GetBytes(string.IsNullOrEmpty(password) ? "AUTHENTICATE" : $"AUTHENTICATE \"{password}\"" + Environment.NewLine);
            if (_networkStream.CanWrite)
            {
                isAuthenticating = true;
                _networkStream.Write(data, 0, data.Length);
            }
            else
            {
                throw new Exception("NetworkStream closed");
            }
        }

        private void Read(CancellationToken token)
        {
            using (var reader = new StreamReader(_networkStream, Encoding.UTF8))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null && !token.IsCancellationRequested)
                {
                    if (line.Contains("250"))
                    {
                        if(isAuthenticating)
                        {
                            OnSuccessfullAuthentication?.Invoke(this, new EventArgs());
                            isAuthenticating = false;
                        }

                    }
                    else if(line.Contains("515 Bad authentication"))
                    {
                        OnBadAuthentication?.Invoke(this, new EventArgs());
                        isAuthenticating = false;
                    }
                }
            }
        }
    }
}
