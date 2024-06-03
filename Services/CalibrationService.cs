using MRTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MRTest.Services
{
    public class CalibrationService : ICalibrationService
    {

        public void CalibrateMax(ISerialPortService serialPortService, IUdpClientService _udpClientService)
        {
            try
            {
                List<List<double>> maxCalibrationData = new List<List<double>>();

                maxCalibrationData = CollectCalibrationData(maxCalibrationData, serialPortService,_udpClientService);
                (CalibrateRezult.maxThumb, CalibrateRezult.maxIndex, CalibrateRezult.maxMiddle, CalibrateRezult.maxRing, CalibrateRezult.maxPinky) = CalculateAverages(maxCalibrationData);
               
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, ex.InnerException);
            }
            catch
            {
                throw new Exception();
            }


        }

        public void CalibrateMin(ISerialPortService serialPortService, IUdpClientService _udpClientService)
        {

            try
            {
                List<List<double>> minCalibrationData = new();

                minCalibrationData = CollectCalibrationData(minCalibrationData, serialPortService, _udpClientService);
                (CalibrateRezult.minThumb, CalibrateRezult.minIndex, CalibrateRezult.minMiddle, CalibrateRezult.minRing, CalibrateRezult.minPinky) = CalculateAverages(minCalibrationData);
               
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, ex.InnerException);
            }
            catch
            {
                throw new Exception();
            }



        }

        private (double, double, double, double, double) CalculateAverages(List<List<double>> calibrationData)
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
        public List<List<double>> CollectCalibrationData(List<List<double>> calibrationData, ISerialPortService _serialPortService, IUdpClientService _udpClientService)
        {
            try
            {
                double startTime = Environment.TickCount;
                _udpClientService.StartPing();
                

                while (Environment.TickCount - startTime < 5 * 1000)
                {
                    try
                    {
                        var receiveData = _udpClientService.ReceiveData();

                        if (receiveData != null && receiveData.type == "position")
                        {
                            double angThumb = Math.Round(receiveData.data.fingers[0].ang[0], 2);
                            double angIndex = Math.Round(receiveData.data.fingers[1].ang[0], 1);
                            double angMiddle = Math.Round(receiveData.data.fingers[2].ang[0], 1);
                            double angRing = Math.Round(receiveData.data.fingers[3].ang[0], 1);
                            double angPinky = Math.Round(receiveData.data.fingers[4].ang[0], 1);

                            (angThumb, angIndex, angMiddle, angRing, angPinky) = MaxMin(angThumb, angIndex, angMiddle, angRing, angPinky);

                            calibrationData.Add(new List<double> { angThumb, angIndex, angMiddle, angRing, angPinky });
                            string data = $"{Dictionaries.MyDictThumb[angThumb]},{Dictionaries.MyDict[angIndex]},{Dictionaries.MyDict[angMiddle]},{Dictionaries.MyDict[angRing]},{Dictionaries.MyDict[angPinky]}\n";
                            _serialPortService.SendData(data);
                        }
                    }
                    catch(Exception ex)
                    {
                        throw new ArgumentException(ex.Message, ex);
                    }
                }
               
            }
            catch (Exception ex)
            {
               throw new ArgumentException(ex.Message, ex);
            }

            return calibrationData;
        }
        private (double, double, double, double, double) MaxMin(double angThumb, double angIndex, double angMiddle, double angRing, double angPinky) 
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
    }

}
