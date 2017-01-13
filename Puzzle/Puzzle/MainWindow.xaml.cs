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
using Puzzle.Interfaces;

namespace Puzzle
{
    /// <summary>
    /// Zawiera logikę dla MainWindow.xaml
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
        private IPuzzleCluster _currentCluster;

        /// <summary>
        /// Przechowuje wszystkie klastry puzzli
        /// </summary>
        private List<IPuzzleCluster> _clusters;

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
        /// Przechowuje referencję do puzzla który jest obserwowany przez klawiaturę
        /// </summary>
        private Image _currentFocusImage;

        /// <summary>
        /// Służy do wyciągania na pierwszy plan przenoszonych kawałków puzzli
        /// </summary>
        private int _zindex = 1;

        /// <summary>
        /// Silnik puzzli
        /// </summary>
        private IPuzzleEngine _engine;
        
        #endregion 

        /// <summary>
        /// Kontruktor
        /// </summary>
        /// <param name="menuWindow">Referencja do okna Menu aplikacji</param>
        public MainWindow(Window menuWindow)
        {
            InitializeComponent();

            _menuWindow = menuWindow;
            _previousMousePosition = Mouse.GetPosition(mainGrid);
            _clusters = new List<IPuzzleCluster>();
            _canMovePiece = false;
            _random = new Random();
            _engine = new Engine();
        }

        #region Events

        /// <summary>
        /// Event odpowiadający za przesuwanie puzzli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Event odpowiadający za wykrycie i przygotowanie "złapanego" puzzla do przesuwania 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PieceCluster_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            Image pieceImage = sender as Image;

            int pieceId = int.Parse(pieceImage.Name.Substring(1)); // id z nazwy 
            _currentCluster = GetClusterContainsPiece(pieceId);

            _previousMousePosition = e.GetPosition(this);
            _canMovePiece = true;

