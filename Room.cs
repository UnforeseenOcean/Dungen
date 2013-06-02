using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator {
    abstract class Room {
        public enum RoomSize {
            Small, Medium, Big, Huge
        }

        public bool shown = false;

        public static int TYPES_COUNT = 1;
        public Rectangle bounds;

        public bool finalPosition = false;

        abstract public Rectangle[] getCollisionBoxes();
        abstract public Point[] getConnectionPoints();
        abstract public Tile[][] getTiles();
        abstract public void shift(int xShift, int yShift);
        abstract public Point getCenter();

        /**
         * Return the "center of mass".
         * Avoid using, as it uses a very slow method.
         */
        public Point getCenterOfMass() {
            double cx = 0, cy = 0;

            Rectangle[] cb = getCollisionBoxes();

            if (cb.Length == 0) {
                throw new InvalidOperationException("The object has no collision box.");
            }

            int count = 0;
            foreach (var p in cb) {
                count++;
                cx += (p.x + p.w / 2.0);
                cy += (p.y + p.h / 2.0);
            }


            Point center = new Point((int)(cx / count), (int)(cy / count));

            return center;
        }

        /**
         * Return the smallest distance between the collision boxes
         */
        public double getDistance(Room other) {
            double smallest = 0;

            Rectangle[] ocb = other.getCollisionBoxes();
            Rectangle[] cb = getCollisionBoxes();

            for (int i = 0; i < ocb.Length; i++) {
                for (int j = 0; j < cb.Length; j++) {
                    double distance = Helpers.Distance(ocb[i], cb[j]);
                    if (distance == 0) {
                        return 0;
                    }
                    smallest = (i == 0 && j == 0) ? distance : Math.Min(distance, smallest);
                }
            }

            return smallest;

        }

        /**
         * Generates the contents of this room and adds the TileEntities to the tiles.
         * Use getTiles() to get the results.
         */
        abstract public void generateContent(int numChests);

        public List<Point> connections;

        /**
         * Adds p to the list of connections. Used to avoid path obstruction.
         * <param name="p">The point relative to the room</param>
         */
        public void setConnection(Point p) {
            connections.Add(p);
        }
    }


    /**
     * Basic, square rooms. Entry points can be on any wall.
     */
    class BasicRoom : Room {

        private struct BasicRoomSize {
            public int minW, minH, maxW, maxH;

            public BasicRoomSize(int mw, int mh, int xw, int xh) {
                minW = mw;
                minH = mh;
                maxH = xh;
                maxW = xw;
            }
        }

        static BasicRoomSize[] SizeDef = {
                                             new BasicRoomSize(4, 4, 7, 7),
                                             new BasicRoomSize(8, 8, 13, 13),
                                             new BasicRoomSize(13, 13, 17, 17),
                                             new BasicRoomSize(40, 40, 80, 80)
                                         };

        public static int Dispersion = 30;

        public BasicRoom(int x, int y, int w, int h) {
            bounds = new Rectangle(x, y, w, h);
        }

        public override Rectangle[] getCollisionBoxes() {
            Rectangle[] r = new Rectangle[bounds.w * bounds.h];

            for (int i = 0; i < bounds.h; i++) {
                for (int j = 0; j < bounds.w; j++) {
                    r[i * bounds.w + j] = new Rectangle(bounds.x + j, bounds.y + i, 1, 1);
                }
            }

            return r;
        }

        public override string ToString() {
            return "Room (" + bounds.x + ", " + bounds.y + ", " + bounds.w + ", " + bounds.h + ")";
        }

        public static Room generate(RoomSize size, Random r, int dispersion) {
            BasicRoomSize s = SizeDef[(int)size];
            return new BasicRoom((int)((r.NextDouble() < 0.5 ? 1 : -1) * (Math.Pow(dispersion, r.NextDouble()) - 1)),
                (int)((r.NextDouble() < 0.5 ? 1 : -1) * (Math.Pow(dispersion, r.NextDouble()) - 1)),
                (int)((s.maxW - s.minW) * r.NextDouble() + s.minW),
                (int)((s.maxH - s.minH) * r.NextDouble() + s.minH));
        }

        public override void generateContent(int numChests) {
            throw new NotImplementedException();
        }

        public override Tile[][] getTiles() {
            Tile[][] tiles = new Tile[bounds.h][];

            for (int i = 0; i < bounds.h; i++) {
                tiles[i] = new Tile[bounds.w];
                for (int j = 0; j < bounds.w; j++) {
                    if (i == 0 || j == 0 || j == bounds.w - 1 || i == bounds.h - 1) {
                        tiles[i][j] = new TileWall(bounds.x + j, bounds.y + i);
                    } else {
                        tiles[i][j] = new TileFloor(bounds.x + j, bounds.y + i);
                    }
                }
            }

            return tiles;
        }

        public override Point[] getConnectionPoints() {
            throw new NotImplementedException();
        }

        public override void shift(int xShift, int yShift) {
            bounds.x += xShift;
            bounds.y += yShift;
        }

        public override Point getCenter() {
            return new Point((bounds.x + bounds.w) / 2, (bounds.y + bounds.h) / 2);
        }
    }
}
