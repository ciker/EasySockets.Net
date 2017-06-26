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
        public IPEndPoint LocalIPEP { get; }
        public ushort BufferSize { get; }
        public IPEndPoint[] Connections { get; }

        private ValueWrapper<bool> _isListening;
        private Socket _socket;
        private Thread _thread;

        public void Start(IPEndPoint localIPEP, ushort bufferSize = 512)
        {
        }

        public void Send(IPEndPoint remoteIPEP, byte[] data)
        {
        }

        public void Disconnect(IPEndPoint remoteIPEP)
        {
        }

        public void Stop()
        {
        }

        private void _Cleanup()
        {
        }

        private void _ListenThread()
        {
        }

        private void _ReceiveThread()
        {
        }
    }
}

/*
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MFatihMAR.EasySockets
{
    public class TcpServer
    {
        private class Client
        {
            public delegate void ReceiveHandler(IPEndPoint ipep, byte[] data);

            private ValueWrapper<bool> _isListening;

            public bool IsListening => _isListening.Value;
            public Socket Socket { get; private set; }
            public Thread Thread { get; private set; }

            public IPEndPoint RemoteIPEP { get; private set; }

            public byte[] Buffer { get; private set; }

            private ReceiveHandler _receiveHandler;

            public Client(ValueWrapper<bool> isListening, Socket socket, ParameterizedThreadStart threadFunction, int bufferSize, ReceiveHandler receiveHandler)
            {
                _isListening = isListening;

                Buffer = new byte[bufferSize];
                _receiveHandler = receiveHandler;

                Socket = socket;
                RemoteIPEP = (IPEndPoint)socket.RemoteEndPoint;

                Thread = new Thread(threadFunction);
                Thread.Start(this);
            }

            public void Close()
            {
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

        private ValueWrapper<bool> _isListening = new ValueWrapper<bool>(false);
        public bool IsListening => _isListening.Value;
        public IPEndPoint LocalIPEP { get; private set; }
        public int BufferSize { get; private set; }

        private Socket _socket;
        private Thread _thread;
        private Dictionary<IPEndPoint, Client> _connections;
        private Dictionary<IPEndPoint, Client> _connectionsCache;

        public void Start(IPEndPoint localIPEP, int bufferSize = 512)
        {
            _Cleanup();

            LocalIPEP = localIPEP ?? throw new ArgumentNullException(nameof(localIPEP));
            BufferSize = bufferSize;

            _isListening = new ValueWrapper<bool>(true);

            _connections = new Dictionary<IPEndPoint, Client>();
            _connectionsCache = new Dictionary<IPEndPoint, Client>();

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(localIPEP);
            _socket.Listen(64);

            _thread = new Thread(_ServerThread);
            _thread.Start();

            LocalIPEP = (IPEndPoint)_socket.LocalEndPoint;

            OnStart?.Invoke();
        }

        public void Send(IPEndPoint ipep, byte[] data)
        {
            if (!_connectionsCache.ContainsKey(ipep))
            {
                return;
            }

            _connectionsCache[ipep].Socket.Send(data);
        }

        public void Disconnect(IPEndPoint ipep)
        {
            if (!_connectionsCache.ContainsKey(ipep))
            {
                return;
            }

            _connectionsCache[ipep].Socket.Close();
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

                foreach (var conn in _connectionsCache)
                {
                    conn.Value.Close();
                }
            }
            catch
            {
            }
        }

        private void _ServerThread()
        {
            try
            {
                var isListening = _isListening;

                while (isListening.Value)
                {
                    var clientSocket = _socket.Accept();
                    lock (_connections)
                    {
                        _connections.Add(
                            (IPEndPoint)clientSocket.RemoteEndPoint,
                            new Client(isListening, clientSocket, _ClientThread, BufferSize, (ipep, data) => OnData?.Invoke(ipep, data)));
                        _connectionsCache = new Dictionary<IPEndPoint, Client>(_connections);
                    }
                    OnConnect?.Invoke((IPEndPoint)clientSocket.RemoteEndPoint);
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

        private void _ClientThread(object source)
        {
            if (!(source is Client))
            {
                return;
            }

            var client = (Client)source;

            try
            {
                while (client.IsListening)
                {
                    var recSize = client.Socket.Receive(client.Buffer, 0, BufferSize, SocketFlags.None);
                    var packet = new byte[recSize];
                    Array.Copy(client.Buffer, packet, recSize);
                    OnData?.Invoke(client.RemoteIPEP, packet);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                client.Close();
                OnDisconnect?.Invoke(client.RemoteIPEP, ex);
            }
        }
    }
}
*/
