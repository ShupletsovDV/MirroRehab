using MRTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public class PositionProcessor : IPositionProcessor
    {
        public void ProcessPosition(dynamic receiveData, ISerialPortService serialPortService)
        {
            double angThumb = Math.Round(receiveData.data.fingers[0].ang[0], 1);
            double angIndex = Math.Round(receiveData.data.fingers[1].ang[0], 1);
            double angMiddle = Math.Round(receiveData.data.fingers[2].ang[0], 1);
            double angRing = Math.Round(receiveData.data.fingers[3].ang[0], 1);
            double angPinky = Math.Round(receiveData.data.fingers[4].ang[0], 1);

            (angThumb, angIndex, angMiddle, angRing, angPinky) = MaxMin(angThumb, angIndex, angMiddle, angRing, angPinky);

            string data = $"${Dictionaries.MyDictThumb[angThumb]}{Dictionaries.MyDictIndex[angIndex]}{Dictionaries.MyDictIndex[angMiddle]}{Dictionaries.MyDict[angRing]}{Dictionaries.MyDict[angPinky]}";

            serialPortService.SendData(data);
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
