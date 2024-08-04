using MRTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MRTest
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private string port;
        HandController1 handController = HandController1.GetHandController();
        public SettingsWindow(string com)
        {
            InitializeComponent();
            try
            {
                port = com;
                handController.OpenPort(com);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
           
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //Большой палец разгиб
        {
            try
            {
                handController.SendPort($"{new byte[0]},{new byte[0]},{new byte[0]},{new byte[0]},{e.NewValue}");
                CalibrateRezult.minThumb = Dictionaries.MyDictThumbCalibrate[Convert.ToInt32(e.NewValue)][0];
            }
            catch
            {

            }
            
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e) //Большой палец сгиб
        {
            try
            {
                handController.SendPort($"{new byte[0]},{new byte[0]},{new byte[0]},{new byte[0]},{e.NewValue}");
                CalibrateRezult.maxThumb = Dictionaries.MyDictThumbCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
       
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e) //Указательный палец разгиб
        {
            try
            {
                handController.SendPort($"{e.NewValue}");
                CalibrateRezult.minIndex = Dictionaries.MyDictCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
           
        }

        private void Slider_ValueChanged_3(object sender, RoutedPropertyChangedEventArgs<double> e)//Указательный палец сгиб
        {
            try
            {
                handController.SendPort($"{e.NewValue}");
                CalibrateRezult.maxIndex = Dictionaries.MyDictCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
           
        }

        private void Slider_ValueChanged_4(object sender, RoutedPropertyChangedEventArgs<double> e)//Средний палец разгиб
        {
            try
            {
                handController.SendPort($"{new byte[0]},{e.NewValue}");
                CalibrateRezult.minMiddle = Dictionaries.MyDictCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
            
        }

        private void Slider_ValueChanged_5(object sender, RoutedPropertyChangedEventArgs<double> e)//Средний палец сгиб
        {
            try
            {
                handController.SendPort($"{new byte[0]},{e.NewValue}");
                CalibrateRezult.maxMiddle = Dictionaries.MyDictCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
        }

        private void Slider_ValueChanged_6(object sender, RoutedPropertyChangedEventArgs<double> e) //Безымянный палец разгиб
        {
            try
            {
                handController.SendPort($"{new byte[0]},{new byte[0]},{e.NewValue}");
                CalibrateRezult.minRing = Dictionaries.MyDictCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
        }

        private void Slider_ValueChanged_7(object sender, RoutedPropertyChangedEventArgs<double> e)//Безымянный палец сгиб
        {
            try
            {
                handController.SendPort($"{new byte[0]},{new byte[0]},{new byte[0]},{e.NewValue}");
                CalibrateRezult.maxRing = Dictionaries.MyDictCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
        }

        private void Slider_ValueChanged_8(object sender, RoutedPropertyChangedEventArgs<double> e)//Мизинец палец разгиб
        {
            try
            {
                handController.SendPort($"{new byte[0]},{new byte[0]},{new byte[0]},{new byte[0]},{e.NewValue}");
                CalibrateRezult.minPinky = Dictionaries.MyDictCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
        }

        private void Slider_ValueChanged_9(object sender, RoutedPropertyChangedEventArgs<double> e)//Мизинец палец сгиб
        {
            try
            {
                handController.SendPort($"{new byte[0]},{new byte[0]},{new byte[0]},{new byte[0]},{e.NewValue}");
                CalibrateRezult.maxPinky = Dictionaries.MyDictCalibrate[int.Parse(e.NewValue.ToString())][0];
            }
            catch
            {

            }
        }
    }
}
