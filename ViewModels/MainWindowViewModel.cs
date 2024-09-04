
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using MRTest.Infrastructure.Commands;
using MRTest.Interfaces;
using MRTest.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Management;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfElmaBot_2._0_.ViewModels.Base;
using static MRTest.Services.Notifications;


namespace MRTest.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {

        private static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private string comPort = "";
        private CancellationTokenSource _cancellationTokenSource;

        private HandController1 handController;
        private Notifications notifications;



        #region Свойства

        private string _pathImage = "";
        public string PathImage
        {
            get => _pathImage;
            set => Set(ref _pathImage,value);
        }

       

        private string _imageVisible = "Visible"; //Visible

        public string ImageVisible
        {
            get => _imageVisible;
            set => Set(ref _imageVisible, value);
        }
       
        #region подключение к порту

        private string _connectinPort = "Hidden"; //Visible

        public string ConnectinPort
        {
            get => _connectinPort;
            set => Set(ref _connectinPort, value);
        }
        #endregion


        #region Текс сообщения
        private string _messageInfo = "";

        public string MessageInfo
        {
            get => _messageInfo;
            set => Set(ref _messageInfo, value);
        }
        #endregion

        #region кнопка свернуть

        private WindowState _WindowState;

        public WindowState WindowState
        {
            get => _WindowState;
            set => Set(ref _WindowState, value);
        }
        #endregion

        #region кружок запуска
        private string _colorStart = "Hidden";
        public string ColorStart
        {
            get => _colorStart;
            set => Set(ref _colorStart, value);
        }
        #endregion

        #region знак поиска
        private SolidColorBrush _colorSearch;

        public SolidColorBrush ColorSearch
        {
            get => _colorSearch;
            set => Set(ref _colorSearch, value);
        }
        #endregion

        #region Видимость кнопки запустить

        private string _visibleStartBtn = "Hidden";
        /// <summary>
        /// логин
        /// </summary>
        public string VisibleStartBtn
        {
            get => _visibleStartBtn;
            set => Set(ref _visibleStartBtn, value);
        }
        #endregion

        #region комбокос

        private List<string> _comPorts = new List<string>();
        public List<string> ComPorts
        {
            get { return _comPorts; }
            set => Set(ref _comPorts, value);
        }
        #endregion

        #region выбранный элемент комбокоса
        private string _selectedComPort;

        public string SelectedComPort
        {
            get { return _selectedComPort; }
            set
            {
                if (_selectedComPort != value)
                {
                    _selectedComPort = value;
                    OnPropertyChanged(nameof(SelectedComPort));
                }
            }
        }
        #endregion



        

        #endregion

        #region Команды

