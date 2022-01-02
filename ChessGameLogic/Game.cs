using System;
using System.Collections.Generic;
using ChessGameLogic.Pieces;

namespace ChessGameLogic
{
    public class Game
    {
        public const int BoardSize = 8;
        public const int TileCount = BoardSize * BoardSize;

        public Piece?[] Board { get; }
        public Player CurrentPlayer { get; private set; }

        public Game()
        {
            Board = new Piece?[64];
            CurrentPlayer = Player.White;

            SetupBoard();
        }

        public void DoMove(int startPosition, int targetPosition)
        {
            var targetPiece = Board[startPosition];
            if (targetPiece is null)
                throw new ArgumentException("Piece to move does not exist", nameof(startPosition));

            var possibleMoves = GetMoves(startPosition);
            if (!possibleMoves.Contains(targetPosition))
                throw new InvalidOperationException("Move is not possible");

            Board[startPosition] = null;
            Board[targetPosition] = targetPiece;

            CurrentPlayer = CurrentPlayer == Player.White
                ? Player.Black
                : Player.White;
        }

        public List<int> GetMoves(int piecePosition)
        {
            var piece = Board[piecePosition];
            if (piece is null)
                throw new ArgumentOutOfRangeException(nameof(piecePosition), "No piece on given position");

            if (piece.Player != CurrentPlayer)
                throw new InvalidOperationException("Not users turn");

            return piece.PieceType switch
            {
                PieceType.Pawn => GetPawnMoves(piecePosition, piece),
                PieceType.King => GetKingMoves(piecePosition, piece),
                PieceType.Queen => GetQueenMoves(piecePosition, piece),
                PieceType.Rock => GetRockMoves(piecePosition, piece),
                PieceType.Bishop => GetBishopMoves(piecePosition, piece),
                PieceType.Knight => GetKnightMoves(piecePosition, piece),
                _ => throw new InvalidOperationException("Invalid piece type")
            };
        }

        private List<int> GetKingMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<int> GetQueenMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<int> GetRockMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<int> GetBishopMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<int> GetKnightMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<int> GetPawnMoves(int piecePosition, Piece piece)
        {
            var moves = new List<int>();
            var direction = (int) piece.Player;

            // check if piece is in front
            var moveInFront = piecePosition + (BoardSize * direction);
            if (Board[moveInFront] is null)
                moves.Add(moveInFront);

            // 2 moves on start position
            var isOnStartPosition = (piece.Player == Player.White && piecePosition / 8 == 1) ||
                                    (piece.Player == Player.Black && piecePosition / 8 == 6);
            if (isOnStartPosition)
            {
                var doubleMove = piecePosition + (BoardSize * 2 * direction);
                if (Board[doubleMove] is null)
                    moves.Add(doubleMove);
            }

            return moves;
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