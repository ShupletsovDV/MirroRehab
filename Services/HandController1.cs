using MRTest.Interfaces;
using MRTest.ViewModels;
using System;
using System.Threading;

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
            if (handController == null)
            {
                handController = new HandController1(new CalibrationService(), new UdpClientService(), new PositionProcessor(), SerialPortService.GetInstance());
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
                Notifications.GetNotifications().InvokeCommonStatus("Проверка подключения к устройству", Notifications.NotificationEvents.ConnectionPort);
                OpenPort(port);
                ClosePort();
                return true;
            }
            catch(Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при проверке порта: {ex}");
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
                string defaultData = $"{Dictionaries.MinIndex},{Dictionaries.MinMiddle},{Dictionaries.MinRing},{Dictionaries.MinPinky},0\n";
                Notifications.GetNotifications().InvokeCommonStatus("Проверка подключения к устройству", Notifications.NotificationEvents.ConnectionPort);
                OpenPort(port);
                for (int i = 0; i < 10; i++)
                {
                    SendPort(defaultData);
                }
                ClosePort();
                Notifications.GetNotifications().InvokeCommonStatus("Настройки Mirro сброшены", Notifications.NotificationEvents.Success);

            }
            catch(Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при отправке default значение: {ex}");
                Notifications.GetNotifications().InvokeCommonStatus("Не удалось подключиться к устройству", Notifications.NotificationEvents.NotConnectionPort);

            }
        }
        public void DemoMirro(CancellationToken cancellationToken, string port)
        {
            int sleep = 50;
           
            try
            {
                Notifications.GetNotifications().InvokeCommonStatus("Проверка подключения к устройству", Notifications.NotificationEvents.ConnectionPort);
                OpenPort(port);

                while (true)
                {
                    
                    sleep =new Random().Next(10,60);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        ClosePort();
                        Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                        break;
                    }

                    // Сжатие всех пальцев
                    for (double i = 0.0; i <= 3.0; i += 0.1)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            ClosePort();
                            Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                            break;
                        }

                        // Округляем значение i до одного знака после запятой
                        double roundedI = Math.Round(i, 1);

                        string defaultData = $"{Dictionaries.DictIndex[roundedI]},{Dictionaries.DictMiddle[roundedI]},{Dictionaries.DictRing[roundedI]},{Dictionaries.DictPinky[roundedI]},0";
                        Notifications.GetNotifications().InvokeCommonStatus(defaultData, Notifications.NotificationEvents.ConnectionPort);
                        Thread.Sleep(sleep);
                        SendPort(defaultData);
                    }

                    // Аналогично для других циклов
                    for (double i = 3.0; i >= 0.0; i -= 0.1)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            ClosePort();
                            Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                            break;
                        }

                        // Округляем значение i до одного знака после запятой
                        double roundedI = Math.Round(i, 1);

                        string defaultData = $"{Dictionaries.DictIndex[roundedI]},{Dictionaries.DictMiddle[roundedI]},{Dictionaries.DictRing[roundedI]},{Dictionaries.DictPinky[roundedI]},0";
                        Notifications.GetNotifications().InvokeCommonStatus(defaultData, Notifications.NotificationEvents.ConnectionPort);
                        Thread.Sleep(sleep);
                        SendPort(defaultData);
                    }

                    // Аналогично для цикла сжатия и разжатия пальцев по отдельности
                    for (int finger = 0; finger < 4; finger++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            ClosePort();
                            Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                            break;
                        }

                        // Сжатие одного пальца
                        for (double i = 0.0; i <= 3.0; i += 0.1)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                ClosePort();
                                Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                                break;
                            }

                            // Округляем значение i до одного знака после запятой
                            double roundedI = Math.Round(i, 1);

                            /* string a1 = finger == 0 ? $"{Dictionaries.MyDictReverse[roundedI]}" : $"{Dictionaries.MyDictReverse[0.0]}";
                             string b1 = finger == 1 ? $"{Dictionaries.MyDict[roundedI]}" : $"{Dictionaries.MyDict[0.0]}";
                             string c1 = finger == 2 ? $"{Dictionaries.MyDictReverse[roundedI]}" : $"{Dictionaries.MyDictReverse[0.0]}";
                             string f1 = finger == 3 ? $"{Dictionaries.MyDict[roundedI]}" : $"{Dictionaries.MyDict[0.0]}";*/
                            string a1 = finger == 0 ? $"{Dictionaries.DictIndex[roundedI]}" : $"{Dictionaries.DictIndex[0.0]}";
                            string b1 = finger == 1 ? $"{Dictionaries.DictMiddle[roundedI]}" : $"{Dictionaries.DictMiddle[0.0]}";
                            string c1 = finger == 2 ? $"{Dictionaries.DictRing[roundedI]}" : $"{Dictionaries.DictRing[0.0]}";
                            string f1 = finger == 3 ? $"{Dictionaries.DictPinky[roundedI]}" : $"{Dictionaries.DictPinky[0.0]}";

                            string defaultData = $"{a1},{b1},{c1},{f1},{a1}";
                            Notifications.GetNotifications().InvokeCommonStatus(defaultData, Notifications.NotificationEvents.ConnectionPort);
                            Thread.Sleep(sleep);
                            SendPort(defaultData);
                        }

                        // Разжатие одного пальца
                        for (double i = 3.0; i >= 0.0; i -= 0.1)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                ClosePort();
                                Notifications.GetNotifications().InvokeCommonStatus("Stop", Notifications.NotificationEvents.Success);
                                break;
                            }

                            // Округляем значение i до одного знака после запятой
                            double roundedI = Math.Round(i, 1);

                            string a1 = finger == 0 ? $"{Dictionaries.DictIndex[roundedI]}" : $"{Dictionaries.DictIndex[0.0]}";
                            string b1 = finger == 1 ? $"{Dictionaries.DictMiddle[roundedI]}" : $"{Dictionaries.DictMiddle[0.0]}";
                            string c1 = finger == 2 ? $"{Dictionaries.DictRing[roundedI]}" : $"{Dictionaries.DictRing[0.0]}";
                            string f1 = finger == 3 ? $"{Dictionaries.DictPinky[roundedI]}" : $"{Dictionaries.DictPinky[0.0]}";

                            string defaultData = $"{a1},{b1},{c1},{f1},{a1}";
                            Notifications.GetNotifications().InvokeCommonStatus(defaultData, Notifications.NotificationEvents.ConnectionPort);
                            Thread.Sleep(sleep);
                            SendPort(defaultData);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ClosePort();
                MainWindowViewModel.Log.Error($"\nОшибка в демо : {ex}");
                Notifications.GetNotifications().InvokeCommonStatus("Не удалось подключиться к устройству " + ex.Message, Notifications.NotificationEvents.NotConnectionPort);
            }
        }

        public bool CalibrateDevice(string port)
        {
            try
            {
                _udpClientService.StartPing();
                Notifications.GetNotifications().InvokeCommonStatus("Подключение к устройству", Notifications.NotificationEvents.ConnectionPort);
                OpenPort(port);
                Notifications.GetNotifications().InvokeCommonStatus("Разожмите руку и удерживайте", Notifications.NotificationEvents.CalibrateMin);
                Thread.Sleep(2000);
                _calibrationService.CalibrateMin(_serialPortService, _udpClientService);
                Notifications.GetNotifications().InvokeCommonStatus("Сожмите руку и удерживайте", Notifications.NotificationEvents.CalibrateMax);
                Thread.Sleep(2000);
                _calibrationService.CalibrateMax(_serialPortService, _udpClientService);
                ClosePort();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Удаленный хост"))
                {
                    Notifications.GetNotifications().InvokeCommonStatus("Убедитесь, что перчатка Senso подключена", Notifications.NotificationEvents.NotConnectionPort);
                }
                return false;
            }
        }



        public bool StartTracking(CancellationToken cancellationToken, string com)
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
                        receiveData = null;
                    }
                    catch (Exception ex)
                    {
                        MainWindowViewModel.Log.Error($"\nОшибка при работе перчатки: {ex}");

                        if (ex.Message.Contains("Удаленный хост"))
                        {
                            Notifications.GetNotifications().InvokeCommonStatus("Убедитесь, что перчатка Senso подключена", Notifications.NotificationEvents.NotConnectionPort);

                        }
                        else if(ex.Message.Contains("Serial port is not open."))
                        {
                            Notifications.GetNotifications().InvokeCommonStatus("Убедитесь, что перчатка MirroRehab подключена", Notifications.NotificationEvents.NotConnectionPort);
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
            catch(Exception ex)
            {

                MainWindowViewModel.Log.Error($"\nОшибка при работе перчатки: {ex}");

                Notifications.GetNotifications().InvokeCommonStatus("Не удалось подключиться к устройству", Notifications.NotificationEvents.NotConnectionPort);
                return false;
            }
        }
    }


}
