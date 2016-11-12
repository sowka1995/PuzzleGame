using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Puzzle
{
    /// <summary>
    /// Klasa reprezentująca kawałek puzzli
    /// </summary>
    internal class Piece
    {
        /// <summary>
        /// ID kawałka puzzli
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID klastra do którego należy kawałek puzzli
        /// </summary>
        public int ClusterId { get; set; }

        /// <summary>
        /// Lista sąsiednich kawałków
        /// </summary>
        public List<int> AdjacenctPieceIDs { get; set; }

        /// <summary>
        /// Szerokość kawałka puzzli
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Wysokość kawałka puzzli
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Pozycja kawałka puzzli
        /// </summary>
        public Coordinate Position { get; set; }

        /// <summary>
        /// Obrazek reprezentujący wygląd kawałka puzzli
        /// </summary>
        public BitmapImage Picture { get; set; }
    }
}
