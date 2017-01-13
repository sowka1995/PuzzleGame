using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle.Class
{
    internal class PuzzleSettings
    {
        public static int NUM_ROWS = 3;
        public static int NUM_COLUMNS = 3;

        public static int WORKSPACE_WIDTH = 1024;
        public static int WORKSPACE_HEIGHT = 768;

        // Dostępne rozmiary puzzli
        public static int[] SIZE = new int[] {100, 75, 50};

        public static readonly int SNAP_TOLERANCE = 10;
        public static readonly int SPACE_BETWEEN_PIECES = 1; // w pikselach
    }
}
