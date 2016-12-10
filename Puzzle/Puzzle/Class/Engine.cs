using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Puzzle.Class
{
    internal class Engine
    {
        public static List<CroppedBitmap> CutImageToPieces(BitmapImage image)
        {
            List<CroppedBitmap> piecesBitmaps = new List<CroppedBitmap>();

            int pieceWidth = Convert.ToInt32(image.PixelWidth / PuzzleSettings.NUM_COLUMNS);
            int pieceHeight = Convert.ToInt32(image.PixelHeight/ PuzzleSettings.NUM_ROWS);

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

        public static bool DetermineIfMergePieces(Piece currentPiece, Piece adjacentPiece)
        {
            if (adjacentPiece.ClusterId != currentPiece.ClusterId)
            {
                double topPositionDifference = Canvas.GetTop(currentPiece.PieceImage) - Canvas.GetTop(adjacentPiece.PieceImage);
                double leftPositionDifference = Canvas.GetLeft(currentPiece.PieceImage) - Canvas.GetLeft(adjacentPiece.PieceImage);

                // Sklejanie kawałków które przykładane są z lewej bądź prawej strony
                if (currentPiece.Location.Y == adjacentPiece.Location.Y && Math.Abs(topPositionDifference) <= PuzzleSettings.SNAP_TOLERANCE)
                {
                    if ((currentPiece.Location.X + 1 == adjacentPiece.Location.X && Math.Abs(leftPositionDifference + currentPiece.Width) <= PuzzleSettings.SNAP_TOLERANCE) ||
                         (currentPiece.Location.X - 1 == adjacentPiece.Location.X && Math.Abs(leftPositionDifference - currentPiece.Width) <= PuzzleSettings.SNAP_TOLERANCE))
                    {
                        return true;
                    }
                }
                // Sklejanie kawałków które przykładane są z góry bądź z dołu
                else if (currentPiece.Location.X == adjacentPiece.Location.X && Math.Abs(leftPositionDifference) <= PuzzleSettings.SNAP_TOLERANCE)
                {
                    if ((currentPiece.Location.Y - 1 == adjacentPiece.Location.Y && Math.Abs(topPositionDifference - currentPiece.Height) <= PuzzleSettings.SNAP_TOLERANCE) ||
                         (currentPiece.Location.Y + 1 == adjacentPiece.Location.Y && Math.Abs(topPositionDifference + currentPiece.Height) <= PuzzleSettings.SNAP_TOLERANCE))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
