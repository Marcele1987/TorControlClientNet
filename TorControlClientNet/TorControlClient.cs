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
    public class TorControlClient : ITorControlClient
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
        public event EventHandler OnCommandOk;
        public event EventHandler OnAsyncEvent;
        public event EventHandler OnCommandData;

        public TorControlClient(string ip,int port)
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

            isAuthenticating = true;
            var data = Encoding.ASCII.GetBytes(string.IsNullOrEmpty(password) ? TorCommands.AUTHENTICATE.ToString() : $"{TorCommands.AUTHENTICATE.ToString()} \"{password}\"" + Environment.NewLine);
            writeToStream(data);
        }

        private void Read(CancellationToken token)
        {
            using (var reader = new StreamReader(_networkStream, Encoding.UTF8))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null && !token.IsCancellationRequested)
                {
                    if (line.Contains(TorStatusCodes.Tor_Ok))
                    {
                        if(isAuthenticating)
                        {
                            OnSuccessfullAuthentication?.Invoke(this, new EventArgs());
                            isAuthenticating = false;
                        }
                        else if(line.Contains("OK"))
                        {
                            OnCommandOk?.Invoke(this, new EventArgs());
                        }
                        else
                        {
                            OnCommandData?.Invoke(this, new TorEventArgs()
                            {
                                EventName = line,
                                Values = new Dictionary<string, string>()
                            });
                        }

                    }
                    else if(line.Contains(TorStatusCodes.Tor_BadAuthentication))
                    {
                        OnBadAuthentication?.Invoke(this, new EventArgs());
                        isAuthenticating = false;
                    }
                    else if(line.Contains(TorStatusCodes.Tor_AsynchronousEvent))
                    {
                        OnAsyncEvent?.Invoke(this, new TorEventArgs()
                        {
                            EventName = line,
                            Values = new Dictionary<string, string>()
                        });
                    }
                }
            }
        }

        public void SendCommand(TorCommands command, string keyword = "")
        {
            var data = Encoding.ASCII.GetBytes($"{command.ToString()} {keyword}" + Environment.NewLine);
            writeToStream(data);
        }

        private void writeToStream(byte[] data)
        {
            if (_networkStream.CanWrite)
            {
                _networkStream.Write(data, 0, data.Length);
            }
            else
            {
                throw new Exception("NetworkStream closed");
            }
        }
    }
}
