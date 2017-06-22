using System.Net;

namespace MFatihMAR.EasySockets
{
    public class UdpPeer
    {
        public enum Error
        {
            
        }

        public class Config
        {
            public Config(ushort port)
            {
            }
        }

        public delegate void StartEvent();
        public delegate void DataEvent();
        public delegate void StopEvent();
        public delegate void ErrorEvent();

        public event StartEvent OnStart;
        public event DataEvent OnData;
        public event StopEvent OnStop;
        public event ErrorEvent OnError;

        public bool IsOpen { get; private set; }
        public Config Configuration { get; private set; }

        public void Start(Config config)
        {
        }

        public void Send(IPEndPoint ipep, byte[] data)
        {
        }

        public void Stop()
        {
        }
    }
}
