using MRTest.Interfaces;
using Newtonsoft.Json.Linq;
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
        public void OpenPort(string port)
        {
            _serialPortService.OpenPort(port);
        }
        public void SendPort(string data)
        {
            _serialPortService.SendData(data);
        }
        public void DefaultMirro(string port)
        {
            try
            {
                string defaultData = "0,0,0,0,0\n";
                Notifications.GetNotifications().InvokeCommonStatus("Проверка подключения к устройству", Notifications.NotificationEvents.ConnectionPort);
                _serialPortService.OpenPort(port);
                _serialPortService.SendData(defaultData);
                _serialPortService.ClosePort();
                Notifications.GetNotifications().InvokeCommonStatus("Настройки Mirro сброшены", Notifications.NotificationEvents.Success);

            }
            catch
            {
                Notifications.GetNotifications().InvokeCommonStatus("Не удалось подключиться к устройству", Notifications.NotificationEvents.NotConnectionPort);
               
            }
        }
        
        public bool CalibrateDevice(string port)
        {
            try
            {
                _udpClientService.StartPing();
                Notifications.GetNotifications().InvokeCommonStatus("Подключение к устройству", Notifications.NotificationEvents.ConnectionPort);
                _serialPortService.OpenPort(port);
                Notifications.GetNotifications().InvokeCommonStatus("Разожмите руку и удерживайте",Notifications.NotificationEvents.CalibrateMin);
                Thread.Sleep(3000);
                _calibrationService.CalibrateMin(_serialPortService, _udpClientService);
                Notifications.GetNotifications().InvokeCommonStatus("Сожмите руку и удерживайте", Notifications.NotificationEvents.CalibrateMax);
                Thread.Sleep(3000);
                _calibrationService.CalibrateMax(_serialPortService, _udpClientService);
                _serialPortService.ClosePort();
                return true;
            }
            catch(Exception ex)
            {
                if(ex.Message.Contains("Удаленный хост"))
                {
                    Notifications.GetNotifications().InvokeCommonStatus("Убедитесь, что перчатка Senso подключена", Notifications.NotificationEvents.NotConnectionPort);
                }
                return false;
            }
        }

       

        public bool StartTracking(CancellationToken cancellationToken,string com)
        {
            try
            {
                _udpClientService.StartPing();
                _serialPortService.OpenPort(com);
                while (true)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            _serialPortService.ClosePort();
                            break;
                        }

                        var receiveData = _udpClientService.ReceiveData();


                        if (receiveData != null && receiveData.type == "position")
                        {
                            _positionProcessor.ProcessPosition(receiveData, _serialPortService);
                        }

                    }
                    catch(Exception ex)
                    {
                        if(ex.Message.Contains("Удаленный хост"))
                        {
                            Notifications.GetNotifications().InvokeCommonStatus("Убедитесь, что перчатка Senso подключена", Notifications.NotificationEvents.NotConnectionPort);

                        }
                        else
                        {
                            Notifications.GetNotifications().InvokeCommonStatus("Что то пошло не так", Notifications.NotificationEvents.NotConnectionPort);
                        }
                       
                        _serialPortService.ClosePort();
                        _udpClientService.HandleError(cancellationToken);
                        break;
                    }
                }
                return true;
            }
            catch
            {
                
                Notifications.GetNotifications().InvokeCommonStatus("Не удалось подключиться к устройству", Notifications.NotificationEvents.NotConnectionPort);
                return false;
            }
        }
    }


}
