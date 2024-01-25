using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MRTest
{
    public class HandController
    {

        public static string message = "";

        private static Dictionary<double, int> myDict = new Dictionary<double, int>
        {
            {0.0, 0}, {0.1, 0}, {0.2, 0}, {0.3, 0}, {0.4, 1}, {0.5, 1}, {0.6, 1},
            {0.7, 2}, {0.8, 2}, {0.9, 2}, {1.0, 3}, {1.1, 3}, {1.2, 3}, {1.3, 4},
            {1.4, 4}, {1.5, 4}, {1.6, 5}, {1.7, 5}, {1.8, 5}, {1.9, 6}, {2.0, 6},
            {2.1, 6}, {2.2, 7}, {2.3, 7}, {2.4, 7}, {2.5, 8}, {2.6, 9}, {2.7, 9},
            {2.8, 9}, {2.9, 9}, {3.0, 9}, {3.1, 9}
        };

        private static Dictionary<double, int> myDictIndex = new Dictionary<double, int>
        {
            {0.0, 9}, {0.1, 9}, {0.2, 9}, {0.3, 9}, {0.4, 8}, {0.5, 8}, {0.6, 8},
            {0.7, 7}, {0.8, 7}, {0.9, 7}, {1.0, 6}, {1.1, 6}, {1.2, 6}, {1.3, 5},
            {1.4, 5}, {1.5, 5}, {1.6, 4}, {1.7, 4}, {1.8, 4}, {1.9, 3}, {2.0, 3},
            {2.1, 3}, {2.2, 2}, {2.3, 2}, {2.4, 2}, {2.5, 1}, {2.6, 0}, {2.7, 0},
            {2.8, 0}, {2.9, 0}, {3.0, 0}, {3.1, 0}
        };

        private static Dictionary<double, int> myDictThumb = new Dictionary<double, int>
        {
            {0.0, 0}, {0.1, 0}, {0.2, 1}, {0.3, 2}, {0.4, 3}, {0.5, 4}, {0.6, 5},
            {0.7, 6}, {0.8, 7}, {0.9, 8}, {1.0, 9}, {1.1, 9}, {1.2, 9}
        };

        private double max = 2.6;
        private double min = 0.0;

        private const int calibrationDuration = 5;
        private List<List<double>> maxCalibrationData = new List<List<double>>();
        private List<List<double>> minCalibrationData = new List<List<double>>();

        private UdpClient client;
        
        private const int baudRate = 9600;
        public HandController(string com)
        {
           
            client = new UdpClient();
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
            client.Send(new byte[0], 0, remoteEP);

        }

        public void Ping()
        {
            while (true)
            {
                Thread.Sleep(10);
                /*string messageString = "ping";
                byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
                client.Send(messageBytes, messageBytes.Length, remoteEP);*/


                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
                client.Send(new byte[0], 0, remoteEP);
            }
           
        }
       private (double,double,double,double,double) MaxMin(double angThumb,double angIndex,double angMiddle, double angRing, double angPinky) //TODO перенести сюда ифы
        {
            if (angThumb < 0)
            {
                angThumb = 0.0;
            }
            if (angThumb > CalibrateRezult.maxThumb)
            {
                angThumb = CalibrateRezult.maxThumb;
            }

            if (angIndex < 0)
            {
                angIndex = 0.0;
            }
            if (angIndex > CalibrateRezult.maxIndex)
            {
                angIndex = CalibrateRezult.maxIndex;
            }

            if (angMiddle < 0)
            {
                angMiddle = 0.0;
            }
            if (angMiddle > CalibrateRezult.maxMiddle)
            {
                angMiddle = CalibrateRezult.maxMiddle;
            }

            if (angPinky < 0)
            {
                angPinky = 0.0;
            }
            if (angPinky > CalibrateRezult.maxPinky)
            {
                angPinky = CalibrateRezult.maxPinky;
            }

            if (angRing < 0)
            {
                angRing = 0.0;
            }
            if (angRing > CalibrateRezult.maxRing)
            {
                angRing = CalibrateRezult.maxRing;
            }
            return (angThumb, angIndex, angMiddle, angRing, angPinky);
        }
            
        public void ProcessPosition(dynamic receiveData,SerialPort port)
        {
            double angThumb = Math.Round(receiveData.data.fingers[0].ang[0], 1);
            double angIndex = Math.Round(receiveData.data.fingers[1].ang[0], 1);
            double angMiddle = Math.Round(receiveData.data.fingers[2].ang[0], 1);
            double angRing = Math.Round(receiveData.data.fingers[3].ang[0], 1);
            double angPinky = Math.Round(receiveData.data.fingers[4].ang[0], 1);

            #region if

            #endregion
            (angThumb, angIndex, angMiddle, angRing, angPinky) = MaxMin(angThumb, angIndex, angMiddle, angRing, angPinky);

            string data = $"${myDictThumb[angThumb]}{myDictIndex[angIndex]}{myDictIndex[angMiddle]}{myDict[angRing]}{myDict[angPinky]}";

            SendToMirro(data, port);

            MessageBox.Show($"{angThumb} {angIndex} {angMiddle} {angRing} {angPinky}");
        }

        public void CalibrateDeviceMax(CancellationTokenSource cancellationToken,string com)
        {
            using (SerialPort port = new SerialPort(com, baudRate))
            {
                if (port != null && port.IsOpen)
                {
                    port.Close();
                }
                port.Open();
                message = "Сейчас сожмите руку и удерживайте в течении 5 секунд";
                MessageBox.Show(message);

                maxCalibrationData = CollectCalibrationData(maxCalibrationData, cancellationToken, port);
                (CalibrateRezult.maxThumb, CalibrateRezult.maxIndex, CalibrateRezult.maxMiddle, CalibrateRezult.maxRing, CalibrateRezult.maxPinky) = CalculateAverages(maxCalibrationData);
                port.Close();
                message = "Калибровка завершена";
                MessageBox.Show(message);
                MessageBox.Show($"Max Thumb: {CalibrateRezult.maxThumb}");
                MessageBox.Show($"Max Index: {CalibrateRezult.maxIndex}");
                MessageBox.Show($"Max Middle: {CalibrateRezult.maxMiddle}");
                MessageBox.Show($"Max Ring: {CalibrateRezult.maxRing}");
                MessageBox.Show($"Max Pinky: {CalibrateRezult.maxPinky}");
            }
        }
        public bool CalibrateDeviceMin(CancellationTokenSource cancellationToken, string com)
        {
            try
            {
                using (SerialPort port = new SerialPort(com, baudRate))
                {
                    if (port != null && port.IsOpen)
                    {
                        port.Close();
                    }
                    port.Open();
                    message = "Разожмите руку и удерживайте в течении 5 секунд";
                    MessageBox.Show(message);

                    minCalibrationData = CollectCalibrationData(minCalibrationData, cancellationToken, port);
                    (CalibrateRezult.minThumb, CalibrateRezult.minIndex, CalibrateRezult.minMiddle, CalibrateRezult.minRing, CalibrateRezult.minPinky) = CalculateAverages(minCalibrationData);
                    port.Close();
                    MessageBox.Show($"Min Thumb: {CalibrateRezult.minThumb}");
                    MessageBox.Show($"Min Index: {CalibrateRezult.minIndex}");
                    MessageBox.Show($"Min Middle: {CalibrateRezult.minMiddle}");
                    MessageBox.Show($"Min Ring: {CalibrateRezult.minRing}");
                    MessageBox.Show($"Min Pinky: {CalibrateRezult.minPinky}");
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorException(ex,cancellationToken);
                return false;
            }
        }
        private void SendToMirro(string data,SerialPort port)
        {
            port.Write(data);
        }
        public List<List<double>> CollectCalibrationData(List<List<double>> calibrationData, CancellationTokenSource cancellationToken,SerialPort port)
        {
            try
            {
                double startTime = Environment.TickCount;
                string messageString = "ping";
                byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
                client.Send(messageBytes, messageBytes.Length, remoteEP);

                while (Environment.TickCount - startTime < calibrationDuration * 1000)
                {
                    try
                    {
                        byte[] receiveBytes = client.Receive(ref remoteEP);
                        string receiveString = Encoding.ASCII.GetString(receiveBytes).TrimEnd('\0');
                        var receiveData = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonModel>(receiveString);

                        if (receiveData != null && receiveData.type == "position")
                        {

                            double angThumb = Math.Round(receiveData.data.fingers[0].ang[0], 1);
                            double angIndex = Math.Round(receiveData.data.fingers[1].ang[0], 1);
                            double angMiddle = Math.Round(receiveData.data.fingers[2].ang[0], 1);
                            double angRing = Math.Round(receiveData.data.fingers[3].ang[0], 1);
                            double angPinky = Math.Round(receiveData.data.fingers[4].ang[0], 1);

                            #region if

                            #endregion
                            (angThumb, angIndex, angMiddle, angRing, angPinky) = MaxMin(angThumb, angIndex, angMiddle, angRing, angPinky);

                            calibrationData.Add(new List<double> { angThumb, angIndex, angMiddle, angRing, angPinky });
                            string data = $"${myDictThumb[angThumb]}{myDictIndex[angIndex]}{myDictIndex[angMiddle]}{myDict[angRing]}{myDict[angPinky]}";
                            SendToMirro(data, port);
                        }
                    }
                    catch (SocketException e)
                    {
                       
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorException(ex, cancellationToken);
                throw new Exception(ex.Message);
            }

            return calibrationData;
        }

        public (double, double, double, double, double) CalculateAverages(List<List<double>> calibrationData)
        {
            if (calibrationData.Count == 0)
            {
                return (0, 0, 0, 0, 0);
            }

            int numSamples = calibrationData[0].Count;
            int numMeasurements = calibrationData.Count;
            List<double> averages = new List<double>();

            for (int i = 0; i < numSamples; i++)
            {
                double sum = 0;
                for (int j = 0; j < numMeasurements; j++)
                {
                    sum += calibrationData[j][i];
                }
                averages.Add(sum / numMeasurements);
            }

            return (Math.Round(averages[0], 1), Math.Round(averages[1], 1), Math.Round(averages[2], 1), Math.Round(averages[3], 1), Math.Round(averages[4], 1));
        }

        public void StartTracking(CancellationTokenSource cancellationToken,SerialPort port)
        {
            new Thread(() => Ping()).Start();

            while (true)
            {
                try
                {

                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
                    byte[] receiveBytes = client.Receive(ref remoteEP);
                    string receiveString = Encoding.ASCII.GetString(receiveBytes).TrimEnd('\0');
                    var receiveData = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonModel>(receiveString);

                    if (receiveData != null && receiveData.type == "position")
                    {
                        ProcessPosition(receiveData,port);
                    }

                }
                catch (SocketException e)
                {
                    ErrorException(e, cancellationToken);
                    break;
                }
            }
        }
        private void ErrorException(Exception e, CancellationTokenSource cancellationToken)
        {

            if (e.Message.Contains("Удаленный хост принудительно разорвал существующее подключение"))
            {
                MessageBox.Show("Убедитесь, что перчатка Senso подключена.");
                message = "Убедитесь, что перчатка Senso подключена.";
            }
            else if(e.Message.Contains("Превышен таймаут семафора"))
            {
                message = "Убедитесь, что контроллер подключен.";
            }
            else
            {
                MessageBox.Show($"SocketException: {e.Message}");
            }
            cancellationToken.Cancel();
           

        }

    }
}
