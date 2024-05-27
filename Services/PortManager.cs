using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public class PortManager
    {
        private readonly string com;
        private readonly int baudRate = 9600;

        public PortManager(string com)
        {
            this.com = com;
        }

        public void StartTracking(CancellationTokenSource cancellationToken, Action<double, double, double, double, double> processPosition)
        {
            using (var port = new SerialPort(this.com, this.baudRate))
            {
                port.Open();
                while (!cancellationToken.Token.IsCancellationRequested)
                {
                    // Чтение данных с порта и вызов метода processPosition для их обработки
                    double angThumb = 0, angIndex = 0, angMiddle = 0, angRing = 0, angPinky = 0; // Пример чтения данных с порта
                    processPosition(angThumb, angIndex, angMiddle, angRing, angPinky);
                }
                port.Close();
            }
        }
    }
}
