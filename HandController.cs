using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Windows;
using System.ComponentModel;

namespace MRTest
{
    internal class HandController
    {
        private object lockObject = new object();
        public static bool stopThread = false;
        private string exceptionMessage = "";
        private string message = "";

        BackgroundWorker worker = new BackgroundWorker();

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
        private double maxIndex = 2.6;
        private double maxThumb = 1.1;
        private double maxPinky = 2.4;
        private double maxRing = 2.6;
        private double maxMiddle = 2.6;

        private double minIndex = 0.0;
        private double minThumb = 0.0;
        private double minPinky = 0.0;
        private double minRing = 0.0;
        private double minMiddle = 0.0;

        private int calibrationDuration = 5;
        private List<List<double>> maxCalibrationData = new List<List<double>>();
        private List<List<double>> minCalibrationData = new List<List<double>>();

        private UdpClient client;

        public HandController()
        {
            client = new UdpClient();
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
            client.Send(new byte[0], 0, remoteEP);

            worker.DoWork += WorkerOnDoWork;
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += WorkerOnProgressChanged;
        }
        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            
            worker.ReportProgress(0); 
            Thread.Sleep(2000);
        }

        private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            new InfoWindow().ShowDialog();
        }
        public void Ping()
        {
            while (true)
            {
                if (!stopThread)
                {
                    Thread.Sleep(10);
                    string messageString = "ping";
                    byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
                    client.Send(messageBytes, messageBytes.Length, remoteEP);
                 
                }
                else
                {
                    break;
                }
            }
        }

        public void Send(List<double> fingers)
        {
            // Implement your logic for sending data
        }

        public void ProcessPosition(dynamic receiveData, string com)
        {
            double angThumb = Math.Round(receiveData.data.fingers[0].ang[0] * 10) / 10;
            double angIndex = Math.Round(receiveData.data.fingers[1].ang[0] * 10) / 10;
            double angMiddle = Math.Round(receiveData.data.fingers[2].ang[0] * 10) / 10;
            double angRing = Math.Round(receiveData.data.fingers[3].ang[0] * 10) / 10;
            double angPinky = Math.Round(receiveData.data.fingers[4].ang[0] * 10) / 10;

            // Implement your logic for processing position
            // Assuming you have the necessary variables defined, such as maxThumb, maxIndex, max, maxPinky, maxRing, my_dictThumb, my_dictIndex, my_dict
            #region if
            if (angThumb < 0)
            {
                angThumb = 0.0;
            }
            if (angThumb > maxThumb)
            {
                angThumb = maxThumb;
            }

            if (angIndex < 0)
            {
                angIndex = 0.0;
            }
            if (angIndex > maxIndex)
            {
                angIndex = maxIndex;
            }

            if (angMiddle < 0)
            {
                angMiddle = 0.0;
            }
            if (angMiddle > max)
            {
                angMiddle = max;
            }

            if (angPinky < 0)
            {
                angPinky = 0.0;
            }
            if (angPinky > maxPinky)
            {
                angPinky = maxPinky;
            }

            if (angRing < 0)
            {
                angRing = 0.0;
            }
            if (angRing > maxRing)
            {
                angRing = maxRing;
            }
#endregion

            List<int> fings = new List<int>
            {
                myDictThumb[angThumb],
                myDictIndex[angIndex],
                myDictIndex[angMiddle],
                myDict[angRing],
                myDict[angPinky]
            };


            MessageBox.Show($"{angThumb} {angIndex} {angMiddle} {angRing} {angPinky}");
        }

        public void CalibrateDeviceMax(string com)
        {
            message = "Сейчас сожмите руку и удерживайте в течении 5 секунд";
            MessageBox.Show(message);
            //worker.RunWorkerAsync(message);
            maxCalibrationData = CollectCalibrationData(com, maxCalibrationData, max);
            (maxThumb, maxIndex, maxMiddle, maxRing, maxPinky) = CalculateAverages(maxCalibrationData);
            //worker.RunWorkerAsync(message);
            message = "Калибровка завершена";
            MessageBox.Show(message);
            MessageBox.Show($"Max Thumb: {maxThumb}");
            MessageBox.Show($"Max Index: {maxIndex}");
            MessageBox.Show($"Max Middle: {maxMiddle}");
            MessageBox.Show($"Max Ring: {maxRing}");
            MessageBox.Show($"Max Pinky: {maxPinky}");
        }
        public void CalibrateDeviceMin(string com)
        {
            
           

            message = "Разожмите руку и удерживайте в течении 5 секунд";
            //worker.RunWorkerAsync(message);
            MessageBox.Show(message);

            minCalibrationData = CollectCalibrationData(com, minCalibrationData, min);
            (minThumb, minIndex, minMiddle, minRing, minPinky) = CalculateAverages(minCalibrationData);

           
           
            MessageBox.Show($"Min Thumb: {minThumb}");
            MessageBox.Show($"Min Index: {minIndex}");
            MessageBox.Show($"Min Middle: {minMiddle}");
            MessageBox.Show($"Min Ring: {minRing}");
            MessageBox.Show($"Min Pinky: {minPinky}");
        }

        public List<List<double>> CollectCalibrationData(string com, List<List<double>> calibrationData, double maxValue)
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
                   
                    if (receiveData!=null && receiveData.type == "position")
                    {
                        double angThumb = Math.Round(receiveData.data.fingers[0].ang[0] * 10) / 10;
                        double angIndex = Math.Round(receiveData.data.fingers[1].ang[0] * 10) / 10;
                        double angMiddle = Math.Round(receiveData.data.fingers[2].ang[0] * 10) / 10;
                        double angRing = Math.Round(receiveData.data.fingers[3].ang[0] * 10) / 10;
                        double angPinky = Math.Round(receiveData.data.fingers[4].ang[0] * 10) / 10;

                        calibrationData.Add(new List<double> { angThumb, angIndex, angMiddle, angRing, angPinky });
                       
                    }
                }
                catch (SocketException e)
                {
                    MessageBox.Show($"SocketException: {e}");
                    break;
                }
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

            return (averages[0], averages[1], averages[2], averages[3], averages[4]);
        }

        public void StartTracking(string com)
        {
            new Thread(Ping).Start();

            while (true)
            {
                try
                {
                    if (!stopThread)
                    {
                        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53450);
                        byte[] receiveBytes = client.Receive(ref remoteEP);
                        string receiveString = Encoding.ASCII.GetString(receiveBytes).TrimEnd('\0');
                        var receiveData = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonModel>(receiveString);

                        if (receiveData!=null && receiveData.type == "position")
                        {
                            ProcessPosition(receiveData, com);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (SocketException e)
                {
                    MessageBox.Show($"SocketException: {e}");
                    break;
                }
            }
        }

    }
}
