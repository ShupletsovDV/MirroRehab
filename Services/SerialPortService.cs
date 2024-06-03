using MRTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public class SerialPortService : ISerialPortService
    {
        private SerialPort _serialPort;

        public bool IsPortOpen => _serialPort != null && _serialPort.IsOpen;

        public void OpenPort(string comPort)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                _serialPort = new SerialPort(comPort, 9600);
                _serialPort.Open();
            }
            catch 
            {
                throw new Exception();
            }

        }

        public void ClosePort()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void SendData(string data)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.WriteLine(data);
            }
            else
            {
                throw new InvalidOperationException("Serial port is not open.");
            }
        }
    }
}
