using System.Windows;
using System.Windows.Media.Imaging;
using Puzzle.Class;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        private BitmapImage _sourcePicture;

        public MenuWindow() 
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Photo|*.png;*.jpg;*.jpeg";

            bool? openFileResult = openFileDialog.ShowDialog();

            if (openFileResult == true)
            {
                Photo insertedPhoto = new Photo(openFileDialog.FileName);
                insertedPhoto.resize();

                button1.IsEnabled = true;
                _sourcePicture = insertedPhoto.getBitmapImage();

                // BitmapImage dla wczytanego, przeskalowanego zdjęcia.
                image.Source = insertedPhoto.getBitmapImage();

                // Tymczsowy label pokazujący wymiary zdjęcia po przeskalowaniu.
                tmpLabel.Content = "(" + insertedPhoto.getBitmapImage().PixelHeight + " / " + PuzzleSettings.NUM_ROWS + ") x (" + insertedPhoto.getBitmapImage().PixelWidth + " / " + PuzzleSettings.NUM_COLUMNS + ")";
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;

            MainWindow mainWindow = new MainWindow(this);
            mainWindow.Show();
            mainWindow.SetSourcePicture(_sourcePicture);
            mainWindow.CreateAndDisplayPuzzle();
        }
    }
}
