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

        /// <summary>
        /// Kontruktor
        /// </summary>
        public MenuWindow() 
        {
            InitializeComponent();
            InitializeDifficultyLevelSlider();
        }

        /// <summary>
        /// Event odpowiadający za wybieranie obrazka 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Photo|*.png;*.jpg;*.jpeg";

            bool? openFileResult = openFileDialog.ShowDialog();

            if (openFileResult == true)
            {
                int puzzleSize = PuzzleSettings.SIZE[(int)difficultyLevelSlider.Value];

                Photo photo = new Photo(openFileDialog.FileName);
                photo.preapre(puzzleSize);
                _sourcePicture = photo.getBitmapImage();

                Visibility = Visibility.Hidden;

                MainWindow mainWindow = new MainWindow(this);
                mainWindow.Show();

                mainWindow.SetSourcePicture(_sourcePicture);
                mainWindow.SetSourcePhoto(photo);
                mainWindow.SetPuzzleSize(puzzleSize);

                mainWindow.MinWidth = mainWindow.ActualWidth - 20;
                mainWindow.MinHeight = mainWindow.ActualHeight - 20;

                mainWindow.CreateAndDisplayPuzzle();
            }
           
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

        private void InitializeDifficultyLevelSlider()
        {
            difficultyLevelSlider.Minimum = 0;
            difficultyLevelSlider.Maximum = PuzzleSettings.SIZE.Length - 1;

            difficultyLevelSlider.IsSnapToTickEnabled = true;
            difficultyLevelSlider.TickFrequency = 1;
        }

        private void InstructionButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
