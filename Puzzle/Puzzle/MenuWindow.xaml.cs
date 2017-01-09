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
                Photo insertedPhoto = new Photo(openFileDialog.FileName);
                insertedPhoto.preapre();
                _sourcePicture = insertedPhoto.getBitmapImage();
            }
            Visibility = Visibility.Hidden;

            MainWindow mainWindow = new MainWindow(this);
            mainWindow.Show();
            mainWindow.SetSourcePicture(_sourcePicture);
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
