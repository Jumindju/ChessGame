using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ChessGameUI.Forms
{
    public class ChessTile : Panel
    {
        public ChessTile()
        {
            SetStyle(
                ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.DoubleBuffer |
                ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using var brush = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(brush, ClientRectangle);

            if (BackgroundImage is not null)
            {
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                
                e.Graphics.DrawImage(BackgroundImage, 0, 0, ClientRectangle.Width, ClientRectangle.Height);
            }

            e.Graphics.DrawRectangle(Pens.Black, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
        }
    }
}