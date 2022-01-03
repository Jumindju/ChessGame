using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessGameLogic;
using ChessGameLogic.Pieces;
using ChessGameUI.Forms;
using ChessGameUI.Helper;

namespace ChessGameUI
{
    public partial class ChessBoardForm : Form
    {
        private static readonly Color _whiteColor = Color.White;
        private static readonly Color _blackColor = Color.FromArgb(255, 18, 122, 29);
        private static readonly Color _highlightColor = Color.FromArgb(71, 137, 255);
        private static readonly Color _takeHighlightColor = Color.FromArgb(255, 83, 84);

        private const int BoardPadding = 10;

        private Panel[] _tilePanels;
        private PictureBox?[] _pieceImageBoxes;
        private Dictionary<Piece, Image> _pieceImageCache;

        private readonly Game _game;

        private int? _currentSelectedPiecePosition;
        private List<MoveOption>? _currentlyHighlightedMoves;

        public int TileSize { get; private set; }
        public int PaddingLeft { get; private set; }

        public ChessBoardForm()
        {
            InitializeComponent();
            _game = new Game();
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadPieceImages();

            SetupUIElements();

            RenderBoard();
        }

        protected override void OnResize(EventArgs e)
        {
            RenderBoard();
        }

        private void SetupUIElements()
        {
            _tilePanels = new Panel[Game.TileCount];
            _pieceImageBoxes = new PictureBox[Game.TileCount];

            // add all cells to the game board after another (8 >> 2 = 64)
            for (var cell = 0; cell < Game.TileCount; cell++)
            {
                var column = cell % Game.BoardSize;
                var row = cell / Game.BoardSize;

                // panel represents a tile
                var tilePanel = new ChessTile
                {
                    Tag = cell
                };
                tilePanel.Click += OnTileClick;

                // add to Form's Controls so that they show up
                Controls.Add(tilePanel);

                // add to our 2d array of panels for future use
                _tilePanels[cell] = tilePanel;

                // add empty image box
                var piece = _game.Board[cell];
                if (piece != null)
                {
                    var pieceImgBox = new PictureBox
                    {
                        Image = _pieceImageCache[piece],
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Cursor = Cursors.Hand,
                        Tag = cell,
                        Location = new Point(1, 1)
                    };
                    pieceImgBox.Click += OnPieceClick;

                    _pieceImageBoxes[cell] = pieceImgBox;
                }
            }
        }

        private void RenderBoard()
        {
            if (_tilePanels == null)
                return;

            var tileWidth = ClientSize.Width / Game.BoardSize;
            var tileHeight = (ClientSize.Height - 2 * BoardPadding) / Game.BoardSize;
            TileSize = Math.Min(tileWidth, tileHeight);

            PaddingLeft = (ClientSize.Width - (TileSize * Game.BoardSize)) / 2;
            var paddingTop = (ClientSize.Height - (TileSize * Game.BoardSize)) / 2;

            for (var cell = 0; cell < Game.TileCount; cell++)
            {
                var column = cell % Game.BoardSize;
                var row = cell / Game.BoardSize;

                // position tiles
                var tilePanel = _tilePanels[cell];
                tilePanel.Controls.Clear();

                tilePanel.Size = new Size(TileSize, TileSize);
                tilePanel.Location = new Point(PaddingLeft + (TileSize * column),
                    paddingTop + (TileSize * (Game.BoardSize - 1 - row)));

                // color the backgrounds
                var highlightedMove = _currentlyHighlightedMoves?.FirstOrDefault(hm => hm.MovePosition == cell);
                if (highlightedMove is not null)
                {
                    tilePanel.BackColor = GetHighlightColor(highlightedMove.MoveType);
                    tilePanel.Cursor = Cursors.Hand;
                }
                else
                {
                    tilePanel.BackColor = GetTileBackColor(cell);
                    tilePanel.Cursor = Cursors.Default;
                }

                tilePanel.Cursor = Cursors.Default;

                // position pieces
                var piece = _game.Board[cell];
                if (piece != null)
                {
                    var pictureBox = _pieceImageBoxes[cell];
                    tilePanel.Controls.Add(pictureBox);

                    pictureBox.Size = new Size(TileSize - 2, TileSize - 2);
                }
            }
        }

        private void OnTileClick(object? sender, EventArgs args)
        {
            if (_currentlyHighlightedMoves is null || _currentSelectedPiecePosition is null)
                return;

            var targetedPosition = (int) ((ChessTile) sender).Tag;
            var targetMove = _currentlyHighlightedMoves?.FirstOrDefault(hm => hm.MovePosition == targetedPosition);

            // move to the desired position
            if (targetMove is not null)
            {
                MovePiece(targetMove);
            }
            else
            {
                UndoHighlighting();
                _currentlyHighlightedMoves = null;
                _currentSelectedPiecePosition = null;
            }
        }

