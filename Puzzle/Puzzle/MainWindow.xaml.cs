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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _mouseFirstPoint;

        private static Random _random;

        public MainWindow()
        {
            InitializeComponent();
            _mouseFirstPoint = Mouse.GetPosition(this);
            _random = new Random();
            test();
        }

        private void test()
        {
            Image image = new Image();
            BitmapImage bitmapImage = new BitmapImage(new Uri(@"/Resources/kwiatek.jpg", UriKind.Relative));
            image.Source = bitmapImage;
            image.Name = "testImage1";
            image.Width = image.Height = 120;

            image.AddHandler(Image.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Puzzle_MouseLeftButtonDown));
            image.AddHandler(Image.MouseMoveEvent, new MouseEventHandler(Puzzle_MouseMove));
            image.AddHandler(Image.MouseUpEvent, new MouseButtonEventHandler(Puzzle_MouseUp));

            canvasRoot.Children.Add(image);

            Canvas.SetLeft(image, _random.Next(0, (int)(Application.Current.MainWindow.Width - image.Width)));
            Canvas.SetTop(image, _random.Next(0, (int)(Application.Current.MainWindow.Height - image.Height)));
        }

        private void Puzzle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            _mouseFirstPoint = e.GetPosition(this);
            Image image = sender as Image;
            image.CaptureMouse();
        }

        private void Puzzle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point tmpPoint = e.GetPosition(this);
                Point offset = new Point(_mouseFirstPoint.X - tmpPoint.X, _mouseFirstPoint.Y - tmpPoint.Y);

                Image image = sender as Image;
                
                Canvas.SetLeft(image, Canvas.GetLeft(image) - offset.X);
                Canvas.SetTop(image, Canvas.GetTop(image) - offset.Y);

                _mouseFirstPoint = tmpPoint;
            }
        }

        private void Puzzle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Image image = sender as Image;
            image.ReleaseMouseCapture();       
        }
    }
}
