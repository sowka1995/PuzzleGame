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
            bitmap = new Bitmap(fullFilePath);
        }

        /// <summary>
        /// Metoda sprawdza czy zdjęcie ma rozmiary mniejsze niż obszar roboczy.
        /// </summary>
        /// <returns></returns>
        private bool hasCorrectSize()
        {
            return (this.bitmap.Width <= PuzzleSettings.WORKSPACE_WIDTH && this.bitmap.Height <= PuzzleSettings.WORKSPACE_HEIGHT);
        }

        /// <summary>
        /// Metoda sprawdza czy zdjęcie ma odpowiednie proporcje by można było utworzyć z niego kwadratowe puzzle.
        /// </summary>
        /// <returns></returns>
        private bool hasCorrectProportions()
        {
            return (this.bitmap.Width / PuzzleSettings.NUM_COLUMNS == this.bitmap.Height / PuzzleSettings.NUM_ROWS);
        }

        /// <summary>
        /// Metoda wylicza proporcjonalnie pomnijeszony rozmiar dla zdjęcia tak by zmieściło się ono w obszarze roboczym.
        /// </summary>
        /// <returns></returns>
        public Size getDecresedSize()
        {
            int newWidth = this.bitmap.Width;
            int newHeight = this.bitmap.Height;

            while(newWidth > PuzzleSettings.WORKSPACE_WIDTH || newHeight > PuzzleSettings.WORKSPACE_HEIGHT)
            {
                if(newWidth > PuzzleSettings.WORKSPACE_WIDTH)
                {
                    int widthDiffrence = this.bitmap.Width - PuzzleSettings.WORKSPACE_WIDTH;
                    double scaleHeightRatio = (double)widthDiffrence / (double)this.bitmap.Width;

                    newWidth = PuzzleSettings.WORKSPACE_WIDTH;
                    newHeight = this.bitmap.Height - (int)(this.bitmap.Height * scaleHeightRatio);
                }

                if(newHeight > PuzzleSettings.WORKSPACE_HEIGHT)
                {
                    int heightDiffence = this.bitmap.Height - PuzzleSettings.WORKSPACE_HEIGHT;
                    double scaleWidthRatio = (double)heightDiffence / (double)this.bitmap.Height;

                    newWidth = this.bitmap.Width - (int)(this.bitmap.Width * scaleWidthRatio);
                    newHeight = PuzzleSettings.WORKSPACE_HEIGHT;
                }
            }

            return new Size(newWidth, newHeight);
        }
        
        /// <summary>
        /// Metoda wylicza przeskalowany rozmiar dla zdjęcia tak by można było utworzyć z niego kwadaratowe puzzle.
        /// </summary>
        /// <returns></returns>
        public Size getScaledSize()
        {
            int newWidth = 0;
            int newHeight = 0;

            int puzzleWidth = this.bitmap.Width / PuzzleSettings.NUM_COLUMNS;
            int puzzleHeight = this.bitmap.Height / PuzzleSettings.NUM_ROWS;

            int puzzleSizeDiffrence = Math.Abs(puzzleHeight - puzzleWidth);

            if(puzzleWidth > puzzleHeight)
            {
                int newPuzzleWidth = puzzleWidth - puzzleSizeDiffrence / 2;
                int newPuzzleHeight = puzzleHeight + puzzleSizeDiffrence / 2;

                newWidth = newPuzzleWidth * PuzzleSettings.NUM_COLUMNS;
                newHeight = newPuzzleHeight * PuzzleSettings.NUM_ROWS;
            }

            if (puzzleWidth < puzzleHeight)
            {
                int newPuzzleWidth = puzzleWidth + puzzleSizeDiffrence / 2;
                int newPuzzleHeight = puzzleHeight - puzzleSizeDiffrence / 2;

                newWidth = newPuzzleWidth * PuzzleSettings.NUM_COLUMNS;
                newHeight = newPuzzleHeight * PuzzleSettings.NUM_ROWS;
            }

            if(puzzleHeight == puzzleWidth)
            {
                newWidth = this.bitmap.Width;
                newHeight = this.bitmap.Height;
            }

            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// Metoda przygotowuje zdjęcie pod generacje puzzli.
        /// 1. Skaluje odpowiednio zdjęcie tak by można było utworzyć z niego kwadratowe puzzle.
        /// 2. Zmniejsza proporcjonalnie rozmiar zdjęcia tak by mieściło się ono w obszarze roboczym.
        /// </summary>
        public void preapre(int puzzleSize)
        {
            if (!this.hasCorrectSize())
                this.bitmap = new Bitmap(this.bitmap, this.getDecresedSize());

            if (!this.hasCorrectProportions())
                this.bitmap = new Bitmap(this.bitmap, this.getScaledSize(puzzleSize));
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

        // NEW
        public int getNumberOfRows(int puzzleSize)
        {
            return (int)Math.Round((double)bitmap.Height / puzzleSize);
        }

        // NEW
        public int getNumberOfColumns(int puzzleSize)
        {
            return (int)Math.Round((double)bitmap.Width / puzzleSize);
        }
        
        // NEW
        public Size getScaledSize(int puzzleSize)
        {
            return new Size(getNumberOfColumns(puzzleSize) * puzzleSize, getNumberOfRows(puzzleSize) * puzzleSize);
        }
    }
}