        private void OnPieceClick(object? sender, EventArgs e)
        {
            var piecePosition = (int) ((PictureBox) sender).Tag;
            var targetMove = _currentlyHighlightedMoves?.FirstOrDefault(hm => hm.MovePosition == piecePosition);

            var piece = _game.Board[piecePosition];
            if (piece is null)
                return;

            if (piece.Player != _game.CurrentPlayer)
            {
                if (targetMove is not null)
                    MovePiece(targetMove);

                return;
            }

            _currentSelectedPiecePosition = piecePosition;

            var pieceMoves = _game.GetMoves(piecePosition);
            var lastHighlighted = _currentlyHighlightedMoves != null
                ? new List<MoveOption>(_currentlyHighlightedMoves)
                : null;

            _currentlyHighlightedMoves = pieceMoves;

            HighlightPossibleTiles(lastHighlighted);
        }

        private void MovePiece(MoveOption targetMove)
        {
            UndoHighlighting();

            var selectedPiecePosition = _currentSelectedPiecePosition.Value;
            var targetedPosition = targetMove.MovePosition;

            var move = _game.MovePiece(selectedPiecePosition, targetMove);
            var oldTile = _tilePanels[selectedPiecePosition];
            oldTile.Controls.Clear();

            var pieceImageBox = _pieceImageBoxes[selectedPiecePosition];
            var targetedTile = _tilePanels[targetedPosition];
            targetedTile.Controls.Clear();
            targetedTile.Controls.Add(pieceImageBox);

            _pieceImageBoxes[targetedPosition] = pieceImageBox;
            pieceImageBox.Tag = targetMove.MovePosition;

            _pieceImageBoxes[selectedPiecePosition] = null;

            _currentlyHighlightedMoves = null;
            _currentSelectedPiecePosition = null;

            if (move == MoveType.Promotion)
            {
                var promotionDialog = new PromotionDialog(_game.CurrentPlayer)
                {
                    TopMost = true
                };
                promotionDialog.ShowDialog(this);
                var promotionPiece = promotionDialog.SelectedPiece;
                _game.Promote(targetedPosition, promotionPiece);
                pieceImageBox.Image = GetPieceImage(_game.Board[targetedPosition]);
            }
            else if (move == MoveType.EnPassent)
            {
                // switch direction because player where already switched in MovePiece method
                var direction = _game.CurrentPlayer == Player.White
                    ? 1
                    : -1;
                var enPassentCapturePosition = targetedPosition + (Game.BoardSize * direction);
                _tilePanels[enPassentCapturePosition].Controls.Clear();
                _pieceImageBoxes[enPassentCapturePosition] = null;
            }
        }

        private void HighlightPossibleTiles(List<MoveOption>? lastHighlightedTiles)
        {
            foreach (var (movePosition, moveType) in _currentlyHighlightedMoves)
            {
                var highlightedPanel = _tilePanels[movePosition];
                highlightedPanel.BackColor = GetHighlightColor(moveType);
                highlightedPanel.Cursor = Cursors.Hand;
            }

            if (lastHighlightedTiles is null)
                return;

            foreach (var (movePosition, _) in lastHighlightedTiles)
            {
                var lastHighlightedTile = _tilePanels[movePosition];
                lastHighlightedTile.BackColor = GetTileBackColor(movePosition);
                lastHighlightedTile.Cursor = Cursors.Default;
            }
        }

        private void UndoHighlighting()
        {
            foreach (var (movePosition, _) in _currentlyHighlightedMoves)
            {
                var highlightedTile = _tilePanels[movePosition];
                highlightedTile.BackColor = GetTileBackColor(movePosition);
                highlightedTile.Cursor = Cursors.Default;
            }
        }

        private static Color GetHighlightColor(MoveType moveType) =>
            moveType == MoveType.Capture || moveType == MoveType.EnPassent
                ? _takeHighlightColor
                : _highlightColor;

        private static Color GetTileBackColor(int position)
        {
            var column = position % Game.BoardSize;
            var row = position / Game.BoardSize;

            return row % 2 + column % 2 != 1
                ? _whiteColor
                : _blackColor;
        }

        private void LoadPieceImages()
        {
            _pieceImageCache = new Dictionary<Piece, Image>();
            var chessPiecesSprite = Image.FromFile("./Resc/ChessPieceSprite.png");

            foreach (PieceType pieceType in Enum.GetValues(typeof(PieceType)))
            {
                var whitePiece = new Piece(pieceType, Player.White);
                var blackPiece = new Piece(pieceType, Player.Black);

                _pieceImageCache[whitePiece] = PieceHelper.GetPieceImage(chessPiecesSprite, whitePiece);
                _pieceImageCache[blackPiece] = PieceHelper.GetPieceImage(chessPiecesSprite, blackPiece);
            }
        }

        public Image GetPieceImage(Piece piece)
            => _pieceImageCache[piece];
    }
}