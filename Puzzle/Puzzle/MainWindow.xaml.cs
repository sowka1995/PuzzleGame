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
        #region Variables

        /// <summary>
        /// Określa czy można przesunąć kawałek puzzli lub klaster
        /// </summary>
        private bool _canMovePiece;

        /// <summary>
        /// Przechowuje poprzednią pozycje myszki
        /// </summary>
        private Point _previousMousePosition;

        /// <summary>
        /// Aktualnie przenoszony klaster
        /// </summary>
        private Cluster _currentCluster;

        /// <summary>
        /// Przechowuje wszystkie klastry puzzli
        /// </summary>
        private List<Cluster> _clusters;

        /// <summary>
        /// Służy do generowania losowej pozycji dla kawałka puzzli
        /// </summary>
        private Random _random;

        /// <summary>
        /// Przechowuje obrazek z którego generujemy puzzle
        /// </summary>
        private BitmapImage _sourcePicture;

        #endregion 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        #region Events

        private void PieceCluster_MouseMove(object sender, MouseEventArgs e)
        {
            if (_canMovePiece && e.LeftButton == MouseButtonState.Pressed)
            {
                Point tmpPosition = e.GetPosition(this);
                Point offset = new Point(_previousMousePosition.X - tmpPosition.X, _previousMousePosition.Y - tmpPosition.Y);

                foreach (Piece piece in _currentCluster.Pieces)
                {
                    Canvas.SetLeft(piece.PieceImage, Canvas.GetLeft(piece.PieceImage) - offset.X);
                    Canvas.SetTop(piece.PieceImage, Canvas.GetTop(piece.PieceImage) - offset.Y);
                }

                _previousMousePosition = tmpPosition;
            }
        }

        private void PieceCluster_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            Image pieceImage = sender as Image;

            int pieceId = int.Parse(pieceImage.Name.Substring(1)); // id z nazwy 
            _previousMousePosition = e.GetPosition(this);
            _canMovePiece = true;

            foreach (Cluster cluster in _clusters)
            {
                foreach (Piece piece in cluster.Pieces)
                {
                    if (piece.Id == pieceId)
                    {
                        _currentCluster = cluster;
                        break;
                    }
                }
            }
        }

        private void PieceCluster_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;

            if (_canMovePiece)
            {
                // TODO: łączenie puzzli

                _canMovePiece = false;
            }
        }

        #endregion

        #region HelperMethods

        private void CreateAndDisplayPuzzle()
        {
            int pieceId = 0;
            int pieceCount = 0;

            var pieces = Engine.CutImageToPieces(_sourcePicture);

            for (int row = 0; row < PuzzleSettings.NUM_ROWS; row++)
            {
                for (int col = 0; col < PuzzleSettings.NUM_COLUMNS; col++)
                {
                    List<Coordinate> adjacentCoordinates = new List<Coordinate>()
                    {
                        new Coordinate(col + 1, row),
                        new Coordinate(col, row + 1),
                        new Coordinate(col - 1, row),
                        new Coordinate(col, row - 1)
                    };

                    List<int> adjacentPieceIDs = Engine.DetermineAdjacentPieceIDs(adjacentCoordinates);

                    Piece piece = new Piece()
                    {
                        Id = pieceId,
                        ClusterId = pieceId,
                        AdjacentPieceIDs = adjacentPieceIDs,
                        Location = new Coordinate(col, row),
                        Height = Convert.ToInt32(pieces[pieceCount].Height),
                        Width = Convert.ToInt32(pieces[pieceCount].Width),
                        Picture = pieces[pieceCount],
                    };

                    InitPiece(piece);

                    Cluster cluster = new Cluster()
                    {
                        Id = pieceId,
                        Pieces = new List<Piece>() { piece }
                    };

                    _clusters.Add(cluster);

                    pieceId++;
                    pieceCount++;
                }
            }
        }

        private void InitPiece(Piece piece)
        {
            Image pieceImage = new Image
            {
                Source = piece.Picture,
                Width = piece.Width,
                Height = piece.Height,
                Name = "p" + piece.Id
            };

            pieceImage.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(PieceCluster_MouseLeftButtonDown));
            pieceImage.AddHandler(MouseMoveEvent, new MouseEventHandler(PieceCluster_MouseMove));
            pieceImage.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PieceCluster_MouseUp));

            canvasRoot.Children.Add(pieceImage);
            piece.PieceImage = pieceImage;

            Canvas.SetLeft(pieceImage, _random.Next(0, (int)(mainGrid.ActualWidth - pieceImage.Width)));
            Canvas.SetTop(pieceImage, _random.Next(0, (int)(mainGrid.ActualHeight - pieceImage.Height)));
        }

        #endregion
    }
}
