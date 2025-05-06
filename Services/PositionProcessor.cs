using MathNet.Spatial.Euclidean;
using MRTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Spatial.Units;
using System.Reflection;
using MRTest.ViewModels;

namespace MRTest.Services
{   //указательный, инверсия

    public class PositionProcessor : IPositionProcessor
    {

        private double AngleFinger(Finger finger, List<double> palmQuat)
        {
            var fingerQuat = finger.quat;
            var palmMatrix = QuaternionClass.QuaternionToMatrix(palmQuat);
            var fingerMatrix = QuaternionClass.QuaternionToMatrix(fingerQuat);

            // Calculate the relative rotation directly between finger and palm
            var relativeRotationMatrix = QuaternionClass.MultiplyMatrices(QuaternionClass.TransposeMatrix(palmMatrix), fingerMatrix);

            // Extract the bend angle, ensuring we are only considering the relative rotation
            return QuaternionClass.ExtractBendAngle(relativeRotationMatrix);
        }

        public void ProcessPosition(JsonModel receiveData, ISerialPortService serialPortService)
        {
            try
            {
                /* var palmQuat = receiveData.data.palm.quat;

                 double angThumb1 = Math.Round(AngleFinger(receiveData.data.fingers[0], palmQuat), 2);
                 double angIndex1 = Math.Round(AngleFinger(receiveData.data.fingers[1], palmQuat), 1);
                 double angMiddle1 = Math.Round(AngleFinger(receiveData.data.fingers[2], palmQuat), 1);
                 double angRing1 = Math.Round(AngleFinger(receiveData.data.fingers[3], palmQuat), 1);
                 double angPinky1 = Math.Round(AngleFinger(receiveData.data.fingers[4], palmQuat), 1);{angThumb1}   {angIndex1}  {angMiddle1}  {angRing1}  {angPinky1}*/


                double angThumb = Math.Round(receiveData.data.fingers[0].ang[0], 2); //-0.9    0.9  на новой прошивке
                double angIndex = Math.Round(receiveData.data.fingers[1].ang[0], 1);
                double angMiddle = Math.Round(receiveData.data.fingers[2].ang[0], 1);
                double angRing = Math.Round(receiveData.data.fingers[3].ang[0], 1);
                double angPinky = Math.Round(receiveData.data.fingers[4].ang[0], 1);
               
                (angThumb, angIndex, angMiddle, angRing, angPinky) = MaxMin(angThumb, angIndex, angMiddle, angRing, angPinky);

                string data = "";
                string type = "";
                string fingers = "";
                if (receiveData.data.type == "lh")
                {
                    type = "right mirro hand";
                    fingers = $"Угол Senso: {angIndex},{angMiddle},{angRing},{angPinky}";
                    data = $"{Dictionaries.DictIndex[angIndex]},{Dictionaries.DictMiddle[angMiddle]},{Dictionaries.DictRing[angRing]},{Dictionaries.DictPinky[angPinky]},0";
                }
                else
                {
                    type = "left mirro hand";
                    fingers = $"Угол Senso: {angPinky},{angRing},{angMiddle},{angIndex}";
                    data = $"{Dictionaries.DictPinkyRight[angPinky]},{Dictionaries.DictRingRight[angRing]},{Dictionaries.DictMiddleRight[angMiddle]},{Dictionaries.DictIndexRight[angIndex]},0";
                }
                
                string test = $"\n{fingers}\nУгол Mirro: {data}\n{type}";
                serialPortService.SendData(data);
                Notifications.GetNotifications().InvokeCommonStatus(test, Notifications.NotificationEvents.PositionProcessor);
            }
            catch(Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при обработке данных SENSO: {ex}");

                throw new InvalidOperationException(ex.Message);
                //throw new Exception();
            }

        }
        
        private (double, double, double, double, double) MaxMin(double angThumb, double angIndex, double angMiddle, double angRing, double angPinky)
        {
            if (angThumb < 0)
            {
                angThumb = 0.00;
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
