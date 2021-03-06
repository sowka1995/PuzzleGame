﻿using System.Windows;
using System.Windows.Media.Imaging;
using Puzzle.Class;

namespace Puzzle
{
    /// <summary>
    /// Klasa zawierająca logikę dla MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
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

                Visibility = Visibility.Hidden;

                MainWindow mainWindow = new MainWindow(this);
                mainWindow.Show();

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
            InstructionGrid.Visibility = Visibility.Visible;
        }

        private void MainMenu_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                InstructionGrid.Visibility = Visibility.Hidden;
        }

        private void BackLabel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InstructionGrid.Visibility = Visibility.Hidden;
        }


    }
}
