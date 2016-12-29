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

        /// <summary>
        /// Przechowuje referencję do okienka startowego aplikacji.
        /// </summary>
        private Window _menuWindow;

        /// <summary>
        /// Określa czy puzzle zostały ułożone
        /// </summary>
        private bool _isSolved;


        /// <summary>
        /// Służy do wyciągania na pierwszy plan przenoszonych kawałków puzzli
        /// </summary>
        private int _zindex = 1;


        #endregion 

        public MainWindow(Window menuWindow)
        {
            InitializeComponent();

            _menuWindow = menuWindow;
            _previousMousePosition = Mouse.GetPosition(mainGrid);
            _clusters = new List<Cluster>();
            _canMovePiece = false;
            _random = new Random();
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

            Engine.DropShadowEffect(_currentCluster);
        }

        private void PieceCluster_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Engine.DeleteShadowEffect(_currentCluster, ref _zindex);

            if (_canMovePiece && !_isSolved)
            {
                List<int> adjacentClusterIDs = new List<int>();

                // łączenie puzzli
                for (int i = 0; i < _currentCluster.Pieces.Count; i++)
                {
                    Piece currentPiece = _currentCluster.Pieces[i];

                    foreach (int pieceId in currentPiece.AdjacentPieceIDs)
                    {
                        Piece adjacentPiece = GetPieceById(pieceId);

                        if (adjacentPiece != null && adjacentPiece.ClusterId != currentPiece.ClusterId)
                        {
                            if (Engine.DetermineIfMergePieces(currentPiece, adjacentPiece))
                            {
                                Cluster adjacentCluster = GetClusterById(adjacentPiece.ClusterId);

                                adjacentClusterIDs.Add(adjacentCluster.Id);

                                // Aktualizacja ClusterId dla kawałków w sąsiędnim klastrze
                                foreach (Piece piece in adjacentCluster.Pieces)
                                {
                                    piece.ClusterId = currentPiece.ClusterId;
                                }

                                // usuwanie zbędnęgo już obracania kawałka puzzli po ich złączeniu (jeżeli złączono to muszą być już obrócone w dobrą stronę)
                                currentPiece.PieceImage.RemoveHandler(MouseWheelEvent, new MouseWheelEventHandler(PieceCluster_MouseWheel));
                                adjacentPiece.PieceImage.RemoveHandler(MouseWheelEvent, new MouseWheelEventHandler(PieceCluster_MouseWheel));
                            }
                        }
                    }
                }

                if (adjacentClusterIDs.Count > 0)
                {
                    foreach (int clusterId in adjacentClusterIDs)
                    {
                        Cluster adjacentCluster = GetClusterById(clusterId);

                        foreach (Piece piece in adjacentCluster.Pieces)
                        {
                            _currentCluster.Pieces.Add(piece);
                        }
                        Engine.AlignPiecesPositions(ref _currentCluster);

                        RemoveClusterById(clusterId);
                    }
                }

                if (_clusters.Count <= 1)
                {
                    MessageBox.Show("Gratulacje, ułożyłeś swoje puzzle!", "Ułożone!!!");
                    _isSolved = true;
                }

                _canMovePiece = false;
            }
        }

        private void PieceCluster_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Image pieceImage = sender as Image;

            if (pieceImage != null)
            {
                Matrix matrix = pieceImage.RenderTransform.Value;

                int pieceId = int.Parse(pieceImage.Name.Substring(1)); // id z nazwy 
                Piece piece = GetPieceById(pieceId);

                if (e.RightButton == MouseButtonState.Pressed)
                {
                    double centerX = piece.PieceImage.ActualWidth / 2;
                    double centerY = piece.PieceImage.ActualHeight / 2;

                    int sign = Math.Sign(e.Delta);

                    matrix.RotateAtPrepend(90 * sign, centerX, centerY);
                    if (piece.Rotation == 270 * sign)
                        piece.Rotation = 0;
                    else
                        piece.Rotation += 90 * sign;
                }

                MatrixTransform matrixTransform = new MatrixTransform(matrix);
                piece.PieceImage.RenderTransform = matrixTransform;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _menuWindow.Visibility = Visibility.Visible;
        }

        #endregion

        #region HelperMethods

        public void CreateAndDisplayPuzzle()
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
                        Height = pieces[pieceCount].Height,
                        Width = pieces[pieceCount].Width,
                        Picture = pieces[pieceCount],
                    };

                    InitPiece(piece);
                    Engine.RotatePieceRandom(ref piece);

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

        public void SetSourcePicture(BitmapImage sourcePicture)
        {
            _sourcePicture = sourcePicture;
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
            pieceImage.AddHandler(MouseWheelEvent, new MouseWheelEventHandler(PieceCluster_MouseWheel));

            canvasRoot.Children.Add(pieceImage);
            piece.PieceImage = pieceImage;

            Canvas.SetLeft(pieceImage, _random.Next(0, (int)(mainGrid.ActualWidth - pieceImage.Width)));
            Canvas.SetTop(pieceImage, _random.Next(0, (int)(mainGrid.ActualHeight - pieceImage.Height)));
        }

        private void RemoveClusterById(int clusterId)
        {
            for (int i = 0; i < _clusters.Count; i++)
            {
                if (_clusters[i].Id == clusterId)
                {
                    _clusters.RemoveAt(i);
                    break;
                }
            }
        }

        private Piece GetPieceById(int pieceId)
        {
            foreach (Cluster cluster in _clusters)
            {
                foreach (Piece piece in cluster.Pieces)
                {
                    if (piece.Id == pieceId)
                        return piece;
                }
            }

            return null;
        }

        private Cluster GetClusterById(int clusterId)
        {
            foreach (Cluster cluster in _clusters)
            {
                if (cluster.Id == clusterId)
                {
                    return cluster;
                }
            }

            return null;
        }

        #endregion
    }
}
