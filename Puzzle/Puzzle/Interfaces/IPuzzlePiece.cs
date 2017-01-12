using Puzzle.Class;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Puzzle.Interfaces
{
    /// <summary>
    /// Interfejs kawałka puzzli
    /// </summary>
    internal interface IPuzzlePiece
    {
        /// <summary>
        /// ID kawałka puzzli
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// ID klastra do którego należy kawałek puzzli
        /// </summary>
        int ClusterId { get; set; }

        /// <summary>
        /// Lokalizacja/pozycja puzzla względem wszystkich puzzli
        /// </summary>
        Coordinate Location { get; set; }

        /// <summary>
        /// Szerokość kawałka puzzli
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Wysokość kawałka puzzli
        /// </summary>
        double Height { get; set; }

        /// <summary>
        /// Obrót kawałka puzzli w stopniach (0, 90, 180, 270)
        /// </summary>
        int Rotation { get; set; }

        /// <summary>
        /// Bedzię reprezentować kawałek puzzli
        /// </summary>
        Image PieceImage { get; set; }

        /// <summary>
        /// Obrazek z którego jest tworzony będzie kawałek puzzli
        /// </summary>
        CroppedBitmap Picture { get; set; }
    }
}
