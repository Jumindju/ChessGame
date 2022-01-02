using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ChessGameLogic;
using ChessGameLogic.Pieces;

namespace ChessGameUI.Helper
{
    public static class PieceHelper
    {
        private const int SpriteWidth = 333;
        private const int SpriteHeight = 333;

        public static Image GetPieceImage(Image pieceSprite, Piece piece)
        {
            var cropRect = GetSpritePosition(piece);

            var pieceImg = new Bitmap(SpriteWidth, SpriteHeight);
            using var g = Graphics.FromImage(pieceImg);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.DrawImage(pieceSprite, 0, 0, cropRect, GraphicsUnit.Pixel);

            pieceImg.Save("./test.png");

            return pieceImg;
        }

        private static Rectangle GetSpritePosition(Piece piece)
        {
            var (pieceType, player) = piece;
            var y = player == Player.White
                ? 0
                : SpriteHeight;

            var x = pieceType switch
            {
                PieceType.King => 0, 
                PieceType.Queen => SpriteWidth,
                PieceType.Bishop => 2 * SpriteWidth,
                PieceType.Knight => 3 * SpriteWidth,
                PieceType.Rock => 4 * SpriteWidth,
                PieceType.Pawn => 5 * SpriteWidth,
                _ => throw new ArgumentException("Unknown piece")
            };

            return new Rectangle(x, y, SpriteWidth, SpriteHeight);
        }
    }
}