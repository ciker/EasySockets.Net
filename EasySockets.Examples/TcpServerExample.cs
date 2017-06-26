using System;
using System.Net;
using System.Text;

namespace MFatihMAR.EasySockets.Examples
{
    public class TcpServerExample
    {
        private TcpServer _server;

        public void Run(ushort port)
        {
            _server = new TcpServer();

            _server.OnStart += _OnStart;
            _server.OnConnect += _OnConnect;
            _server.OnData += _OnData;
            _server.OnDisconnect += _OnDisconnect;
            _server.OnStop += _OnStop;

            var isAlive = true;
            while (isAlive)
            {
                var input = Console.ReadLine();
                var blocks = input.Split(' ');

                switch (blocks[0])
                {
                    default: Console.WriteLine("commands: start / send <ipep> <message> / disconnect <ipep> / stop / exit"); break;
                    case "start": _server.Start(new IPEndPoint(IPAddress.Any, port)); break;
                    case "send":
                        {
                            if (blocks.Length < 3)
                            {
                                Console.WriteLine("usage: send <ipep> <message>");
                            }
                            else
                            {
                                var ipep = blocks[1].ToIPEP();
                                if (ipep == null)
                                {
                                    Console.WriteLine("bad ipendpoint");
                                }
                                else
                                {
                                    var message = input.Substring(("send " + blocks[1] + " ").Length);
                                    _server.Send(ipep, Encoding.UTF8.GetBytes(message));
                                }
                            }
                        }
                        break;
                    case "disconnect": _server.Disconnect(blocks[1].ToIPEP()); break;
                    case "stop": _server.Stop(); break;
                    case "exit":
                        {
                            if (_server.IsListening)
                            {
                                _server.Stop();
                            }

                            isAlive = false;
                        }
                        break;
                }
            }
        }

        private void _OnStart()
        {
            Console.WriteLine("[start] " + _server.LocalIPEP);
        }

        private void _OnConnect(IPEndPoint remoteIPEP)
        {
            Console.WriteLine("[connect] " + remoteIPEP);
        }

        private void _OnData(IPEndPoint remoteIPEP, byte[] data)
        {
            Console.WriteLine($"[data] {remoteIPEP} ({data.Length}) {Encoding.UTF8.GetString(data)}");
        }

        private void _OnDisconnect(IPEndPoint remoteIPEP, Exception exception)
        {
            Console.WriteLine($"[disconnect] {remoteIPEP} exception: {exception?.Message ?? "null"}");
        }

        private void _OnStop(Exception exception)
        {
            Console.WriteLine($"[stop] exception: {exception?.Message ?? "null"}");
        }
    }
}
