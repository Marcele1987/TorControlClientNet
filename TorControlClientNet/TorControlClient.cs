using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TorControlClientNet.Constants;
using TorControlClientNet.Entities;
using TorControlClientNet.Helper;
using TorControlClientNet.Interfaces;

namespace TorControlClientNet
{
    public class TorControlClient : ITorControlClient
    {
        #region Fields

        private readonly string _ip;
        private readonly int _port;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isAuthenticating;
        private bool _isListenerRunning;
        private NetworkStream _networkStream;
        private TcpClient _tcpClient;

        #endregion

        #region Constructors

        public TorControlClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        #endregion

        #region Properties

        public bool IsAuthenticated { get; private set; }

        #endregion

        #region  Interface Implementations

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnBadAuthentication;
        public event EventHandler OnSuccessfullAuthentication;
        public event EventHandler OnCommandOk;
        public event EventHandler OnAsyncEvent;
        public event EventHandler OnCommandData;

        public void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_ip, _port);

            _networkStream = _tcpClient.GetStream();

            OnConnect?.Invoke(this, new EventArgs());
        }

        public void Disconnect()
        {
            if (_tcpClient?.Connected ?? false)
            {
                var data = Encoding.ASCII.GetBytes("QUIT" + Environment.NewLine);
                if (_networkStream.CanWrite)
                    _networkStream.Write(data, 0, data.Length);
                else
                    throw new Exception("NetworkStream closed");

                _cancellationTokenSource.Cancel();
                _tcpClient.Close();

                _isListenerRunning = false;
                IsAuthenticated = false;
            }

            OnDisconnect?.Invoke(this, new EventArgs());
        }

        public void Authenticate(string password = "")
        {
            StartListener();

            _isAuthenticating = true;
            var data = Encoding.ASCII.GetBytes(string.IsNullOrEmpty(password) ? TorCommands.AUTHENTICATE + Environment.NewLine : $"{TorCommands.AUTHENTICATE.ToString()} \"{password}\"" + Environment.NewLine);
            WriteToStream(data);
        }

        public void SendCommand(TorCommands command, string keyword = "")
        {
            var data = Encoding.ASCII.GetBytes($"{command.ToString()} {keyword}" + Environment.NewLine);
            WriteToStream(data);
        }

        #endregion

        #region Members

        private void WriteToStream(byte[] data)
        {
            if (!_tcpClient?.Connected ?? false) throw new Exception("Client is not connected. Call method connect first.");

            if (_networkStream.CanWrite)
                _networkStream.Write(data, 0, data.Length);
            else
                throw new Exception("NetworkStream closed");
        }

        private void StartListener()
        {
            if (_isListenerRunning)
                return;

            if (!_tcpClient?.Connected ?? false) throw new Exception("Client is not connected. Call method connect first.");

            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => ReadStream(_cancellationTokenSource.Token));

            _isListenerRunning = true;
        }

        private void ReadStream(CancellationToken token)
        {
            try
            {
                using (var reader = new StreamReader(_networkStream, Encoding.UTF8))
                {
                    var isMultiline = false;
                    var keyword = "";
                    var values = new List<string>();

                    string line;
                    while ((line = reader.ReadLine()) != null && !token.IsCancellationRequested)
                        try
                        {
                            if (line.CheckStatusCode(TorStatusCodes.Tor_Ok))
                            {
                                if (_isAuthenticating)
                                {
                                    try
                                    {
                                        OnSuccessfullAuthentication?.Invoke(this, new EventArgs());
                                    }
                                    catch (Exception exc)
                                    {
                                        Debug.WriteLine(exc.Message);
                                    }

                                    _isAuthenticating = false;
                                    IsAuthenticated = true;
                                }
                                else if (line.Equals(TorStatusCodes.Tor_Ok + " OK"))
                                {
                                    OnCommandOk?.Invoke(this, new TorEventArgs {EventName = "Ok"});
                                }
                                else
                                {
                                    GetValues(ref isMultiline, ref keyword, ref values, line);
                                }
                            }
                            else if (line.CheckStatusCode(TorStatusCodes.Tor_BadAuthentication))
                            {
                                OnBadAuthentication?.Invoke(this, new EventArgs());
                                _isAuthenticating = false;
                            }
                            else if (line.CheckStatusCode(TorStatusCodes.Tor_AsynchronousEvent))
                            {
                                OnAsyncEvent?.Invoke(this, new TorEventArgs
                                {
                                    EventName = line,
                                    Values = new List<string>()
                                });
                            }
                            else
                            {
                                GetValues(ref isMultiline, ref keyword, ref values, line);
                            }
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine(exc.Message);
                        }
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                _isListenerRunning = false;
            }
        }

        private void GetValues(ref bool isMultiline, ref string keyword, ref List<string> values, string line)
        {
            if (isMultiline)
            {
                if (line == ".")
                {
                    isMultiline = false;
                    OnCommandData?.Invoke(this, new TorEventArgs
                    {
                        EventName = keyword,
                        Values = values
                    });

                    values = new List<string>();
                }
                else
                {
                    values.Add(line);
                }
            }
            else
            {
                //parse content
                var divider = line.Substring(3, 1);
                if (divider == "-")
                {
                    //single line
                    keyword = ParseResponseSingle(values, line);
                }
                else if (divider == " ")
                {
                    //end line
                    //single line
                    keyword = ParseResponseSingle(values, line);
                }
                else if (divider == "+")
                {
                    //multiline
                    isMultiline = true;
                    keyword = ParseResponseSingle(values, line);
                }

                if (!isMultiline)
                {
                    OnCommandData?.Invoke(this, new TorEventArgs
                    {
                        EventName = keyword,
                        Values = values
                    });

                    values = new List<string>();
                }
            }
        }

        private static string ParseResponseSingle(List<string> values, string line)
        {
            var content = line.Substring(4, line.Length - 4);
            var response = content.Split('=');

            if (!string.IsNullOrEmpty(response[1]))
                values.Add(response[1]);

            return response[0];
        }

        #endregion
    }
}