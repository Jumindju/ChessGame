using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessGameLogix;
using ChessGameLogix.Pieces;
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

        public ChessBoard()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            _chessBoardPanels = new Panel[TileCount];

            var chessPiecesSprite = Image.FromFile("./Resc/ChessPieceSprite.png");

            var pawn = new Pawn(0, Player.Black);
            var pawnImg = PieceHelper.GetPieceImage(chessPiecesSprite, pawn);

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
            }
            
            _chessBoardPanels[0].BackgroundImage = pawnImg;

            RenderBoard();
        }

        protected override void OnResize(EventArgs e)
        {
            RenderBoard();
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

                var tilePanel = _chessBoardPanels[cell];

                tilePanel.Size = new Size(tileSize, tileSize);
                tilePanel.Location = new Point(paddingLeft + (tileSize * column),
                    paddingTop + (tileSize * row));
            }
        }
    }
}