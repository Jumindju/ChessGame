using ChessGameLogic.Pieces;

namespace ChessGameLogic
{
    public class Game
    {
        public Piece[] Board { get; init; }  

        public Game()
        {
            Board = new Piece?[64];
            SetupBoard();
        }

        private void SetupBoard()
        {
            // Kings
            Board[4] = new Piece(PieceType.King, Player.White);
            Board[60] = new Piece(PieceType.King, Player.Black);

            // Queen
            Board[3] = new Piece(PieceType.Queen, Player.White);
            Board[59] = new Piece(PieceType.Queen, Player.Black);

            // Rock
            Board[0] = new Piece(PieceType.Rock, Player.White);
            Board[7] = new Piece(PieceType.Rock, Player.White);
            Board[56] = new Piece(PieceType.Rock, Player.Black);
            Board[63] = new Piece(PieceType.Rock, Player.Black);

            // Bishop
            Board[2] = new Piece(PieceType.Bishop, Player.White);
            Board[5] = new Piece(PieceType.Bishop, Player.White);
            Board[58] = new Piece(PieceType.Bishop, Player.Black);
            Board[61] = new Piece(PieceType.Bishop, Player.Black);

            // Knight
            Board[1] = new Piece(PieceType.Knight, Player.White);
            Board[6] = new Piece(PieceType.Knight, Player.White);
            Board[57] = new Piece(PieceType.Knight, Player.Black);
            Board[62] = new Piece(PieceType.Knight, Player.Black);

            // Pawns
            for (var i = 0; i < 8; i++)
            {
                Board[i + 8] = new Piece(PieceType.Pawn, Player.White);
                Board[i + 48] = new Piece(PieceType.Pawn, Player.Black);
            }
        }
    }
}