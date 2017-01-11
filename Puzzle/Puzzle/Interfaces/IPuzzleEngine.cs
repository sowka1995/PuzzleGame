using Puzzle.Class;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Puzzle.Interfaces
{
    internal interface IPuzzleEngine
    {
        /// <summary>
        /// Metoda do cięcia obrazka na kawałki/puzzle
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        List<CroppedBitmap> CutImageToPieces(BitmapImage image);
        
        /// <summary>
        /// Metoda do rozstrzygania czy złączyć puzzle
        /// </summary>
        /// <param name="current"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        bool DetermineIfMergePieces(Piece current, Piece adjacent);
       
        /// <summary>
        /// Metoda do określania ID sąsiednich puzzli
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        List<int> DetermineAdjacentPieceIDs(List<Coordinate> coordinates);
        
        /// <summary>
        /// Metoda do wyrównywania pozycji puzzli
        /// </summary>
        /// <param name="cluster"></param>
        void AlignPiecesPositions(Cluster cluster);
        
        /// <summary>
        /// Metoda do usuwania cienia pod puzzlami
        /// </summary>
        /// <param name="cluster"></param>
        /// <param name="zindex"></param>
        void DeleteShadowEffect(Cluster cluster, ref int zindex);
        
        /// <summary>
        /// Metoda do pokazywania cienia pod puzzlami
        /// </summary>
        /// <param name="cluster"></param>
        void DropShadowEffect(Cluster cluster);
        
        /// <summary>
        /// Metoda do obracania puzzli
        /// </summary>
        /// <param name="piece"></param>
        void RotatePieceRandom(Piece piece);
    }
}
