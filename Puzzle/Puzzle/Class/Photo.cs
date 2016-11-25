using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Puzzle.Class
{
    /// <summary>
    /// Klasa reprezentująca obiekt zdjęcia wczytywanego do aplikacji.
    /// </summary>
    public class Photo
    {
        /// <summary>
        /// Pełna ścieżka do pliku zdjęcia na dysku.
        /// </summary>
        private string fullFilePath;

        /// <summary>
        /// Bitmapa przechowująca zawartość zdjęcia.
        /// </summary>
        private Bitmap bitmap { get; set; }

        public Photo(string fullFilePath)
        {
            this.fullFilePath = fullFilePath;
            this.bitmap = new Bitmap(fullFilePath);
        }

        /// <summary>
        /// Metoda sprawdza czy zdjęcie ma odpowiednie rozmiary do generacji puzzli.
        /// </summary>
        /// <returns>
        /// Boolean:
        /// true - jeżeli obraz ma odpowiednie rozmiary,
        /// false - jeżli obraz nie ma odpowiednich rozmiarów.
        /// </returns>
        public bool isValid()
        {
            int photoHeight = this.bitmap.Height;
            int photoWidth = this.bitmap.Width;
            
            return photoHeight / PuzzleSettings.NUM_ROWS == photoWidth / PuzzleSettings.NUM_COLUMNS;
        }

        /// <summary>
        /// Metoda skaluje zdjęcie (odpowiednio je rozciąga lub zwęża) tak by można było z niego wygenerować puzzle których parametry są zdefiniowane w ustawieniach.
        /// </summary>
        public void resize()
        {
            int photoHeight = this.bitmap.Height;
            int photoWidth = this.bitmap.Width;

            double currentPuzzleHeight = photoHeight / PuzzleSettings.NUM_ROWS;
            double currentPuzzleWidth = photoWidth / PuzzleSettings.NUM_COLUMNS;

            double currentPuzzleDimensionDiffrence = Math.Abs(currentPuzzleHeight - currentPuzzleWidth);
 
            double scaleDownRatio = (currentPuzzleDimensionDiffrence / 2) / Math.Max(currentPuzzleHeight, currentPuzzleWidth);
            double scaleUpRatio = (currentPuzzleDimensionDiffrence / 2) / Math.Min(currentPuzzleHeight, currentPuzzleWidth);

            if(PuzzleSettings.NUM_ROWS >= PuzzleSettings.NUM_COLUMNS)
            {
                int newPhotoHeight = (int) Math.Round(photoHeight + photoHeight * scaleUpRatio);
                int newPhotoWidth = (int)Math.Round(photoWidth - photoWidth * scaleDownRatio);
                this.bitmap = new Bitmap(this.bitmap, new Size(newPhotoWidth, newPhotoHeight));
            }

            if (PuzzleSettings.NUM_ROWS < PuzzleSettings.NUM_COLUMNS)
            {
                int newPhotoHeight = (int)Math.Round(photoHeight - photoHeight * scaleDownRatio);
                int newPhotoWidth = (int)Math.Round(photoWidth + photoWidth * scaleUpRatio);
                this.bitmap = new Bitmap(this.bitmap, new Size(newPhotoWidth, newPhotoHeight));
            }

            if (PuzzleSettings.NUM_ROWS == PuzzleSettings.NUM_COLUMNS)
            {
                if (photoWidth > photoHeight)
                {
                    int newPhotoHeight = (int)Math.Round(photoHeight + photoHeight * scaleUpRatio);
                    int newPhotoWidth = (int)Math.Round(photoWidth - photoWidth * scaleDownRatio);
                    this.bitmap = new Bitmap(this.bitmap, new Size(newPhotoWidth, newPhotoHeight));
                }

                if (photoWidth < photoHeight)
                {
                    int newPhotoHeight = (int)Math.Round(photoHeight - photoHeight * scaleDownRatio);
                    int newPhotoWidth = (int)Math.Round(photoWidth + photoWidth * scaleUpRatio);
                    this.bitmap = new Bitmap(this.bitmap, new Size(newPhotoWidth, newPhotoHeight));
                }
            }
        }

        public BitmapImage getBitmapImage()
        {
            MemoryStream ms = new MemoryStream();
            this.bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