            _engine.DropShadowEffect(_currentCluster);
        }

        /// <summary>
        /// Event odpowiadający za przygotowanie puzzla do obracania
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PieceCluster_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            image.Focusable = true;
            _currentFocusImage = image;

            Panel.SetZIndex(image, _zindex++);
            Keyboard.Focus(_currentFocusImage);
        }

        /// <summary>
        /// Event kończący proces obracania puzzla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PieceCluster_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentFocusImage != null)
            {
                _currentFocusImage.Focusable = false;
                Keyboard.Focus(canvasRoot);
            }
        }

        /// <summary>
        /// Event odpowiadający za scalanie puzzli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PieceCluster_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CheckAndMergePieces();
        }

        /// <summary>
        /// Event odpowiadający za obracanie puzzla poprzez scrolla
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PieceCluster_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Image pieceImage = sender as Image;       

            if (pieceImage != null)
            {
                Panel.SetZIndex(pieceImage, _zindex++);
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

                if (piece.Rotation == 0)
                {
                    _canMovePiece = true;
                    CheckAndMergePieces();
                    _canMovePiece = false;
                }
            }
        }

        /// <summary>
        /// Event odpowiadający za obracanie puzzla za pomocą strzałek (lewo/prawo)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PieceCluster_KeyDown(object sender, KeyEventArgs e)
        {
            Image pieceImage = sender as Image;

            if (pieceImage != null && Mouse.RightButton == MouseButtonState.Pressed)
            {
                Panel.SetZIndex(pieceImage, _zindex++);
                Matrix matrix = pieceImage.RenderTransform.Value;

                int pieceId = int.Parse(pieceImage.Name.Substring(1)); // id z nazwy 
                Piece piece = GetPieceById(pieceId);

                if (e.Key == Key.Left || e.Key == Key.Right)
                {
                    double centerX = piece.PieceImage.ActualWidth / 2;
                    double centerY = piece.PieceImage.ActualHeight / 2;

                    int sign = e.Key == Key.Left ? -1 : 1;

                    matrix.RotateAtPrepend(90 * sign, centerX, centerY);
                    if (piece.Rotation == 270 * sign)
                        piece.Rotation = 0;
                    else
                        piece.Rotation += 90 * sign;
                }

                MatrixTransform matrixTransform = new MatrixTransform(matrix);
                piece.PieceImage.RenderTransform = matrixTransform;

                if (piece.Rotation == 0 && _currentCluster != null)
                {
                    _canMovePiece = true;
                    CheckAndMergePieces();
                    _canMovePiece = false;
                }
            }
        }

        /// <summary>
        /// Event odpowiadający za zamknięcie okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {      
            _menuWindow.Visibility = Visibility.Visible;
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Główna metoda tworząca i wyświetlająca puzzle
        /// </summary>
        public void CreateAndDisplayPuzzle()
        {
            int pieceId = 0;
            int pieceCount = 0;

            var pieces = _engine.CutImageToPieces(_sourcePicture);

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

                    List<int> adjacentPieceIDs = _engine.DetermineAdjacentPieceIDs(adjacentCoordinates);

                    var piece = new Piece()
                    {
                        ID = pieceId,
                        ClusterId = pieceId,
                        AdjacentPieceIDs = adjacentPieceIDs,
                        Location = new Coordinate(col, row),
                        Height = pieces[pieceCount].Height,
                        Width = pieces[pieceCount].Width,
                        Picture = pieces[pieceCount],
                    };

                    InitPiece(piece);
                    _engine.RotatePieceRandom(piece);

                    var cluster = new Cluster()
                    {
                        ID = pieceId,
                        Pieces = new List<IPuzzlePiece>() { piece }
                    };

                    _clusters.Add(cluster);

                    pieceId++;
                    pieceCount++;
                }
            }

            ShowHintImage();
        }

        /// <summary>
        /// Metoda ustawiająca obrazek z którego generowane są puzzle
        /// </summary>
        /// <param name="sourcePicture">Obrazek źródłowy</param>
        public void SetSourcePicture(BitmapImage sourcePicture)
        {
            _sourcePicture = sourcePicture;
        }

        private void ShowHintImage()
        {
            Image hintImage = new Image()
            {
                Source = _sourcePicture,
                Width = _sourcePicture.Width,
                Height = _sourcePicture.Height,
                Focusable = false,
                IsEnabled = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                OpacityMask = new ImageBrush() { ImageSource = _sourcePicture, Opacity = 0.6d }
            };
            Panel.SetZIndex(hintImage, -1);
            mainGrid.Children.Add(hintImage);        
        }

        /// <summary>
        /// Metoda dokonująca inicjalizacji pojedynczego kawałka puzzli
        /// </summary>
        /// <param name="piece">Kawałek puzzli</param>
        private void InitPiece(IPuzzlePiece piece)
        {
            Image pieceImage = new Image
            {
                Source = piece.Picture,
                Width = piece.Width,
                Height = piece.Height,
                Name = "p" + piece.ID
            };
            pieceImage.FocusVisualStyle = null;

            pieceImage.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(PieceCluster_MouseLeftButtonDown));
            pieceImage.AddHandler(MouseRightButtonDownEvent, new MouseButtonEventHandler(PieceCluster_MouseRightButtonDown));
            pieceImage.AddHandler(MouseRightButtonUpEvent, new MouseButtonEventHandler(PieceCluster_MouseRightButtonUp));
            pieceImage.AddHandler(MouseMoveEvent, new MouseEventHandler(PieceCluster_MouseMove));
            pieceImage.AddHandler(MouseUpEvent, new MouseButtonEventHandler(PieceCluster_MouseUp));
            pieceImage.AddHandler(MouseWheelEvent, new MouseWheelEventHandler(PieceCluster_MouseWheel));
            pieceImage.AddHandler(KeyDownEvent, new KeyEventHandler(PieceCluster_KeyDown));

            canvasRoot.Children.Add(pieceImage);
            piece.PieceImage = pieceImage;

            Canvas.SetLeft(pieceImage, _random.Next(0, (int)(mainGrid.ActualWidth - pieceImage.Width)));
            Canvas.SetTop(pieceImage, _random.Next(0, (int)(mainGrid.ActualHeight - pieceImage.Height)));
        }

        /// <summary>
        /// Metoda usuwająca klaster/grupę puzzli
        /// </summary>
        /// <param name="clusterId">Id klastra do usunięcia</param>
        private void RemoveClusterById(int clusterId)
        {
            for (int i = 0; i < _clusters.Count; i++)
            {
                if (_clusters[i].ID == clusterId)
                {
                    _clusters.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Metoda zwracająca kawałek puzzli
        /// </summary>
        /// <param name="pieceId">Id kawałka puzzli</param>
        /// <returns></returns>
        private Piece GetPieceById(int pieceId)
        {
            foreach (Cluster cluster in _clusters)
            {
                foreach (Piece piece in cluster.Pieces)
                {
                    if (piece.ID == pieceId)
                        return piece;
                }
            }

            return null;
        }

        private Cluster GetClusterContainsPiece(int pieceId)
        {
            foreach (Cluster cluster in _clusters)
            {
                foreach (Piece piece in cluster.Pieces)
                {
                    if (piece.ID == pieceId)
                    {
                        return cluster;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Metoda zwracająca klaster/grupę puzzli
        /// </summary>
        /// <param name="clusterId">Id klastra/grupy puzzli</param>
        /// <returns></returns>
        private Cluster GetClusterById(int clusterId)
        {
            foreach (Cluster cluster in _clusters)
            {
                if (cluster.ID == clusterId)
                {
                    return cluster;
                }
            }

            return null;
        }

        /// <summary>
        /// Metoda sprawdza czy są jakieś puzzle do złączenia, jeśli tak to je łączy
        /// </summary>
        private void CheckAndMergePieces()
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            _engine.DeleteShadowEffect(_currentCluster, ref _zindex);

            if (_canMovePiece && !_isSolved && _currentCluster != null)
            {
                List<int> adjacentClusterIDs = new List<int>();

                // łączenie puzzli
                for (int i = 0; i < _currentCluster.Pieces.Count; i++)
                {
                    Piece currentPiece = _currentCluster.Pieces[i] as Piece;

                    foreach (int pieceId in currentPiece.AdjacentPieceIDs)
                    {
                        Piece adjacentPiece = GetPieceById(pieceId);

                        if (adjacentPiece != null && adjacentPiece.ClusterId != currentPiece.ClusterId)
                        {
                            if (_engine.DetermineIfMergePieces(currentPiece, adjacentPiece))
                            {
                                Cluster adjacentCluster = GetClusterById(adjacentPiece.ClusterId);

                                adjacentClusterIDs.Add(adjacentCluster.ID);

                                // Aktualizacja ClusterId dla kawałków w sąsiędnim klastrze
                                foreach (Piece piece in adjacentCluster.Pieces)
                                {
                                    piece.ClusterId = currentPiece.ClusterId;
                                }

                                // usuwanie zbędnych Eventów (jeżeli złączono to muszą być już obrócone w dobrą stronę)
                                currentPiece.PieceImage.RemoveHandler(MouseWheelEvent, new MouseWheelEventHandler(PieceCluster_MouseWheel));
                                currentPiece.PieceImage.RemoveHandler(KeyDownEvent, new KeyEventHandler(PieceCluster_KeyDown));
                                currentPiece.PieceImage.RemoveHandler(MouseRightButtonDownEvent, new MouseButtonEventHandler(PieceCluster_MouseRightButtonDown));
                                currentPiece.PieceImage.RemoveHandler(MouseRightButtonUpEvent, new MouseButtonEventHandler(PieceCluster_MouseRightButtonUp));
                                adjacentPiece.PieceImage.RemoveHandler(MouseWheelEvent, new MouseWheelEventHandler(PieceCluster_MouseWheel));
                                adjacentPiece.PieceImage.RemoveHandler(KeyDownEvent, new KeyEventHandler(PieceCluster_KeyDown));
                                adjacentPiece.PieceImage.RemoveHandler(MouseRightButtonDownEvent, new MouseButtonEventHandler(PieceCluster_MouseRightButtonDown));
                                adjacentPiece.PieceImage.RemoveHandler(MouseRightButtonUpEvent, new MouseButtonEventHandler(PieceCluster_MouseRightButtonUp));
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
                            Panel.SetZIndex(piece.PieceImage, _zindex++);
                        }
                        _engine.AlignPiecesPositions(_currentCluster);

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

        #endregion
    }
}
