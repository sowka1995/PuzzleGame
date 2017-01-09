namespace Puzzle.Class
{
    /// <summary>
    /// Interfejs kawałka puzzli
    /// </summary>
    internal interface IPiece
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
