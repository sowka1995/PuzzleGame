using Puzzle.Interfaces;
using System.Collections.Generic;

namespace Puzzle.Class
{
    /// <summary>
    /// Klasa reprezentująca grupę złączonych kawałków puzzli
    /// </summary>
    internal class Cluster : IPuzzleCluster
    {
        /// <summary>
        /// Id klastra (złączonych kawałków puzzli)
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Lista kawałków należących do danego klastra
        /// </summary>
        public List<IPuzzlePiece> Pieces { get; set; }
    }
}
