using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Puzzle.Class
{
    internal class Engine
    {
        public static List<CroppedBitmap> CutImageToPieces(BitmapImage image)
        {
            List<CroppedBitmap> piecesBitmaps = new List<CroppedBitmap>();

            int pieceWidth = Convert.ToInt32(image.Width / PuzzleSettings.NUM_COLUMNS);
            int pieceHeight = Convert.ToInt32(image.Height / PuzzleSettings.NUM_ROWS);

            for (int i = 0; i < PuzzleSettings.NUM_ROWS; i++)
            {
                for (int j = 0; j < PuzzleSettings.NUM_COLUMNS; j++)
                {
                    piecesBitmaps.Add(new CroppedBitmap(image, new Int32Rect(j * pieceWidth, i * pieceHeight, pieceWidth, pieceHeight)));
                }
            }

            return piecesBitmaps;
        }

        public static List<int> DetermineAdjacentPieceIDs(List<Coordinate> adjacentCoordinates)
        {
            List<int> adjacentPieceIDs = new List<int>();

            foreach (Coordinate coordinate in adjacentCoordinates)
            {
                if (coordinate.X >= 0 && coordinate.X < PuzzleSettings.NUM_COLUMNS &&
                     coordinate.Y >= 0 && coordinate.Y < PuzzleSettings.NUM_ROWS)
                {
                    int pieceID = (coordinate.Y * PuzzleSettings.NUM_COLUMNS) + coordinate.X;
                    adjacentPieceIDs.Add(pieceID);
                }
            }
            adjacentPieceIDs.Sort();

            return adjacentPieceIDs;
        }
    }
}
