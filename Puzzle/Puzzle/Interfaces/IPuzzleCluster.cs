using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Interfaces
{
    interface IPuzzleCluster 
    {
        /// <summary>
        /// ID klastra
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Lista kawałków należących do danego klastra
        /// </summary>
        List<IPuzzlePiece> Pieces { get; set; }
    }
}
