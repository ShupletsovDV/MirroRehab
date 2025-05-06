using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRTest.Interfaces
{
    public interface IPositionProcessor
    {
        void ProcessPosition(JsonModel receiveData, ISerialPortService serialPortService);
    }

}
