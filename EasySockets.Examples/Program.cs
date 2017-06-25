using System;

namespace MFatihMAR.EasySockets.Examples
{
    internal static class Program
    {
        private static void Main()
        {
            var input = string.Empty;
            while (true)
            {
                input = Console.ReadLine();
                switch (input)
                {
                    default: Console.WriteLine("commands: udpPeer, tcpServer, tcpClient, exit"); break;
                    case "udpPeer":
                        Console.WriteLine("udpPeer started");
                        new UdpPeerExample().Run();
                        Console.WriteLine("udpPeer ended");
                        break;
                    case "tcpServer":
                        Console.WriteLine("not implemented!");
                        break;
                    case "tcpClient":
                        Console.WriteLine("not implemented!");
                        break;
                    case "exit": return;
                }
            }
        }
    }
}
