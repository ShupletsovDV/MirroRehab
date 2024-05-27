using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public class UdpManager
    {
        private const string ipAdress = "127.0.0.1";
        private const int port = 53450;

        private UdpClient client;
        private IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ipAdress), port);
        public UdpManager()
        {
            client = new UdpClient();
            client.Send(new byte[0], 0, remoteEP);
        }

        public void Ping()
        {
            while (true)
            {
                Thread.Sleep(10);
                client.Send(new byte[0], 0, remoteEP);
            }

        }
    }
}
