using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using ChessGameLogix;
using ChessGameLogix.Pieces;

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
            if (piece is null)
                throw new ArgumentNullException(nameof(piece));
            if (piece.Player == Player.None)
                throw new ArgumentException("This piece belongs to no player");

            var y = piece.Player == Player.White
                ? 0
                : SpriteHeight;

            var x = piece switch
            {
                Pawn => 5 * SpriteWidth,
                _ => throw new ArgumentException("Unknown piece")
            };

            return new Rectangle(x, y, SpriteWidth, SpriteHeight);
        }
    }
}