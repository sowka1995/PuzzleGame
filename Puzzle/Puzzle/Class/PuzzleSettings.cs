namespace Puzzle.Class
{
    internal class PuzzleSettings
    {
        public static int WORKSPACE_WIDTH = (int)(System.Windows.SystemParameters.PrimaryScreenWidth * 0.85);
        public static int WORKSPACE_HEIGHT = (int)(System.Windows.SystemParameters.PrimaryScreenHeight * 0.85);

        public static int[] SIZE = new int[] { 190, 170, 150, 125, 100 };

        public static readonly int SNAP_TOLERANCE = 10;
        public static readonly int SPACE_BETWEEN_PIECES = 1; // w pikselach
    }
}
