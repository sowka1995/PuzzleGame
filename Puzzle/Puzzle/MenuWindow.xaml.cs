using System.Windows;
using System.Windows.Media.Imaging;
using Puzzle.Class;

namespace Puzzle
{
    /// <summary>
    /// Klasa zawierająca logikę dla MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private BitmapImage _sourcePicture;
        private Photo photo;
        private int puzzleSize;
        /// <summary>
        /// Kontruktor
        /// </summary>
        public MenuWindow() 
        {
            InitializeComponent();

            // NEW - Slider określający poziom trudności gry
            slider.Minimum = 0;
            slider.Maximum = PuzzleSettings.SIZE.Length-1;
            slider.IsSnapToTickEnabled = true;
            slider.TickFrequency = 1;
        }

        /// <summary>
        /// Event odpowiadający za wybieranie obrazka 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            puzzleSize = PuzzleSettings.SIZE[(int) slider.Value];

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Photo|*.png;*.jpg;*.jpeg";

            bool? openFileResult = openFileDialog.ShowDialog();

            if (openFileResult == true)
            {
                photo = new Photo(openFileDialog.FileName);
                photo.preapre(puzzleSize);
                _sourcePicture = photo.getBitmapImage();
            }
            Visibility = Visibility.Hidden;

            MainWindow mainWindow = new MainWindow(this);
            mainWindow.Show();
            mainWindow.SetSourcePicture(_sourcePicture);

            // NEW
            mainWindow.Width = PuzzleSettings.WORKSPACE_WIDTH + 50;
            mainWindow.Height = PuzzleSettings.WORKSPACE_HEIGHT + 50;
            mainWindow.SetSourcePhoto(photo);
            mainWindow.SetPuzzleSize(puzzleSize);

            mainWindow.CreateAndDisplayPuzzle();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
