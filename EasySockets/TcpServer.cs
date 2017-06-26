using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MFatihMAR.EasySockets
{
    public class TcpServer
    {
        public delegate void StartEvent();
        public delegate void ConnectEvent(IPEndPoint remoteIPEP);
        public delegate void DataEvent(IPEndPoint remoteIPEP, byte[] data);
        public delegate void DisconnectEvent(IPEndPoint remoteIPEP, Exception exception = null);
        public delegate void StopEvent(Exception exception = null);

        public event StartEvent OnStart;
        public event ConnectEvent OnConnect;
        public event DataEvent OnData;
        public event DisconnectEvent OnDisconnect;
        public event StopEvent OnStop;

        public bool IsListening => _isListening?.Value ?? false;
        public IPEndPoint LocalIPEP { get; private set; }
        public ushort BufferSize { get; private set; }
        public IPEndPoint[] Connections { get; }

        private ValueWrapper<bool> _isListening;
        private Socket _socket;
        private Thread _thread;

        public void Start(IPEndPoint localIPEP, ushort bufferSize = 512)
        {
            _Cleanup();

            LocalIPEP = localIPEP ?? throw new ArgumentNullException(nameof(localIPEP));
            BufferSize = bufferSize < 64 ? throw new ArgumentOutOfRangeException(nameof(bufferSize)) : bufferSize;

            _isListening = new ValueWrapper<bool>(true);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(localIPEP);
            _socket.Listen(64);

            _thread = new Thread(_ListenThread);
            _thread.Start();

            LocalIPEP = (IPEndPoint)_socket.LocalEndPoint;

            OnStart?.Invoke();
        }

        public void Send(IPEndPoint remoteIPEP, byte[] data)
        {
            if (!IsListening)
            {
                return;
            }
            
            // TODO
        }

        public void Disconnect(IPEndPoint remoteIPEP)
        {
            // TODO
        }

        public void Stop()
        {
            _Cleanup();
            OnStop?.Invoke();
        }

        private void _Cleanup()
        {
            // TODO
        }

        private void _ListenThread()
        {
            // TODO
        }

        private void _ReceiveThread()
        {
            // TODO
        }
    }
}
