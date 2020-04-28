
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace EFTService.Utils
{
    public class FileTransfer
    {
        #region Constants
        const int _portReceive = 2345;
        #endregion

        #region fields
        public string _serverIP = "127.0.0.1";
        Thread _listenThread;
        //public Label InfoLabel;
        int _flag = 0;
        string _receivedPath;
        public delegate void ReceiveDelegate();
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        string _configPath;
        #endregion

        public FileTransfer(bool listen, string ConfigPath)
        {
            if (listen)
            {
                _configPath = ConfigPath;
                _listenThread = new Thread(new ThreadStart(StartListening));
                _listenThread.Start();
            }
        }

        #region Receive file
        private void StartListening()
        {
#if DEBUG
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "EFTService";
                eventLog.WriteEntry("Starting to listen", EventLogEntryType.Information, 101, 1);
            }
#endif
            //byte[] bytes = new Byte[1024];
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, _portReceive);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(ipEnd);
                listener.Listen(32);
                while (true)
                {
                    allDone.Reset();
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "EFTService";
                    eventLog.WriteEntry("Error in Start Listening " + ex.Message, EventLogEntryType.Information, 101, 1);
                }
#endif
            }
        }
        public void AcceptCallback(IAsyncResult ar)
        {
#if DEBUG
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "EFTService";
                eventLog.WriteEntry("Accept Callback ", EventLogEntryType.Information, 101, 1);
            }
#endif
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
#if DEBUG
            using (EventLog eventLog = new EventLog("Application"))
            {
                eventLog.Source = "EFTService";
                eventLog.WriteEntry("Data Received From : " + IPAddress.Parse(((IPEndPoint)handler.RemoteEndPoint).Address.ToString()), EventLogEntryType.Information, 101, 1);
            }
#endif
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
            _flag = 0;
        }
        public void ReadCallback(IAsyncResult ar)
        {

            int fileNameLen = 1;
            String content = String.Empty;
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (_flag == 0)
                {
                    fileNameLen = BitConverter.ToInt32(state.buffer, 0);
                    string fileName = Encoding.UTF8.GetString(state.buffer, 4, fileNameLen);
                    _receivedPath = Helper.GetSavePath(_configPath, fileName); // TODO
                    _flag++;
                }
                if (_flag >= 1)
                {
                    BinaryWriter writer = new BinaryWriter(File.Open(_receivedPath, FileMode.Append));
                    if (_flag == 1)
                    {
                        writer.Write(state.buffer, 4 + fileNameLen, bytesRead - (4 + fileNameLen));
                        _flag++;
                    }
                    else
                        writer.Write(state.buffer, 0, bytesRead);
                    writer.Close();
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
            else
            {
                //InfoLabel.Invoke(new ReceiveDelegate(LabelWriter));
            }
        }
        public void LabelWriter()
        {
            //  InfoLabel.Text = "Data has been received";
        }
        #endregion

        public void Stop()
        {
            _listenThread.Abort();
        }
    }

    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;

        public const int BufferSize = 1024 * 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
    }
}
