using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DungeonGenerator {
    public abstract class Tile {
        abstract public System.Drawing.Color getColor();

        public int x, y;
    }

    public class TileEmpty : Tile {
        public override Color getColor() {
            return Color.Black;
        }

        public TileEmpty(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    public class TileWall : Tile {
        public override Color getColor() {
            return Color.DarkGray;
        }

        public TileWall(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    public class TileFloor : Tile {
        public override Color getColor() {
            return Color.LightGray;
        }

        public TileFloor(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    abstract class TileEntity {

    }

    class Chest : TileEntity {

    }

    class TreasureGold : TileEntity {

    }
}
