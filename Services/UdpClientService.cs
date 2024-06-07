using MRTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MRTest.Services
{
    public class UdpClientService : IUdpClientService
    {
        private UdpClient _client;
        private  IPEndPoint _remoteEP;

        public UdpClientService()
        {
            _client = new UdpClient();
            _remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53452);
           
        }

        public void StartPing()
        {
            
          _client.Send(new byte[0], 0, _remoteEP);
             
        }

        public JsonModel ReceiveData()
        {
            string messageString = "ping";
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
            _client.Send(messageBytes, messageBytes.Length, remoteEP);
            
            byte[] receiveBytes = _client.Receive(ref remoteEP);
            string receiveString = Encoding.ASCII.GetString(receiveBytes).TrimEnd('\0');
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JsonModel>(receiveString);
        }

        public void HandleError( CancellationToken cancellationToken)
        {
            // Error handling logic
        }
    }

}
