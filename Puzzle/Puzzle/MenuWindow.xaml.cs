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
                insertedPhoto.preapre();
                _sourcePicture = insertedPhoto.getBitmapImage();


            }

            MainWindow mainWindow = new MainWindow(this);
            mainWindow.Show();
            mainWindow.SetSourcePicture(_sourcePicture);
            mainWindow.CreateAndDisplayPuzzle();
        }


        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
