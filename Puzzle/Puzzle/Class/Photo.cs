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
        /// Bitmapa przechowująca zawartość zdjęcia.
        /// </summary>
        private Bitmap bitmap { get; set; }

        public Photo(string filePath)
        {
            bitmap = new Bitmap(filePath);
        }

        public Photo(Bitmap bitmap)
        {
            this.bitmap = new Bitmap(bitmap);
        }

        public Photo(BitmapImage bitmapImage)
        {
            this.bitmap = new Bitmap(bitmapImage.StreamSource);
        }

        /// <summary>
        /// Metoda sprawdza czy zdjęcie ma rozmiary mniejsze niż obszar roboczy.
        /// </summary>
        /// <returns></returns>
        private bool hasCorrectSize()
        {
            return (bitmap.Width <= PuzzleSettings.WORKSPACE_WIDTH && bitmap.Height <= PuzzleSettings.WORKSPACE_HEIGHT);
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
        
        public int getNumberOfColumns(int puzzleSize)
        {
            return (int)Math.Round((double)bitmap.Width / puzzleSize);
        }

        public int getNumberOfRows(int puzzleSize)
        {
            return (int)Math.Round((double)bitmap.Height / puzzleSize);
        }

        public Size getScaledSize(int puzzleSize)
        {
            return new Size(getNumberOfColumns(puzzleSize) * puzzleSize, getNumberOfRows(puzzleSize) * puzzleSize);
        }

        /// <summary>
        /// Metoda przygotowuje zdjęcie pod generacje puzzli.
        /// 1. Skaluje odpowiednio zdjęcie tak by można było utworzyć z niego kwadratowe puzzle.
        /// 2. Zmniejsza proporcjonalnie rozmiar zdjęcia tak by mieściło się ono w obszarze roboczym.
        /// </summary>
        public void preapre(int puzzleSize)
        {
            if (!hasCorrectSize())
            {
                bitmap = new Bitmap(bitmap, getDecresedSize());
            }
            
            bitmap = new Bitmap(bitmap, getScaledSize(puzzleSize));
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
