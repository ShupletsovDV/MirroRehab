
using MRTest.Interfaces;
using MRTest.ViewModels;
using System;
using System.IO.Ports;
using System.Windows;

namespace MRTest.Services
{
    public class SerialPortService : ISerialPortService
    {
        private SerialPort _serialPort;
        private static SerialPortService _instance;
        private static readonly object _lock = new object();

        private SerialPortService()
        {
          
        }

        // Потокобезопасный метод для получения единственного экземпляра
        public static SerialPortService GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SerialPortService();
                    }
                }
            }
            return _instance;
        }

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
                string defaultData = $"{Dictionaries.MinIndex},{Dictionaries.MinMiddle},{Dictionaries.MinRing},{Dictionaries.MinPinky}\n";
                SendData(defaultData);
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при подключении: {ex}");
                throw new InvalidOperationException($"\nFailed to open serial port {comPort}: {ex.Message}", ex);
            }
        }

        public void ClosePort()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.Close();
                }
                _serialPort = null;
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при закрытии подлкючения: {ex}");
                MessageBox.Show($"\nError closing serial port: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
        }

        public void SendData(string data)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.WriteLine(data);
                    _serialPort.DiscardOutBuffer();
                }
                else
                {
                    throw new InvalidOperationException("\nSerial port is not open.");
                }
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при отправке данных: {ex}");
                throw new InvalidOperationException($"\nFailed to send data: {ex.Message}", ex);
            }
        }
    }
}