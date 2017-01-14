using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Class
{
    internal class PuzzleSettings
    {
        public static int WORKSPACE_WIDTH = 1024;
        public static int WORKSPACE_HEIGHT = 768;

        public static int[] SIZE = new int[] { 200, 150, 100 };

        public static readonly int SNAP_TOLERANCE = 10;
        public static readonly int SPACE_BETWEEN_PIECES = 1; // w pikselach
    }
}
