using MRTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public class HandController1
    {
        private readonly ICalibrationService _calibrationService;
        private readonly IUdpClientService _udpClientService;
        private readonly IPositionProcessor _positionProcessor;
        private readonly ISerialPortService _serialPortService;

        private static HandController1 handController;
        public static HandController1 GetHandController()
        {
            if(handController==null)
            {
                handController= new HandController1(new CalibrationService(), new UdpClientService(), new PositionProcessor(), new SerialPortService());
                return handController;
            }
            else { return handController; }
        }

        public HandController1(ICalibrationService calibrationService, IUdpClientService udpClientService, IPositionProcessor positionProcessor, ISerialPortService serialPortService)
        {
            _calibrationService = calibrationService;
            _udpClientService = udpClientService;
            _positionProcessor = positionProcessor;
            _serialPortService = serialPortService;
        }
        public bool CheckPort(string port)
        {
            try
            {
                Notifications.GetNotifications().InvokeCommonStatus("Проверка подключения к устройству",Notifications.NotificationEvents.ConnectionPort);
                _serialPortService.OpenPort(port);
                _serialPortService.ClosePort();
                return true;
            }
            catch 
            {
                Notifications.GetNotifications().InvokeCommonStatus("Не удалось подключиться к устройству", Notifications.NotificationEvents.NotConnectionPort);
                return false; 
            }
            
        }
        public void ClosePort() 
        {
            _serialPortService.ClosePort();
        }
        
        public bool CalibrateDevice(string com)
        {
            try
            {
                Notifications.GetNotifications().InvokeCommonStatus("Подключение к устройству", Notifications.NotificationEvents.ConnectionPort);
                _serialPortService.OpenPort(com);
                Notifications.GetNotifications().InvokeCommonStatus("Разожмите руку",Notifications.NotificationEvents.CalibrateMax);
                _calibrationService.CalibrateMax(_serialPortService, _udpClientService);
                Notifications.GetNotifications().InvokeCommonStatus("Сожмите руку", Notifications.NotificationEvents.CalibrateMin);
                _calibrationService.CalibrateMin(_serialPortService, _udpClientService);
                _serialPortService.ClosePort();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

       

        public void StartTracking(CancellationTokenSource cancellationToken, SerialPort port)
        {
            _udpClientService.StartPing();
            while (true)
            {
                try
                {
                    var receiveData = _udpClientService.ReceiveData();
                    if (receiveData != null && receiveData.Type == "position")
                    {
                        //_positionProcessor.ProcessPosition(receiveData, port);
                    }
                }
                catch (SocketException e)
                {
                    _udpClientService.HandleError(e, cancellationToken);
                    break;
                }
            }
        }
    }


}
