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
    /// Логика взаимодействия для InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            //info.Content = "1. Если устройство не найдено, нажмите на поиск устройства.\r\n2. Выберете нужное устройство\r\n3. Откалируйте его\r\n4. Нажмите на запустить";
            //infotxt.DataContext = "1. Если устройство не найдено, нажмите на поиск устройства.\r\n2. Выберете нужное устройство\r\n3. Откалируйте его\r\n4. Нажмите на запустить";

        }

        private void MainWind_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
