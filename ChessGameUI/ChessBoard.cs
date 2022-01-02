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
    public partial class ChessBoard : Form
    {
        private static readonly Color _whiteColor = Color.White;
        private static readonly Color _blackColor = Color.FromArgb(255, 18, 122, 29);
        private static readonly Color _highlightColor = Color.FromArgb(71, 137, 255);
        private static readonly Color _takeHighlightColor = Color.FromArgb(255, 83, 84);

        private const int BoardPadding = 10;

        private Panel[] _tilePanels;
        private PictureBox?[] _pieceImageBoxes;
        private readonly Dictionary<Piece, Image> _pieceImageCache = new();

        private readonly Game _game;

        private int? _currentSelectedPiecePosition;
        private List<int>? _currentlyHighlightedMoves;

        public ChessBoard()
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
            var tileSize = Math.Min(tileWidth, tileHeight);

            var paddingLeft = (ClientSize.Width - (tileSize * Game.BoardSize)) / 2;
            var paddingTop = (ClientSize.Height - (tileSize * Game.BoardSize)) / 2;

            for (var cell = 0; cell < Game.TileCount; cell++)
            {
                var column = cell % Game.BoardSize;
                var row = cell / Game.BoardSize;

                // position tiles
                var tilePanel = _tilePanels[cell];
                tilePanel.Controls.Clear();

                tilePanel.Size = new Size(tileSize, tileSize);
                tilePanel.Location = new Point(paddingLeft + (tileSize * column),
                    paddingTop + (tileSize * (Game.BoardSize - 1 - row)));

                // color the backgrounds
                if (_currentlyHighlightedMoves is not null && _currentlyHighlightedMoves.Contains(cell))
                {
                    tilePanel.BackColor = GetHighlightColor(cell);
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

                    pictureBox.Size = new Size(tileSize - 2, tileSize - 2);
                }
            }
        }

        private void OnTileClick(object? sender, EventArgs args)
        {
            if (_currentlyHighlightedMoves is null || _currentSelectedPiecePosition is null)
                return;

            var targetedPosition = (int) ((ChessTile) sender).Tag;

            // move to the desired position
            if (_currentlyHighlightedMoves.Contains(targetedPosition))
            {
                MovePiece(targetedPosition);
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
            var piece = _game.Board[piecePosition];
            if (piece is null)
                return;

            if (piece.Player != _game.CurrentPlayer)
            {
                if (_currentlyHighlightedMoves is not null &&
                    _currentlyHighlightedMoves.Contains(piecePosition))
                    MovePiece(piecePosition);

                return;
            }

            _currentSelectedPiecePosition = piecePosition;

            var pieceMoves = _game.GetMoves(piecePosition);
            var lastHighlighted = _currentlyHighlightedMoves != null
                ? new List<int>(_currentlyHighlightedMoves)
                : null;

            _currentlyHighlightedMoves = pieceMoves;

            HighlightPossibleTiles(lastHighlighted);
        }

        private void MovePiece(int targetedPosition)
        {
            UndoHighlighting();

            var selectedPiecePosition = _currentSelectedPiecePosition.Value;

            _game.DoMove(selectedPiecePosition, targetedPosition);

            var oldTile = _tilePanels[selectedPiecePosition];
            oldTile.Controls.Clear();

            var pieceImageBox = _pieceImageBoxes[selectedPiecePosition];
            var targetedTile = _tilePanels[targetedPosition];
            targetedTile.Controls.Clear();
            targetedTile.Controls.Add(pieceImageBox);

            _pieceImageBoxes[targetedPosition] = pieceImageBox;
            pieceImageBox.Tag = targetedPosition;

            _pieceImageBoxes[selectedPiecePosition] = null;

            _currentlyHighlightedMoves = null;
            _currentSelectedPiecePosition = null;
        }

        private void HighlightPossibleTiles(List<int>? lastHighlightedTiles)
        {
            foreach (var possibleMove in _currentlyHighlightedMoves)
            {
                var highlightedPanel = _tilePanels[possibleMove];
                highlightedPanel.BackColor = GetHighlightColor(possibleMove);
                highlightedPanel.Cursor = Cursors.Hand;
            }

            if (lastHighlightedTiles is null)
                return;

            foreach (var lastHighlighted in lastHighlightedTiles)
            {
                var lastHighlightedTile = _tilePanels[lastHighlighted];
                lastHighlightedTile.BackColor = GetTileBackColor(lastHighlighted);
                lastHighlightedTile.Cursor = Cursors.Default;
            }
        }

        private void UndoHighlighting()
        {
            foreach (var currentlyHighlightedMove in _currentlyHighlightedMoves)
            {
                var highlightedTile = _tilePanels[currentlyHighlightedMove];
                highlightedTile.BackColor = GetTileBackColor(currentlyHighlightedMove);
                highlightedTile.Cursor = Cursors.Default;
            }
        }

        private Color GetHighlightColor(int position) =>
            _game.Board[position] is null
                ? _highlightColor
                : _takeHighlightColor;

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
            var chessPiecesSprite = Image.FromFile("./Resc/ChessPieceSprite.png");

            foreach (PieceType pieceType in Enum.GetValues(typeof(PieceType)))
            {
                var whitePiece = new Piece(pieceType, Player.White);
                var blackPiece = new Piece(pieceType, Player.Black);

                _pieceImageCache[whitePiece] = PieceHelper.GetPieceImage(chessPiecesSprite, whitePiece);
                _pieceImageCache[blackPiece] = PieceHelper.GetPieceImage(chessPiecesSprite, blackPiece);
            }
        }
    }
}