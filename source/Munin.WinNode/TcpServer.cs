﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Munin.WinNode
{
    class TcpServer : IDisposable
    {
        readonly TcpListener _listener;
        readonly Thread _thread;

        public TcpServer()
        {
            _listener = new TcpListener(IPAddress.Any, 4949);
            _thread = new Thread(ListenForClient);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            _thread.Abort();
            _listener.Stop();
        }

        private void ListenForClient()
        {
            Trace.WriteLine("Waiting for connection...");
            this._listener.Start();
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                var clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private void HandleClient(object client)
        {
            var tcpClient = (TcpClient) client;
            Trace.WriteLine(string.Format("Received connection from {0}", tcpClient.Client.RemoteEndPoint));

            var data = new byte[tcpClient.ReceiveBufferSize];
            var dataString = new StringBuilder();
            using (var stream = tcpClient.GetStream())
            {
                WelcomeMessage(stream);

                int readCount;
                while ((readCount = stream.Read(data, 0, tcpClient.ReceiveBufferSize)) != 0)
                {
                    dataString.Append(Encoding.ASCII.GetString(data, 0, readCount));
                    if (Regex.IsMatch(dataString.ToString(), @"\r?\n"))
                    {
                        string message = NormalizeMessage(dataString.ToString());
                            
                        if (message == "QUIT")
                            break;

                        MessageHandler(stream, message);
                        dataString.Clear();
                    }
                }

                Trace.WriteLine(string.Format("Closing connection from {0}", tcpClient.Client.RemoteEndPoint));
            }

            tcpClient.Close();
        }

        string NormalizeMessage(string message)
        {
            message = message.ToUpper();
            message = Regex.Replace(message, @"\r?\n", string.Empty);
            return message;
        }

        void WelcomeMessage(Stream stream)
        {
            var welcomeMessage = Encoding.ASCII.GetBytes(string.Format("# Connected to Munin.WinNode on {0}{1}", Dns.GetHostName(), Environment.NewLine));
            stream.Write(welcomeMessage, 0, welcomeMessage.Length);
        }

        static void MessageHandler(NetworkStream stream, string message)
        {
            var messageParts = MessageParts.FromString(message);
            var command = CommandManager.CommandFromName(messageParts.Command);
            
            string response = null;
            command.Execute(messageParts.Arguments, out response);
            
            if (! string.IsNullOrEmpty(response))
            {
                var responseBytes = Encoding.ASCII.GetBytes(response + Environment.NewLine);
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
        }
    }
}