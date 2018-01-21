﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private bool _isAuthenticating;
        private bool _isAuthenticated;
        private bool _isListenerRunning;

        public event EventHandler OnConnect;
        public event EventHandler OnDisconnect;
        public event EventHandler OnBadAuthentication;
        public event EventHandler OnSuccessfullAuthentication;
        public event EventHandler OnCommandOk;
        public event EventHandler OnAsyncEvent;
        public event EventHandler OnCommandData;

        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }

        public TorControlClient(string ip, int port)
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

                _isListenerRunning = false;
                _isAuthenticated = false;
            }

            OnDisconnect?.Invoke(this, new EventArgs());
        }

        public void Authenticate(string password = "")
        {
            startListener();

            _isAuthenticating = true;
            var data = Encoding.ASCII.GetBytes(string.IsNullOrEmpty(password) ? TorCommands.AUTHENTICATE.ToString() + Environment.NewLine : $"{TorCommands.AUTHENTICATE.ToString()} \"{password}\"" + Environment.NewLine);
            writeToStream(data);
        }

        public void SendCommand(TorCommands command, string keyword = "")
        {
            var data = Encoding.ASCII.GetBytes($"{command.ToString()} {keyword}" + Environment.NewLine);
            writeToStream(data);
        }

        private void writeToStream(byte[] data)
        {
            if (!_tcpClient?.Connected ?? false)
            {
                throw new Exception("Client is not connected. Call method connect first.");
            }

            if (_networkStream.CanWrite)
            {
                _networkStream.Write(data, 0, data.Length);
            }
            else
            {
                throw new Exception("NetworkStream closed");
            }
        }

        private void startListener()
        {
            if (_isListenerRunning)
                return;

            if (!_tcpClient?.Connected ?? false)
            {
                throw new Exception("Client is not connected. Call method connect first.");
            }

            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(() => readStream(_cancellationTokenSource.Token));

            _isListenerRunning = true;
        }

        private void readStream(CancellationToken token)
        {
            try
            {
                using (var reader = new StreamReader(_networkStream, Encoding.UTF8))
                {
                    var isMultiline = false;
                    var keyword = "";
                    var values = new List<string>();

                    string line = "";
                    while ((line = reader.ReadLine()) != null && !token.IsCancellationRequested)
                    {
                        try
                        {
                            if (line.checkStatusCode(TorStatusCodes.Tor_Ok))
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
                                    _isAuthenticated = true;
                                }
                                else if (line.Equals(TorStatusCodes.Tor_Ok + " OK"))
                                {
                                    OnCommandOk?.Invoke(this, new TorEventArgs() { EventName = "Ok" });
                                }
                                else
                                {
                                    GetValues(ref isMultiline, ref keyword, ref values, line);
                                }
                            }
                            else if (line.checkStatusCode(TorStatusCodes.Tor_BadAuthentication))
                            {
                                OnBadAuthentication?.Invoke(this, new EventArgs());
                                _isAuthenticating = false;
                            }
                            else if (line.checkStatusCode(TorStatusCodes.Tor_AsynchronousEvent))
                            {
                                OnAsyncEvent?.Invoke(this, new TorEventArgs()
                                {
                                    EventName = line,
                                    Values = new List<string>()
                                });
                            }
                            else
                                GetValues(ref isMultiline, ref keyword, ref values, line);
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine(exc.Message);
                        }
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
                    OnCommandData?.Invoke(this, new TorEventArgs()
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
                    keyword = parseResponseSingle(values, line);
                }
                else if (divider == " ")
                {
                    //end line
                    //single line
                    keyword = parseResponseSingle(values, line);
                }
                else if (divider == "+")
                {
                    //multiline
                    isMultiline = true;
                    keyword = parseResponseSingle(values, line);
                }

                if (!isMultiline)
                {
                    OnCommandData?.Invoke(this, new TorEventArgs()
                    {
                        EventName = keyword,
                        Values = values
                    });

                    values = new List<string>();
                }
            }
        }

        private static string parseResponseSingle(List<string> values, string line)
        {
            var content = line.Substring(4, line.Length - 4);
            var response = content.Split('=');

            if (!String.IsNullOrEmpty(response[1]))
                values.Add(response[1]);

            return response[0];
        }
    }
}
