using System;
using System.Drawing;
using System.Windows.Forms;
using ChessGameLogic;
using ChessGameLogic.Pieces;

namespace ChessGameUI
{
    public partial class PromotionDialog : Form
    {
        private readonly Player _currentPlayer;

        public PieceType SelectedPiece { get; set; }

        public PromotionDialog(Player currentPlayer)
        {
            InitializeComponent();

            _currentPlayer = currentPlayer;
        }

        protected override void OnLoad(EventArgs e)
        {
            var chessBoard = (ChessBoardForm) Owner;
            var marginVert = (int) (chessBoard.TileSize * 1.25);
            var marginTop = chessBoard.TileSize / 4;
            var spaceBetween = chessBoard.TileSize / 4;

            var dialogWidth = 2 * marginVert + 4 * chessBoard.TileSize + 3 * spaceBetween;
            var dialogHeight = 2 * marginTop + chessBoard.TileSize;

            Size = new Size(dialogWidth, dialogHeight);
            Location = new Point((chessBoard.Size.Width - dialogWidth) / 2,
                (Owner.Size.Height / 2) - dialogHeight);


            RenderPromotionOptions();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
        }

        private void RenderPromotionOptions()
        {
            var chessBoard = (ChessBoardForm) Owner;
            var marginLeft = (int) (chessBoard.TileSize * 1.25);
            var marginTop = chessBoard.TileSize / 4;
            var spaceBetween = chessBoard.TileSize / 4;

            for (var i = 1; i <= 4; i++)
            {
                var pieceType = (PieceType) i;
                var pictureBox = new PictureBox
                {
                    Location = new Point(marginLeft + (i - 1) * (chessBoard.TileSize + spaceBetween), marginTop),
                    Size = new Size(chessBoard.TileSize, chessBoard.TileSize),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Tag = pieceType,
                    Cursor = Cursors.Hand,
                    Image = chessBoard.GetPieceImage(new Piece(pieceType, _currentPlayer))
                };

                pictureBox.Click += OnPromotionBtnClick;

                Controls.Add(pictureBox);
            }
        }

        private void OnPromotionBtnClick(object? sender, EventArgs e)
        {
            var pieceType = (PieceType) ((PictureBox) sender).Tag;
            SelectedPiece = pieceType;
            DialogResult = DialogResult.OK;
        }
    }
}