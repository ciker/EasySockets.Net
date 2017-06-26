using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MFatihMAR.EasySockets
{
    public class TcpServer
    {
        private class _Client
        {
            private bool _isClosed;
            private ValueWrapper<bool> _isListening;
            public bool IsListening => !_isClosed && (_isListening?.Value ?? false);

            public Socket Socket { get; }
            public Thread Thread { get; }

            public ushort BufferSize { get; }
            public byte[] Buffer { get; }

            public _Client(ValueWrapper<bool> isListening, Socket socket, ushort bufferSize, ParameterizedThreadStart threadFunction)
            {
                _isListening = isListening;

                BufferSize = bufferSize;
                Buffer = new byte[bufferSize];

                Socket = socket;

                Thread = new Thread(threadFunction);
                Thread.Start(this);
            }

            public void Close()
            {
                try
                {
                    _isClosed = true;

                    Socket?.Close();

                    if (Thread != null && Thread.IsAlive)
                    {
                        Thread.Abort();
                    }
                }
                catch
                {
                }
            }
        }

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
        private Dictionary<IPEndPoint, _Client> _connections;
        private Dictionary<IPEndPoint, _Client> _connectionsCached;

        public void Start(IPEndPoint localIPEP, ushort bufferSize = 512)
        {
            _Cleanup();

            LocalIPEP = localIPEP ?? throw new ArgumentNullException(nameof(localIPEP));
            BufferSize = bufferSize < 64 ? throw new ArgumentOutOfRangeException(nameof(bufferSize)) : bufferSize;

            _isListening = new ValueWrapper<bool>(true);

            _connections = new Dictionary<IPEndPoint, _Client>();
            _connectionsCached = new Dictionary<IPEndPoint, _Client>();

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

            if (remoteIPEP == null)
            {
                throw new ArgumentNullException(nameof(remoteIPEP));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length < 1 || data.Length > BufferSize)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            if (!_connectionsCached.ContainsKey(remoteIPEP))
            {
                return;
            }

            _connectionsCached[remoteIPEP].Socket.Send(data);
        }

        public void Disconnect(IPEndPoint remoteIPEP)
        {
            if (!_connectionsCached.ContainsKey(remoteIPEP))
            {
                return;
            }

            _connectionsCached[remoteIPEP].Close();
            OnDisconnect?.Invoke(remoteIPEP);
        }

        public void Stop()
        {
            _Cleanup();
            OnStop?.Invoke();
        }

        private void _Cleanup()
        {
            try
            {
                _isListening.Value = false;

                _socket?.Close();

                if (_thread != null && _thread.IsAlive)
                {
                    _thread.Abort();
                }

                foreach (var conn in _connectionsCached)
                {
                    conn.Value.Close();
                }
            }
            catch
            {
            }
        }

        private void _ListenThread()
        {
            try
            {
                var isListening = _isListening;
                while (isListening.Value)
                {
                    var clientSocket = _socket.Accept();
                    var clientIPEP = (IPEndPoint)clientSocket.RemoteEndPoint;

                    lock (_connections)
                    {
                        if (_connections.ContainsKey(clientIPEP))
                        {
                            continue;
                        }

                        var client = new _Client(isListening, clientSocket, BufferSize, _ReceiveThread);

                        _connections.Add(clientIPEP, client);
                        _connectionsCached.Add(clientIPEP, client);
                    }

                    OnConnect?.Invoke(clientIPEP);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                _Cleanup();
                OnStop?.Invoke(ex);
            }
        }

        private void _ReceiveThread(object source)
        {
            if (!(source is _Client))
            {
                return;
            }

            var client = (_Client)source;
            var clientIPEP = (IPEndPoint)client.Socket.RemoteEndPoint;

            try
            {
                while (client.IsListening)
                {
                    var recSize = client.Socket.Receive(client.Buffer, 0, client.BufferSize, SocketFlags.None);
                    var packet = new byte[recSize];
                    Array.Copy(client.Buffer, packet, recSize);
                    OnData?.Invoke(clientIPEP, packet);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                client.Close();
                OnDisconnect?.Invoke(clientIPEP, ex);
            }
        }
    }
}
