using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Puzzle.Class
{
    /// <summary>
    /// Klasa reprezentująca grupę złączonych kawałków puzzli
    /// </summary>
    internal class Cluster
    {
        /// <summary>
        /// Id klastra (złączonych kawałków puzzli)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Lista kawałków należących do danego klastra
        /// </summary>
        public List<Piece> Pieces { get; set; }
    }
}
