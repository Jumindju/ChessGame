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
        private const int BoardSize = 8;
        private const int TileCount = BoardSize * BoardSize;
        private static readonly Color _whiteColor = Color.White;
        private static readonly Color _blackColor = Color.FromArgb(255, 18, 122, 29);
        private const int BoardPadding = 10;

        private Panel[] _chessBoardPanels;
        private PictureBox?[] _pieceImageBoxes;
        private readonly Dictionary<Piece, Image> _pieceImageCache = new();

        private readonly Game _game;

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
            _chessBoardPanels = new Panel[TileCount];
            _pieceImageBoxes = new PictureBox[TileCount];

            // add all cells to the game board after another (8 >> 2 = 64)
            for (var cell = 0; cell < TileCount; cell++)
            {
                var column = cell % BoardSize;
                var row = cell / BoardSize;

                // panel represents a tile
                var tilePanel = new ChessTile();

                // add to Form's Controls so that they show up
                Controls.Add(tilePanel);

                // add to our 2d array of panels for future use
                _chessBoardPanels[cell] = tilePanel;

                // color the backgrounds
                tilePanel.BackColor = (row % 2 + column % 2) != 1
                    ? _whiteColor
                    : _blackColor;

                // add empty image box
                var piece = _game.Board[cell];
                if (piece != null)
                {
                    var pieceImgBox = new PictureBox
                    {
                        Image = _pieceImageCache[piece],
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Cursor = Cursors.Hand
                    };

                    tilePanel.Controls.Add(pieceImgBox);

                    _pieceImageBoxes[cell] = pieceImgBox;
                }
            }
        }

        private void RenderBoard()
        {
            if (_chessBoardPanels == null)
                return;

            var tileWidth = ClientSize.Width / BoardSize;
            var tileHeight = (ClientSize.Height - 2 * BoardPadding) / BoardSize;
            var tileSize = Math.Min(tileWidth, tileHeight);

            var paddingLeft = (ClientSize.Width - (tileSize * BoardSize)) / 2;
            var paddingTop = (ClientSize.Height - (tileSize * BoardSize)) / 2;

            for (var cell = 0; cell < TileCount; cell++)
            {
                var column = cell % BoardSize;
                var row = cell / BoardSize;

                // position tiles
                var tilePanel = _chessBoardPanels[cell];

                tilePanel.Size = new Size(tileSize, tileSize);
                tilePanel.Location = new Point(paddingLeft + (tileSize * column),
                    paddingTop + (tileSize * (BoardSize - 1 - row)));

                // position pieces
                var piece = _game.Board[cell];
                if (piece != null)
                {
                    var pictureBox = _pieceImageBoxes[cell];
                    pictureBox.Size = new Size(tileSize - 2, tileSize - 2);
                    pictureBox.Location = new Point(1, 1);
                }
            }
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