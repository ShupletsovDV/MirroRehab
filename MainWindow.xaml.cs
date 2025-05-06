
using Microsoft.Win32;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MRTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
           /* MainWind.Visibility = Visibility.Hidden;
            mediaPlayer.Open(new Uri("E:\\Download\\ssstik.io_1723799913817.wav"));
            mediaPlayer.Play();

            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;*/


        }
        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            // Когда музыка закончила играть, показываем MainWind
            MainWind.Visibility = Visibility.Visible;
            ww.Visibility = Visibility.Hidden;

            // Не забудьте отключить обработчик события, если он больше не нужен
            mediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
        }
        private void MainWind_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
