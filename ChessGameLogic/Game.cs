﻿using System;
using System.Collections.Generic;
using System.Linq;
using ChessGameLogic.Pieces;

namespace ChessGameLogic
{
    public class Game
    {
        public const int BoardSize = 8;
        public const int TileCount = BoardSize * BoardSize;

        private Move LastDoublePawnMoveRow = null;

        public Piece?[] Board { get; }
        public Player CurrentPlayer { get; private set; }

        public Game()
        {
            Board = new Piece?[64];
            CurrentPlayer = Player.White;

            SetupBoard();
        }

        public MoveType MovePiece(int startPosition, MoveOption targetMove)
        {
            var targetPiece = Board[startPosition];
            if (targetPiece is null)
                throw new ArgumentException("Piece to move does not exist", nameof(startPosition));

            var (targetPosition, moveType) = targetMove;

            var possibleMoves = GetMoves(startPosition);
            if (possibleMoves.All(move => move.MovePosition != targetPosition))
                throw new InvalidOperationException("Move is not possible");

            Board[startPosition] = null;
            Board[targetPosition] = targetPiece;

            var targetRow = targetPosition / BoardSize;
            var targetColumn = targetPosition % 8;

            // check for promotion
            var isOnLastRow = (targetPiece.Player == Player.White && targetRow == BoardSize - 1) ||
                              (targetPiece.Player == Player.Black && targetRow == 0);
            if (targetPiece.PieceType == PieceType.Pawn && isOnLastRow)
                return MoveType.Promotion;

            // save double move of pawn for en passent
            if (targetPiece.PieceType == PieceType.Pawn && Math.Abs(targetPosition - startPosition) == 16)
                LastDoublePawnMoveRow = new Move(targetRow, targetColumn);
            else
                LastDoublePawnMoveRow = null;

            // handle en passent capture
            // until here the pawn moved to the new position but the captured pawn is still on the board
            if (moveType == MoveType.EnPassent)
            {
                var moveDirection = CurrentPlayer == Player.White
                    ? -1
                    : 1;
                var enPassentCapturePosition = targetPosition + (BoardSize * moveDirection);
                Board[enPassentCapturePosition] = null;
            }

            SwitchPlayer();
            return moveType;
        }

        public void Promote(int position, PieceType promotion)
        {
            var row = position / 8;
            if ((CurrentPlayer == Player.White && row != BoardSize - 1) || (CurrentPlayer == Player.Black) && row != 0)
                throw new InvalidOperationException("Piece is not on promotion field");
            if (Board[position]?.PieceType != PieceType.Pawn)
                throw new InvalidOperationException("Piece for promotion is not a pawn");

            Board[position] = new Piece(promotion, CurrentPlayer);
            SwitchPlayer();
        }

        public List<MoveOption> GetMoves(int piecePosition)
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

        private List<MoveOption> GetKingMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<MoveOption> GetQueenMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<MoveOption> GetRockMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<MoveOption> GetBishopMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<MoveOption> GetKnightMoves(int piecePosition, Piece piece)
        {
            throw new NotImplementedException();
        }

        private List<MoveOption> GetPawnMoves(int piecePosition, Piece piece)
        {
            var moves = new List<MoveOption>();
            var (_, player) = piece;
            var direction = (int) player;

            // check if piece is in front
            var moveInFront = piecePosition + (BoardSize * direction);
            if (Board[moveInFront] is null)
                moves.Add(new MoveOption(moveInFront, MoveType.Regular));

            // 2 moves on start position
            var column = piecePosition % BoardSize;
            var row = piecePosition / BoardSize;
            
            var isOnStartPosition = (player == Player.White && row == 1) ||
                                    (player == Player.Black && row == BoardSize - 2);
            if (isOnStartPosition)
            {
                var doubleMove = piecePosition + (BoardSize * 2 * direction);
                if (Board[doubleMove] is null)
                    moves.Add(new MoveOption(doubleMove, MoveType.Regular));
            }

            // take left
            if (column > 0)
            {
                var leftTakePosition = piecePosition + (BoardSize - 1) * direction;
                if (Board[leftTakePosition] is not null)
                    moves.Add(new MoveOption(leftTakePosition, MoveType.Capture));
            }

            // take right
            if (column < 7)
            {
                var rightTakePosition = piecePosition + (BoardSize + 1) * direction;
                if (Board[rightTakePosition] is not null)
                    moves.Add(new MoveOption(rightTakePosition, MoveType.Capture));
            }

            // check for en passent
            if (LastDoublePawnMoveRow != null)
            {
                var (lastDoublePawnMoveRow, lastDoublePawnMoveColumn) = LastDoublePawnMoveRow;
                if (lastDoublePawnMoveRow == row &&
                    Math.Abs(lastDoublePawnMoveColumn - column) == 1)
                {
                    var enPassentMove = BoardSize * lastDoublePawnMoveRow + lastDoublePawnMoveColumn +
                                        (BoardSize * direction);
                    moves.Add(new MoveOption(enPassentMove, MoveType.EnPassent));
                }
            }

            return moves;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.White
                ? Player.Black
                : Player.White;
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