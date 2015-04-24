using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DriveFrame
{
    public class KProgressBar : UserControl
    {
        public KProgressBar()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(120, Color.White));
            //g.DrawRectangle(new Pen(Color.FromArgb(128, Color.DeepSkyBlue)), 0, 0, this.Width - 1, this.Height - 1);
            g.FillRectangle(Brushes.DeepSkyBlue, 0, 0, this.Width * Value / 100, this.Height);
        }

        int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                this.Invalidate();
            }
        }
    }
}
