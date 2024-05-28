using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRTest.Interfaces
{
    public interface ISerialPortService
    {
        void OpenPort(string comPort);
        void ClosePort();
        void SendData(string data);
        bool IsPortOpen { get; }
    }
}
