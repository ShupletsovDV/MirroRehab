using MRTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public class UdpClientService : IUdpClientService
    {
        private UdpClient _client;
        private  IPEndPoint _remoteEP;

        public UdpClientService()
        {
            _client = new UdpClient();
            _remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
        }

        public void StartPing()
        {
            
          _client.Send(new byte[0], 0, _remoteEP);
             
        }

        public dynamic ReceiveData()
        {
            byte[] receiveBytes = _client.Receive(ref _remoteEP);
            string receiveString = Encoding.ASCII.GetString(receiveBytes).TrimEnd('\0');
            return Newtonsoft.Json.JsonConvert.DeserializeObject<JsonModel>(receiveString);
        }

        public void HandleError(Exception e, CancellationTokenSource cancellationToken)
        {
            // Error handling logic
        }
    }

}