        #region Команда поиска
        public ICommand SearchDevice { get; set; }
        private void OnSearcBtnCommandExecuted(object p)
        {
            try
            {
                
                Search();
                Notifications_OnCommonPushpin("Устройство найдено", Notifications.NotificationEvents.Success);
                if (_comPorts.Count == 0)
                {
                    Notifications_OnCommonPushpin("Устройство не найдено", Notifications.NotificationEvents.NotConnectionPort); 
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении реестра: " + ex.Message);
            }


        }
        private bool CanSearchBtnCommandExecute(object p) => true;
        #endregion

        #region Команда калибровки
        public ICommand CalibrateBtnCommand { get; set; }
        private async void OnCalibrateBtnCommandExecuted(object p)
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedComPort))
                {
                    return;
                }
                comPort = string.IsNullOrEmpty(comPort) ? SelectedComPort.Split(" ")[0] : comPort;

                var chekPort = await Task.Run(() => handController.CheckPort(comPort));
                if (chekPort)
                {
                    var succesCalibration = await Task.Run(() => handController.CalibrateDevice(comPort));
                    if (succesCalibration)
                    {
                        Notifications_OnCommonPushpin("Устройство откалибровано", Notifications.NotificationEvents.Success);
                    }
                    else
                    {
                        ErrorSound();
                        //Notifications_OnCommonPushpin("Что то пошло не так", Notifications.NotificationEvents.NotConnectionPort);
                    }
                }
                else
                {
                    ErrorSound();
                    Notifications_OnCommonPushpin("Не удается подключиться к устройству", Notifications.NotificationEvents.NotConnectionPort);
                }


            }
            catch (Exception ex)
            {
               
                if (ex.Message.Contains("Object reference not set to an instance of an object"))
                {
                    ////MessageBox.Show("Выберите устройство!");
                }
            }


        }
        private bool CanCalibrateBtnCommandExecute(object p) => true;
        #endregion


        #region Команда кнопки выхода
        public ICommand CloseAppCommand { get; set; }
        private void OnCloseAppCommandExecuted(object p)
        {
            object locker = new();
            lock (locker)
            {
                Environment.Exit(0);

            }
        }
        private bool CanCloseAppCommandExecute(object p) => true;
        #endregion

        #region Команда кнопки запустить
        public ICommand StartBtnCommand { get; set; }
        private async void OnStartBtnCommandExecuted(object p)
        {

            Notifications_OnCommonPushpin("Запуск...", Notifications.NotificationEvents.ConnectionPort);
            _cancellationTokenSource = new CancellationTokenSource();
          
            var start= await Task.Run(()=>handController.StartTracking(_cancellationTokenSource.Token, comPort));
            if(!start)
            {
                ErrorSound();
            }
        }
        private bool CanStartBtnCommandExecute(object p) => true;
        #endregion

        #region Команда кнопки помощи
        public ICommand HelpBtnCommand { get; set; }
        private void OnHelpBtnCommandExecuted(object p)
        {
          
           handController.DefaultMirro( comPort);

        }
        private bool CanHelpBtnCommandExecute(object p) => true;
        #endregion

        #region Команда кнопки остановить
        public ICommand StopBtnCommand { get; set; }
        private void OnStopBtnCommandExecuted(object p)
        {

            ColorStart = "Hidden";
            _cancellationTokenSource?.Cancel();
            //File.WriteAllTextAsync("testData.txt",UdpClientService._positions);


        }
        private bool CanStopBtnCommandExecute(object p) => true;
        #endregion

        #region команда кнопки свернуть

        public ICommand RollUpCommand { get; set; }


        private void OnRollUpCommandExecuted(object p)
        {

            WindowState = WindowState.Minimized;


        }
        private bool CanRollUpCommandExecute(object p) => true;

        #endregion


        #region Команда кнопки демо
        public ICommand DemoBtnCommand { get; set; }
        private void OnDemoBtnCommandExecuted(object p)
        {
            _cancellationTokenSource = new CancellationTokenSource();
          Task.Run(()=> handController.DemoMirro(_cancellationTokenSource.Token, comPort));

        }
        private bool CanDemoBtnCommandExecute(object p) => true;
        #endregion

        #endregion

        public MainWindowViewModel()
        {
            try
            {
                InitializeProperties();
                InitializeCommands();
                SubscribeToNotifications();
                Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ErrorSound();
            }
        }
        private void SubscribeToNotifications()
        {
            handController = HandController1.GetHandController();
            notifications = Notifications.GetNotifications();
            notifications.OnCommonPushpin += Notifications_OnCommonPushpin;
        }
        private void InitializeProperties()
        {
            WindowState = WindowState.Normal;
            ColorSearch = new SolidColorBrush(Colors.Red);
        }
        private void InitializeCommands()
        {
            SearchDevice = new LambdaCommand(OnSearcBtnCommandExecuted, CanSearchBtnCommandExecute);
            CalibrateBtnCommand = new LambdaCommand(OnCalibrateBtnCommandExecuted, CanCalibrateBtnCommandExecute);
            CloseAppCommand = new LambdaCommand(OnCloseAppCommandExecuted, CanCloseAppCommandExecute);
            RollUpCommand = new LambdaCommand(OnRollUpCommandExecuted, CanRollUpCommandExecute);
            StartBtnCommand = new LambdaCommand(OnStartBtnCommandExecuted, CanStartBtnCommandExecute);
            StopBtnCommand = new LambdaCommand(OnStopBtnCommandExecuted, CanStopBtnCommandExecute);
            HelpBtnCommand = new LambdaCommand(OnHelpBtnCommandExecuted, CanHelpBtnCommandExecute);
            DemoBtnCommand = new LambdaCommand(OnDemoBtnCommandExecuted, CanDemoBtnCommandExecute);
        }
        private void Notifications_OnCommonPushpin(string message, Notifications.NotificationEvents notificationEvents)
        {
           
            MessageInfo = message;
            switch (notificationEvents)
            {
                case NotificationEvents.ConnectionPort:
                    ConnectinPort = "Visible";
                    ImageVisible = "Hidden";
                    break;
                case NotificationEvents.NotConnectionPort:
                    ImageVisible = "Visible";
                    ConnectinPort = "Hidden";
                    PathImage = "/Resources/NotConnect.png";
                    break;
                case NotificationEvents.CalibrateMax:
                    ImageVisible = "Visible";
                    ConnectinPort = "Hidden";
                    PathImage = "/Resources/hand1.png";
                    break;
                case NotificationEvents.CalibrateMin:
                    ImageVisible = "Visible";
                    ConnectinPort = "Hidden";
                    PathImage = "/Resources/hand2.png";
                    break;
                case NotificationEvents.Success:
                    ImageVisible = "Visible";
                    ConnectinPort = "Hidden";
                    PathImage = "/Resources/checkMark.png";
                    break;
                case NotificationEvents.PositionProcessor:
                    ConnectinPort = "Hidden";
                    ImageVisible = "Hidden";
                    break;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICalibrationService, CalibrationService>();
            services.AddSingleton<ISerialPortService, SerialPortService>();
            services.AddSingleton<IUdpClientService, UdpClientService>();
        }

        private void ErrorSound()
        {
            try
            {
                SystemSounds.Beep.Play();

            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }

        }
        private void Search()
        {
            try
            {
                _comPorts.Clear();
                using (ManagementClass i_Entity = new ManagementClass("Win32_PnPEntity"))
                {
                    const String CUR_CTRL = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\";

                    foreach (ManagementObject i_Inst in i_Entity.GetInstances())
                    {
                        Object o_Guid = i_Inst.GetPropertyValue("ClassGuid");
                        if (o_Guid == null || o_Guid.ToString().ToUpper() != "{4D36E978-E325-11CE-BFC1-08002BE10318}")
                            continue; // Skip all devices except device class "PORTS"

                        String s_Caption = i_Inst.GetPropertyValue("Caption").ToString();
                        String s_Manufact = i_Inst.GetPropertyValue("Manufacturer").ToString();
                        String s_DeviceID = i_Inst.GetPropertyValue("PnpDeviceID").ToString();
                        String s_RegEnum = CUR_CTRL + "Enum\\" + s_DeviceID + "\\Device Parameters";
                        String s_RegServ = CUR_CTRL + "Services\\BTHPORT\\Parameters\\Devices\\";
                        String s_PortName = Registry.GetValue(s_RegEnum, "PortName", "").ToString();
                        String s_BT_Dir = null; // Bluetooth port direction
                        String s_BT_Dev = "";   // Bluetooth paired device name
                        String s_BT_MAC = "";   // Bluetooth paired device MAC address

                        int s32_Pos = s_Caption.IndexOf(" (COM");
                        if (s32_Pos > 0) // remove COM port from description
                            s_Caption = s_Caption.Substring(0, s32_Pos);

                        if (s_DeviceID.StartsWith("BTHENUM\\")) // Bluetooth
                        {
                            s_BT_Dir = "Входящий";

                            // "{00001101-0000-1000-8000-00805f9b34fb}#7445CEA614BC_C00000000"
                            String s_UniqueID = Registry.GetValue(s_RegEnum, "Bluetooth_UniqueID", "").ToString();

                            String[] s_Split = s_UniqueID.Split('#');
                            if (s_Split.Length == 2)
                            {
                                s_Split = s_Split[1].Split('_');

                                // Ignore MAC = "000000000000"
                                if (s_Split.Length == 2 && s_Split[0].Trim('0').Length > 0)
                                {
                                    s_BT_MAC = s_Split[0]; // 12 digits: "7445CEA614BC"
                                    s_BT_Dir = "Исходящий";

                                    // Read the Bluetooth device that is paired with the COM port.
                                    Object o_BtDevice = Registry.GetValue(s_RegServ + s_BT_MAC, "Name", null);
                                    if (o_BtDevice is Byte[])
                                        s_BT_Dev = Encoding.UTF8.GetString((Byte[])o_BtDevice);
                                }
                            }

                            _comPorts.Add("" + s_PortName +
                                          " " + s_BT_Dir +
                                          " " + s_BT_Dev);
                        }


                    }
                }
                FindNeedCom();

            }
            catch (Exception ex)
            {
                //TODO лог
                MessageBox.Show("Ошибка при чтении реестра: " + ex.Message);
            }
        }

        private void FindNeedCom()
        {
            try
            {
                int i = 0;
                if (_comPorts.Count != 0)
                {
                    foreach (string x in _comPorts)
                    {
                        if (x.Contains("MirroRehab") && x.Contains("Исходящий"))
                        {
                            comPort = x.Split(" ")[0];
                            ColorSearch = new SolidColorBrush(Color.FromRgb(2, 190, 104)); // Цвет #02be68
                            SelectedComPort = x;
                            MessageInfo = "Устройство найдено!";
                            PathImage = "/Resources/checkMark.png";
                            i++;

                            break;
                        }
                    }
                }
                if (i == 0)
                {
                    MessageInfo = "Устройство не найдено!";
                    PathImage = "/Resources/mistake.png";
                }
            }
            catch (Exception ex)
            { 
             //TODO лог
            }
        }
    }
}
