using System.Net;

namespace MFatihMAR.EasySockets
{
    public static class IPEndPointParser
    {
        public static IPEndPoint ToIPEP(this string source)
        {
            var blocks = source.Split(':');
            if (blocks.Length != 2)
            {
                return null;
            }

            if (!IPAddress.TryParse(blocks[0], out IPAddress addr))
            {
                return null;
            }

            if (!ushort.TryParse(blocks[1], out ushort port))
            {
                return null;
            }

            return new IPEndPoint(addr, port);
        }

        public static IPEndPoint Parse(string ipepStr)
        {
            return ipepStr.ToIPEP();
        }
    }
}
