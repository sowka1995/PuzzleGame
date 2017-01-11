using Puzzle.Class;

namespace Puzzle.Interfaces
{
    /// <summary>
    /// Interfejs kawałka puzzli
    /// </summary>
    internal interface IPuzzlePiece
    {
        /// <summary>
        /// Opisuje unikalne id kawałka puzzli
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Opisuje lokalizację/pozycję puzzla względem wszystkich puzzli
        /// </summary>
        Coordinate Location { get; set; }
    }
}
