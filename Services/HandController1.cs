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
                OpenPort(port);
                ClosePort();
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
                string defaultData = "180,0,180,180,0\n";
                Notifications.GetNotifications().InvokeCommonStatus("Проверка подключения к устройству", Notifications.NotificationEvents.ConnectionPort);
                OpenPort(port);
                for(int i= 0; i < 10;i++)
                {
                    SendPort(defaultData);
                }
                ClosePort();
                Notifications.GetNotifications().InvokeCommonStatus("Настройки Mirro сброшены", Notifications.NotificationEvents.Success);

            }
            catch
            {
                Notifications.GetNotifications().InvokeCommonStatus("Не удалось подключиться к устройству", Notifications.NotificationEvents.NotConnectionPort);
               
            }
        }
        public void DemoMirro(CancellationToken cancellationToken, string port)
        {
            try
            {
                Notifications.GetNotifications().InvokeCommonStatus("Проверка подключения к устройству", Notifications.NotificationEvents.ConnectionPort);
                //OpenPort(port);
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        ClosePort();
                        Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                        break;
                    }

                    for (int i = 0; i<=7;i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            ClosePort();
                            Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                            break;
                        }
                        string defaultData = $"{Dictionaries.MyDict[i/10.0]},{Dictionaries.MyDict[i/10.0]},{Dictionaries.MyDictReverse[i/10.0]},{Dictionaries.MyDictReverse[i/10.0]},0";
                        Notifications.GetNotifications().InvokeCommonStatus(defaultData, Notifications.NotificationEvents.ConnectionPort);
                        Thread.Sleep(400);
                        SendPort(defaultData);
                    }
                    for (int i = 7;i>=0;i--)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            ClosePort();
                            Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                            break;
                        }
                        string defaultData = $"{Dictionaries.MyDict[i/10.0]},{Dictionaries.MyDict[i/10.0]},{Dictionaries.MyDictReverse[i/10.0]},{Dictionaries.MyDictReverse[i/10.0]},0";
                        Notifications.GetNotifications().InvokeCommonStatus(defaultData, Notifications.NotificationEvents.ConnectionPort);
                        Thread.Sleep(400);
                        SendPort(defaultData);
                    }

                    for (int i=0;i<4;i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            ClosePort();
                            Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                            break;
                        }
                        for (int j = 0;j<=7;j++)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                ClosePort();
                                Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                                break;
                            }
                            string a1 = "";
                            string b1 = "";
                            string c1 = "";
                            string f1 = "";
                            if (i==0)
                            {
                            
                                a1 = $"{Dictionaries.MyDict[j / 10.0]}";
                                b1 = $"{Dictionaries.MyDict[0.0]}";
                                c1 = $"{Dictionaries.MyDictReverse[0.0]}";
                                f1 = $"{Dictionaries.MyDictReverse[0.0]}";

                            }
                            if(i==1)
                            {
                                a1 = $"{Dictionaries.MyDict[0.7]}";
                                b1 = $"{Dictionaries.MyDict[j / 10.0]}";
                                c1 = $"{Dictionaries.MyDictReverse[0.0]}";
                                f1 = $"{Dictionaries.MyDictReverse[0.0]}";
                            }
                            if (i == 2)
                            {
                                a1 = $"{Dictionaries.MyDict[0.7]}";
                                b1 = $"{Dictionaries.MyDict[0.7]}";
                                c1 = $"{Dictionaries.MyDictReverse[j / 10.0]}";
                                f1 = $"{Dictionaries.MyDictReverse[0.0]}";
                            }
                            if (i == 3)
                            {
                                a1 = $"{Dictionaries.MyDict[0.7]}";
                                b1 = $"{Dictionaries.MyDict[0.7]}";
                                c1 = $"{Dictionaries.MyDictReverse[0.7]}";
                                f1 = $"{Dictionaries.MyDictReverse[j / 10.0]}";
                            }
                            string defaultData = 
                                $"{a1}," +
                                $"{b1}," +
                                $"{c1}," +
                                $"{f1},0";
                            Notifications.GetNotifications().InvokeCommonStatus(defaultData, Notifications.NotificationEvents.ConnectionPort);
                            Thread.Sleep(400);
                            SendPort(defaultData);
                        }
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            ClosePort();
                            Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                            break;
                        }

                        for (int j = 7; j>=0 ; j--)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                ClosePort();
                                Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                                break;
                            }
                            string a1 = "";
                            string b1 = "";
                            string c1 = "";
                            string f1 = "";
                            if (i == 0)
                            {

                                a1 = $"{Dictionaries.MyDict[j / 10.0]}";
                                b1 = $"{Dictionaries.MyDict[0.7]}";
                                c1 = $"{Dictionaries.MyDictReverse[0.7]}";
                                f1 = $"{Dictionaries.MyDictReverse[0.7]}";

                            }
                            if (i == 1)
                            {
                                a1 = $"{Dictionaries.MyDict[0.0]}";
                                b1 = $"{Dictionaries.MyDict[j / 10.0]}";
                                c1 = $"{Dictionaries.MyDictReverse[0.7]}";
                                f1 = $"{Dictionaries.MyDictReverse[0.7]}";
                            }
                            if (i == 2)
                            {
                                a1 = $"{Dictionaries.MyDict[0.0]}";
                                b1 = $"{Dictionaries.MyDict[0.0]}";
                                c1 = $"{Dictionaries.MyDictReverse[j / 10.0]}";
                                f1 = $"{Dictionaries.MyDictReverse[0.7]}";
                            }
                            if (i == 3)
                            {
                                a1 = $"{Dictionaries.MyDict[0.0]}";
                                b1 = $"{Dictionaries.MyDict[0.0]}";
                                c1 = $"{Dictionaries.MyDictReverse[0.0]}";
                                f1 = $"{Dictionaries.MyDictReverse[j / 10.0]}";
                            }
                            string defaultData =
                                $"{a1}," +
                                $"{b1}," +
                                $"{c1}," +
                                $"{f1},0";
                            Notifications.GetNotifications().InvokeCommonStatus(defaultData, Notifications.NotificationEvents.ConnectionPort);
                            Thread.Sleep(400);
                            SendPort(defaultData);
                        }
                    }

                }
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
                OpenPort(port);
                Notifications.GetNotifications().InvokeCommonStatus("Разожмите руку и удерживайте",Notifications.NotificationEvents.CalibrateMin);
                Thread.Sleep(2000);
                _calibrationService.CalibrateMin(_serialPortService, _udpClientService);
                Notifications.GetNotifications().InvokeCommonStatus("Сожмите руку и удерживайте", Notifications.NotificationEvents.CalibrateMax);
                Thread.Sleep(2000);
                _calibrationService.CalibrateMax(_serialPortService, _udpClientService);
                ClosePort();
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
                OpenPort(com);
                while (true)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            ClosePort();
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
                       
                        ClosePort();
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
