using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Puzzle.Class
{
    /// <summary>
    /// Klasa reprezentująca kawałek puzzli
    /// </summary>
    internal class Piece : IPiece
    {
        /// <summary>
        /// ID kawałka puzzli
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// ID klastra do którego należy kawałek puzzli
        /// </summary>
        public int ClusterId { get; set; }

        /// <summary>
        /// Lokalizacja kawałka puzzli w przestrzeni dwuwymiarowej
        /// </summary>
        public Coordinate Location { get; set; }

        /// <summary>
        /// Lista sąsiednich kawałków
        /// </summary>
        public List<int> AdjacentPieceIDs { get; set; }

        /// <summary>
        /// Szerokość kawałka puzzli
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Wysokość kawałka puzzli
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Obrót obrazka w stopniach (0, 90, 180, 270)
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// Reprezentujący kawałek puzzli
        /// </summary>
        public Image PieceImage { get; set; }

        /// <summary>
        /// Obrazek z którego jest tworzony kawałek puzzli
        /// </summary>
        public CroppedBitmap Picture { get; set; }
    }
}
