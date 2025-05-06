
using MRTest.Services;
using MRTest.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;


namespace MRTest
{
    public partial class SettingsWindow : Window
    {
        private string port;
        SerialPortService handController = SerialPortService.GetInstance();
        private double lastIndex = 0;
        private double lastMiddle = 0;
        private double lastRing = 0;
        private double lastPinky = 0;

        public SettingsWindow(SerialPortService com)
        {
            InitializeComponent();
            this.handController = com;

            // Инициализация слайдеров для левой руки с минимальными значениями
            // Index – нормальный режим
            IndexSgib.Minimum = 0;
            IndexSgib.Maximum = 170;
            IndexSgib.Value = Dictionaries.MaxIndex/2; // Минимальный угол для начального положения

            IndexRasgib.Minimum = 0;
            IndexRasgib.Maximum = Dictionaries.MinIndex;
            IndexRasgib.Value = Dictionaries.MinIndex; // Минимальный угол

            // Middle – реверс
            MiddleSgib.Minimum = 0;
            MiddleSgib.Maximum = 170;
            MiddleSgib.Value = 170 - Dictionaries.MinMiddle/2; // Минимальный угол (120 градусов)

            MiddleRasgib.Minimum = 0;
            MiddleRasgib.Maximum = 170 - Dictionaries.MinMiddle;
            MiddleRasgib.Value = 170 - Dictionaries.MinMiddle; // Минимальный угол

            // Ring – нормальный режим
            RingSgib.Minimum = 0;
            RingSgib.Maximum = 170;
            RingSgib.Value = Dictionaries.MaxRing/2; // Минимальный угол

            RingRasgib.Minimum = 0;
            RingRasgib.Maximum = Dictionaries.MinRing;
            RingRasgib.Value = Dictionaries.MinRing; // Минимальный угол

            // Pinky – реверс
            PinkySgib.Minimum = 0;
            PinkySgib.Maximum = 170;
            PinkySgib.Value = 170 - Dictionaries.MinPinky/2; // Минимальный угол (120 градусов)

            PinkyRasgib.Minimum = 0;
            PinkyRasgib.Maximum = 170 - Dictionaries.MinPinky;
            PinkyRasgib.Value = 170 - Dictionaries.MinPinky; // Минимальный угол

            // Установка пальцев в минимальные положения
            lastIndex = Dictionaries.MinIndex;
            lastMiddle = Dictionaries.MinMiddle;
            lastRing = Dictionaries.MinRing;
            lastPinky = Dictionaries.MinPinky;
            handController.SendData($"{lastIndex},{lastMiddle},{lastRing},{lastPinky},0");
        }

        ~SettingsWindow()
        {
            handController.ClosePort();
        }

        public static void WriteToJsonFile(string filePath, FingerSettings settings)
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FingerSettings settings = new FingerSettings
                {
                    MaxIndex = Dictionaries.MaxIndex,
                    MaxMiddle = Dictionaries.MaxMiddle,
                    MaxRing = Dictionaries.MaxRing,
                    MaxPinky = Dictionaries.MaxPinky,

                    MinIndex = Dictionaries.MinIndex,
                    MinMiddle = Dictionaries.MinMiddle,
                    MinRing = Dictionaries.MinRing,
                    MinPinky = Dictionaries.MinPinky,

                    MaxIndexRight = Dictionaries.MaxIndexRight,
                    MaxMiddleRight = Dictionaries.MaxMiddleRight,
                    MaxRingRight = Dictionaries.MaxRingRight,
                    MaxPinkyRight = Dictionaries.MaxPinkyRight,

                    MinIndexRight = Dictionaries.MinIndexRight,
                    MinMiddleRight = Dictionaries.MinMiddleRight,
                    MinRingRight = Dictionaries.MinRingRight,
                    MinPinkyRight = Dictionaries.MinPinkyRight
                };

                WriteToJsonFile("settings.json", settings);

                // ЛЕВАЯ РУКА
                Dictionaries.DictIndex = Dictionaries.RedistributeValues(Dictionaries.MyDict, Dictionaries.MinIndex, Dictionaries.MaxIndex);
                Dictionaries.DictMiddle = Dictionaries.RedistributeValues(Dictionaries.MyDictReverse, Dictionaries.MinMiddle, Dictionaries.MaxMiddle, true);
                Dictionaries.DictRing = Dictionaries.RedistributeValues(Dictionaries.MyDict, Dictionaries.MinRing, Dictionaries.MaxRing);
                Dictionaries.DictPinky = Dictionaries.RedistributeValues(Dictionaries.MyDictReverse, Dictionaries.MinPinky, Dictionaries.MaxPinky, true);

