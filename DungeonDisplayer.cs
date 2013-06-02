using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DungeonGenerator {
    public partial class DungeonDisplayer : Form {
        private Dungeon dungeon;

        public DungeonDisplayer(Dungeon dungeon) {
            InitializeComponent();

            if (dungeon == null) {
                throw new NullReferenceException("Invalid argument for DungeonDisplayer");
            }
            this.dungeon = dungeon;

            this.ClientSize = new Size(800, 600);
        }

        protected override void OnPaint(PaintEventArgs e) {
            SolidBrush black = new SolidBrush(Color.Black);
            Graphics formGraphics = this.CreateGraphics();

            formGraphics.FillRectangle(black, new RectangleF(0, 0, ClientSize.Width, ClientSize.Height));

            Color[][] colors = dungeon.getColored();

            int xsize = ClientSize.Width / colors[0].Length, ysize = ClientSize.Height / colors.Length;
            xsize = Math.Min(xsize, ysize);
            ysize = Math.Min(xsize, ysize);

            for (int i = 0; i < colors.Length; i++) {
                for (int j = 0; j < colors[i].Length; j++) {
                    SolidBrush brush = new SolidBrush(colors[i][j]);
                    formGraphics.FillRectangle(brush, new RectangleF(j * xsize, i * ysize, xsize, ysize));
                    brush.Dispose();
                }
            }

            formGraphics.Dispose();
            black.Dispose();

            //base.OnPaint(e);
        }

        private void DungeonDisplayer_Resize(object sender, EventArgs e) {
            OnPaint(null);
        }

        private void DungeonDisplayer_ClientSizeChanged(object sender, EventArgs e) {
            OnPaint(null);
        }
    }
}
