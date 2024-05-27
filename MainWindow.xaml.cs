using HelixToolkit.Wpf;
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
        public MainWindow()
        {
            InitializeComponent();
            LoadModel();
        }
        private void LoadModel()
        {
            var handModel = new Model3DGroup();

            // Ладонь
            var palm = CreatePalm();
            handModel.Children.Add(palm);

            // Пальцы
            var finger1 = CreateFinger(0.6, 1, 0, 1);
            finger1.Transform = new TranslateTransform3D(0.5, 0.5, 0);
            handModel.Children.Add(finger1);

            var finger2 = CreateFinger(0.6, 1, 0, 1);
            finger2.Transform = new TranslateTransform3D(0.3, 0.5, 0);
            handModel.Children.Add(finger2);

            var finger3 = CreateFinger(0.6, 1, 0, 1);
            finger3.Transform = new TranslateTransform3D(0.1, 0.5, 0);
            handModel.Children.Add(finger3);

            var finger4 = CreateFinger(0.6, 1, 0, 1);
            finger4.Transform = new TranslateTransform3D(-0.1, 0.5, 0);
            handModel.Children.Add(finger4);

            var thumb = CreateFinger(0.4, 0.8, 0, 1);
            thumb.Transform = new TranslateTransform3D(0.6, 0.3, 0);
            handModel.Children.Add(thumb);

            // Присваиваем модель 3D
            modelVisual.Content = handModel;
        }
        private GeometryModel3D CreatePalm()
        {
            var mesh = new MeshGeometry3D
            {
                Positions = new Point3DCollection(new[]
                {
                    new Point3D(-1, -1, 0),
                    new Point3D(1, -1, 0),
                    new Point3D(1, 1, 0),
                    new Point3D(-1, 1, 0),
                    new Point3D(-1, -1, 0),
                    new Point3D(1, 1, 0)
                }),
                TriangleIndices = new Int32Collection(new[] { 0, 1, 2, 0, 2, 3 })
            };

            return new GeometryModel3D
            {
                Geometry = mesh,
                Material = new DiffuseMaterial(new SolidColorBrush(Colors.Beige))
            };
        }

        private GeometryModel3D CreateFinger(double radius, double height, double centerX, double centerY)
        {
            var meshBuilder = new MeshBuilder();
            meshBuilder.AddCylinder(new Point3D(centerX, centerY, 0), new Point3D(centerX, centerY, height), radius, 36);

            var mesh = meshBuilder.ToMesh();

            return new GeometryModel3D
            {
                Geometry = mesh,
                Material = new DiffuseMaterial(new SolidColorBrush(Colors.Beige))
            };
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
