using MRTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRTest.Interfaces
{
    public interface ICalibrationService
    {
        void CalibrateMax(ISerialPortService serialPortService, IUdpClientService _udpClientService);
        void CalibrateMin(ISerialPortService serialPortService, IUdpClientService _udpClientService);
        List<List<double>> CollectCalibrationData(List<List<double>> calibrationData,ISerialPortService serialPortService, IUdpClientService _udpClientService);
    }

}