                // ПРАВАЯ РУКА (симметрия: Index -> PinkyRight, Middle -> RingRight, Ring -> MiddleRight, Pinky -> IndexRight)
                Dictionaries.DictIndexRight = Dictionaries.RedistributeValues(Dictionaries.MyDictReverse, Dictionaries.MinIndexRight, Dictionaries.MaxIndexRight, true); // Pinky левой
                Dictionaries.DictMiddleRight = Dictionaries.RedistributeValues(Dictionaries.MyDict, Dictionaries.MinMiddleRight, Dictionaries.MaxMiddleRight); // Ring левой
                Dictionaries.DictRingRight = Dictionaries.RedistributeValues(Dictionaries.MyDictReverse, Dictionaries.MinRingRight, Dictionaries.MaxRingRight, true); // Middle левой
                Dictionaries.DictPinkyRight = Dictionaries.RedistributeValues(Dictionaries.MyDict, Dictionaries.MinPinkyRight, Dictionaries.MaxPinkyRight); // Index левой

                handController.ClosePort();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void IndexSgib_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = (int)e.NewValue;
                Dictionaries.MaxIndex = val;
                Dictionaries.MaxPinkyRight = val; // Index левой -> Pinky правой

                IndexRasgib.Minimum = 0;
                IndexRasgib.Maximum = val;
                IndexRasgib.Value = val;

                handController.SendData($"{val},{lastMiddle},{lastRing},{lastPinky},0");
                lastIndex = val;

                win.Title = val.ToString();
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при калибровке сгиба указательного: {ex}");

                MessageBox.Show(ex.Message);
            }
        }

        private void IndexRasgib_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = (int)e.NewValue;
                Dictionaries.MinIndex = val;
                Dictionaries.MinPinkyRight = val; // Index левой -> Pinky правой

                handController.SendData($"{val},{lastMiddle},{lastRing},{lastPinky},0");
                lastIndex = val;
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при калибровке разгиба указательного: {ex}");
                MessageBox.Show(ex.Message);
            }
        }

        private void MiddleSgib_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = 180 - (int)e.NewValue;
                Dictionaries.MaxMiddle = val;
                Dictionaries.MaxRingRight = val; // Middle левой -> Ring правой

                MiddleRasgib.Minimum = 0;
                MiddleRasgib.Maximum = 170 - val;
                MiddleRasgib.Value = 170 - val;

                handController.SendData($"{lastIndex},{val},{lastRing},{lastPinky},0");
                lastMiddle = val;
                win.Title = val.ToString();
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при калибровке сгиба среднего: {ex}");
                MessageBox.Show(ex.Message);
            }
        }

        private void MiddleRasgib_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = 170 - (int)e.NewValue;
                Dictionaries.MinMiddle = val;
                Dictionaries.MinRingRight = val; // Middle левой -> Ring правой

                handController.SendData($"{lastIndex},{val},{lastRing},{lastPinky},0");
                lastMiddle = val;
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при калибровке разгиба среднего: {ex}");
                MessageBox.Show(ex.Message);
            }
        }

        private void RingSgib_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = (int)e.NewValue;
                Dictionaries.MaxRing = val;
                Dictionaries.MaxMiddleRight = val; // Ring левой -> Middle правой

                RingRasgib.Minimum = 0;
                RingRasgib.Maximum = val;
                RingRasgib.Value = val;

                handController.SendData($"{lastIndex},{lastMiddle},{val},{lastPinky},0");
                lastRing = val;
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при калибровке сгиба безымянного: {ex}");
                MessageBox.Show(ex.Message);
            }
        }

        private void RingRasgib_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = (int)e.NewValue;
                Dictionaries.MinRing = val;
                Dictionaries.MinMiddleRight = val; // Ring левой -> Middle правой

                handController.SendData($"{lastIndex},{lastMiddle},{val},{lastPinky},0");
                lastRing = val;
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при калибровке разгиба безымянного: {ex}");
                MessageBox.Show(ex.Message);
            }
        }

        private void PinkySgib_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = 170 - (int)e.NewValue;
                Dictionaries.MaxPinky = val;
                Dictionaries.MaxIndexRight = val; // Pinky левой -> Index правой

                PinkyRasgib.Minimum = 0;
                PinkyRasgib.Maximum = 170 - val;
                PinkyRasgib.Value = 170 - val;

                handController.SendData($"{lastIndex},{lastMiddle},{lastRing},{val},0");
                lastPinky = val;
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при калибровке сгиба мизинца: {ex}");
                MessageBox.Show(ex.Message);
            }
        }

        private void PinkyRasgib_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int val = 170 - (int)e.NewValue;
                Dictionaries.MinPinky = val;
                Dictionaries.MinIndexRight = val; // Pinky левой -> Index правой

                handController.SendData($"{lastIndex},{lastMiddle},{lastRing},{val},0");
                lastPinky = val;
            }
            catch (Exception ex)
            {
                MainWindowViewModel.Log.Error($"\nОшибка при калибровке разгиба мизинца: {ex}");
                MessageBox.Show(ex.Message);
            }
        }
    }
}



//160 угол