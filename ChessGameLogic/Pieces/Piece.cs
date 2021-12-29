using System.Drawing;

namespace ChessGameLogix.Pieces
{
    public abstract class Piece
    {
        public int Position { get; set; }
        public Player Player { get; set; }
        public abstract string Name { get; set; }

        public Piece(int position, Player player)
        {
            Position = position;
            Player = player;
        }
    }
}