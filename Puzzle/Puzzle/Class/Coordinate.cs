namespace Puzzle.Class
{
    /// <summary>
    /// Klasa opisująca współrzędne X i Y
    /// </summary>
    internal class Coordinate
    {
        /// <summary>
        /// Współrzędna X (pozioma)
        /// </summary>
        public int X { get; set; }
        
        /// <summary>
        /// Współrzędna Y (pionowa)
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="x">Współrzędna X</param>
        /// <param name="y">Współrzędna Y</param>
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
