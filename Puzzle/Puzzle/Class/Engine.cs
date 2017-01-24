using Puzzle.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace Puzzle.Class
{
    /// <summary>
    /// Klasa będąca "silnikiem" aplikacji
    /// </summary>
    internal class Engine : IPuzzleEngine
    {   
        private static readonly Random _random = new Random();

        /// <summary>
        /// Metoda odpowiadająca za dzielenie obrazak na kawałki
        /// </summary>
        /// <param name="image">Obrazek do pocięcia</param>
        /// <returns>Lista pociętych kawałków</returns>
        public List<CroppedBitmap> CutImageToPieces(BitmapImage image, int puzzleSize)
        {
            List<CroppedBitmap> piecesBitmaps = new List<CroppedBitmap>();
            Photo photo = new Photo(image);        
                
            for (int i = 0; i < photo.getNumberOfRows(puzzleSize); i++)
            {
                for (int j = 0; j < photo.getNumberOfColumns(puzzleSize); j++)
                {
                    piecesBitmaps.Add(new CroppedBitmap(photo.getBitmapImage(), new Int32Rect(j * puzzleSize, i * puzzleSize, puzzleSize, puzzleSize)));
                }
            }

            return piecesBitmaps;
        }

        /// <summary>
        /// Metoda ustalająca ID sąsiednich kawałków puzzli danego puzzla
        /// </summary>
        /// <param name="adjacentCoordinates">Współrzędne puzzla</param>
        /// <returns>ID sąsiednich kawałków puzzli</returns>
        public List<int> DetermineAdjacentPieceIDs(List<Coordinate> adjacentCoordinates, int numberOfColumns, int numberOfRows)
        {
            List<int> adjacentPieceIDs = new List<int>();

            foreach (Coordinate coordinate in adjacentCoordinates)
            {
                if (coordinate.X >= 0 && coordinate.X < numberOfColumns &&
                     coordinate.Y >= 0 && coordinate.Y < numberOfRows)
                {
                    int pieceID = (coordinate.Y * numberOfColumns) + coordinate.X;
                    adjacentPieceIDs.Add(pieceID);
                }
            }
            adjacentPieceIDs.Sort();

            return adjacentPieceIDs;
        }

        /// <summary>
        /// Metoda rozstrzygająca czy puzzle pasują do siebie oraz czy są na tyle blisko by je scalić/złączyć
        /// </summary>
        /// <param name="currentPiece">Aktualny puzzel</param>
        /// <param name="adjacentPiece">Sąsiedni puzzel</param>
        /// <returns>True - jeśli puzzle pasują do siebie, False - jeśli nie pasują, bądź są za daleko od siebie</returns>
        public bool DetermineIfMergePieces(IPuzzlePiece currentPiece, IPuzzlePiece adjacentPiece)   
        {
            // jeżeli obrót jest różny od 0 to nie można scalić dwóch kawałków
            if (currentPiece.Rotation != 0 || adjacentPiece.Rotation != 0)
                return false;

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

        /// <summary>
        /// Metoda odpowiadająca za wyrównywanie scalonych puzzli
        /// </summary>
        /// <param name="cluster">Referencja do klastra w których puzzle mają być wyrównane</param>
        public void AlignPiecesPositions(IPuzzleCluster cluster)
        {
            var mainPiece = cluster.Pieces[0] as Piece;

            foreach (Piece piece in cluster.Pieces)
            {
                int differenceX = mainPiece.Location.X - piece.Location.X;
                int differenceY = mainPiece.Location.Y - piece.Location.Y;

                double canvasLeft = Canvas.GetLeft(mainPiece.PieceImage) - mainPiece.Width * differenceX;
                double canvasTop = Canvas.GetTop(mainPiece.PieceImage) - mainPiece.Height * differenceY;

                canvasTop -= PuzzleSettings.SPACE_BETWEEN_PIECES * differenceY;
                canvasLeft -= PuzzleSettings.SPACE_BETWEEN_PIECES * differenceX;

                Canvas.SetTop(piece.PieceImage, canvasTop);
                Canvas.SetLeft(piece.PieceImage, canvasLeft);
            }
        }

        /// <summary>
        /// Metoda usuwająca efekt cienia pod puzzlami
        /// </summary>
        /// <param name="cluster">Klaster z którego cień usunąć</param>
        /// <param name="zindex"></param>
        public void DeleteShadowEffect(IPuzzleCluster cluster, ref int zindex)
        {
            if (cluster == null)
                return;

            foreach (Piece piece in cluster.Pieces)
            {
                piece.PieceImage.ClearValue(UIElement.EffectProperty);
                Panel.SetZIndex(piece.PieceImage, zindex);
            }
            zindex++;
        }

        /// <summary>
        /// Metoda pokazująca cień pod puzzlami
        /// </summary>
        /// <param name="cluster">Klaster puzzli dla których pokazać cień</param>
        public void DropShadowEffect(IPuzzleCluster cluster)
        {
            if (cluster == null)
                return;

            foreach (Piece piece in cluster.Pieces)
            {
                piece.PieceImage.Effect = new DropShadowEffect()
                {
                    Color = new System.Windows.Media.Color() { A = 2, R = 0, G = 0, B = 0 },
                    Direction = piece.Rotation - 90,
                    ShadowDepth = 1.5d,
                    Opacity = 2,
                };
                Panel.SetZIndex(piece.PieceImage, int.MaxValue);
            }
        }

        /// <summary>
        /// Metoda obracająca pojedynczy puzzel o losowy kąt (0, 90, 180, 270)
        /// </summary>
        /// <param name="pieceToRotate">Kawałek puzzli do obracania</param>
        public void RotatePieceRandom(IPuzzlePiece pieceToRotate)
        {
            Matrix matrix = pieceToRotate.PieceImage.RenderTransform.Value;

            int[] degrees = { 0, 90, 180, 270 };
            int rotation = degrees[_random.Next(0, degrees.Length)];

            double centerX = pieceToRotate.Width / 2;
            double centerY = pieceToRotate.Height / 2;

            pieceToRotate.Rotation = rotation;
            matrix.RotateAtPrepend(rotation, centerX, centerY);

            MatrixTransform matrixTransform = new MatrixTransform(matrix);
            pieceToRotate.PieceImage.RenderTransform = matrixTransform;
        }
    }
}
