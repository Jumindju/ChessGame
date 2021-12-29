using System.Drawing;

namespace ChessGameLogix.Pieces
{
    public class Pawn : Piece
    {
        public override string Name { get; set; }

        public Pawn(int position, Player player) : base(position, player)
        {
            Name = $"{player:G} Pawn";
        }
    }
}