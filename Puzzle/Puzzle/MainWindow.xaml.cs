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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Puzzle.Class;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Określa czy puzzle zostały ułożone
        /// </summary>
        private bool _isSolved;

        /// <summary>
        /// Określa czy można przesunąć kawałek puzzli lub klaster
        /// </summary>
        private bool _canMovePiece;

        /// <summary>
        /// Przechowuje poprzednią pozycje myszki
        /// </summary>
        private Point _previousMousePosition;

        /// <summary>
        /// Przechowuj obrazek z którego generujemy puzzle
        /// </summary>
        private BitmapImage _sourcePicture;

        /// <summary>
        /// Przechowuje obrazek stanowiący tło
        /// </summary>
        private BitmapImage _backgroundImage;

        /// <summary>
        /// Aktualnie przenoszony klaster
        /// </summary>
        private Cluster _currentCluster;

        /// <summary>
        /// Przechowuje wszystkie klastry puzzli
        /// </summary>
        private List<Cluster> _clusters;

        private int Zindex = 1;

        private Random _random;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _previousMousePosition = Mouse.GetPosition(this);
            _sourcePicture = new BitmapImage(new Uri(@"/Resources/kwiatek.jpg", UriKind.Relative));
            _backgroundImage = new BitmapImage(new Uri(@"/Resources/wood.png", UriKind.Relative));
            _isSolved = false;
            _canMovePiece = false;
            _random = new Random();

            Piece piece = new Piece()
            {
                Id = 1,
                AdjacentPieceIDs = new List<int>(){2},
                ClusterId = 1,
                Height = 100,
                Width = 100,
                Picture = _sourcePicture,
                Position = new Coordinate(0.0, 0.0)
            };
            InitPiece(piece);
            InitPiece(piece);
        }

        private void InitPiece(Piece piece)
        {
            Image image = new Image
            {
                Source = piece.Picture,
                Width = piece.Width,
                Height = piece.Height,
            };
            
            image.AddHandler(Image.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Piece_MouseLeftButtonDown));
            image.AddHandler(Image.MouseMoveEvent, new MouseEventHandler(Piece_MouseMove));
            image.AddHandler(Image.MouseUpEvent, new MouseButtonEventHandler(Piece_MouseUp));

            canvasRoot.Children.Add(image);

            Canvas.SetLeft(image, _random.Next(0, (int)(Application.Current.MainWindow.Width - image.Width)));
            Canvas.SetTop(image, _random.Next(0, (int)(Application.Current.MainWindow.Height - image.Height)));
        }

        private void Piece_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            Image image = sender as Image;
            Panel.SetZIndex(image, int.MaxValue);

            _previousMousePosition = e.GetPosition(this);
            _canMovePiece = true;                   
            image.Effect = new DropShadowEffect()
            {
                Color = new Color() {A = 2, R = 0, G = 0, B = 0},
                Direction = 315,
                ShadowDepth = 2.0d,
                Opacity = 2,
                
            };
            image.CaptureMouse();
        }

        private void Piece_MouseMove(object sender, MouseEventArgs e)
        {
            if (_canMovePiece && e.LeftButton == MouseButtonState.Pressed)
            {
                Point tmpPoint = e.GetPosition(this);
                Point offset = new Point(_previousMousePosition.X - tmpPoint.X, _previousMousePosition.Y - tmpPoint.Y);

                Image image = sender as Image;
                
                Canvas.SetLeft(image, Canvas.GetLeft(image) - offset.X);
                Canvas.SetTop(image, Canvas.GetTop(image) - offset.Y);

                _previousMousePosition = tmpPoint;
            }
        }

        private void Piece_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Image image = sender as Image;

            Panel.SetZIndex(image, Zindex++);
            _canMovePiece = false; 
            image.ClearValue(EffectProperty);
            image.ReleaseMouseCapture();       
        }
    }
}
