using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace DungeonGenerator {
    /**
     * Holds all the dungeon information.
     */
    public class Dungeon {

        public void Display() {
            Application.Run(new DungeonDisplayer(this));
        }

        private enum Heading {
            Info, Warning, Attention
        }

        private string[] headings = {
                                        "§1Info§r",
                                        "§2Warning§r",
                                        "§8Attention§r"
                                    };

        private string getHeading(Heading h) {
            return "§7[§r" + headings[(int)h] + "§7]§r";
        }

        private void print(Heading type, string text, object[] p) {
            Program.printf(getHeading(type) + " " + text, p);
            Console.Out.WriteLine();
        }

        private void print(Heading type, string text) {
            print(type, text, null);
        }

        private struct Param {
            public int maxMapWidth;
            public int maxMapHeight;
            public int floors;
            public int chestCount;
            public int roomsCount;
            public bool allowLockedDoors;
        }

        Param param;

        public Dungeon(int width, int height, int floors, int chestCount, int roomsNumber, bool allowLockedDoors) {
            param = new Param();
            param.maxMapWidth = width;
            param.maxMapHeight = height;
            param.floors = floors;
            param.chestCount = chestCount;
            param.roomsCount = roomsNumber;
            param.allowLockedDoors = allowLockedDoors;

            print(Heading.Info, "Init new dungeon. Parameters:\n Width: {0}\n Height: {1}\n Floors: {2}\n Chests: {3}\n Rooms: {4}\n Locked doors: {5}\n",
                new object[] {
                    width, height, floors, chestCount, roomsNumber, allowLockedDoors ? "true" : "false"
                });
        }

        private List<Room> rooms;
        private Random rg;

        /**
         * The plan is: Generate 2*roomsNumber rooms, outshift them and then connect with corridors, removing the ones that intersect a corridor
         * adding locked doors and making sure that the it's possible to reach every room.
         * And I don't fukin' know how to do all of that.
         * 
         * Okay, let's start by creating the Dungeon itself. This object will contain all the map data, that is, the map itself as well as
         * any object metadata (for monster spawners, mechanisms, chest contents, and so on.
         * 
         * Each object that has metadata will hava a unique ID, as the metadata for said object will be stored separetely, in JSON format.
         * The map itself won't retain the Rooms information, and will be turned into a tileset, or so it will be for DunGen 1.0 file specification.
         * 
         * Also, this comments will be very helpful, as I'm most likely going to forget everything I just said, and will come back very
         * often to remember what it is I have to implement next.
         */

        public void generate() {
            int gap = 2;
            int seed = (new Random()).Next();
            rg = new Random(seed);
            int excess = 2;

            print(Heading.Attention, "Starting dungeon generation...");
            print(Heading.Info, "Seed: {0}", new object[] { seed });
            print(Heading.Info, "Generating {0} rooms", new object[] { excess * param.roomsCount });

            rooms = new List<Room>();

            rooms.Add(new BasicRoom(-3, -3, 5, 5));

            //for (int i = 0; i < 10; i++) {
            //    for (int j = 0; j < 10; j++) {
            //        rooms.Add(new BasicRoom(i * 10, j * 10, 10, 10));
            //    }
            //}

            //Application.Run(new DungeonDisplayer(this));

            //return;

            int attemptCount = 0;
            for (int i = 0; i < excess * param.roomsCount; ) {
                if (generateRoom()) {
                    i++;
                    attemptCount = 0;
                } else {
                    attemptCount++;
                    print(Heading.Warning, "Room failed to generate. Attempt #{0}", new object[] { attemptCount });
                }
            }

            //rooms.OrderBy((x) => Helpers.Distance(x.getCenter(), new Point(0, 0)));

            print(Heading.Attention, "Starting outshifting!");
            print(Heading.Info, "{0} set as middle, with center at {1}.", new object[] { rooms[0], rooms[0].getCenter() });
            rooms[0].finalPosition = true;

            bool changed = true;
            while (changed) {
                changed = false;
                foreach (var r in rooms) {
                    if (r.finalPosition) {
                        continue;
                    }
                    r.shown = true;
                    while (!r.finalPosition) {
                        Point centerR = r.getCenter();
                        Point centerOR = rooms[0].getCenter();

                        //radialmente!
                        double d = Helpers.Distance(centerR, centerOR);
                        int[] p;
                        if (d == 0) {
                            p = new int[] { 1, 1 };
                        } else {
                            p = Helpers.IntegerProportion((centerR.x - centerOR.x) / (d), (centerR.y - centerOR.y) / (d), 30);
                        }

                        /*if (p[0] == 0 && p[1] == 0) {
                            p = new int[] { rg.Next(10) * (rg.NextDouble() > 0.5 ? 1 : -1), rg.Next(10) * (rg.NextDouble() > 0.5 ? 1 : -1) };
                        }*/

                        print(Heading.Info, "Increment: {0} {1}", new object[] { p[0], p[1] });
                        r.shift(p[0], p[1]);

                        //Application.Run(new DungeonDisplayer(this));

                        changed = true;

                        bool fix = true;
                        foreach (var or in rooms) {
                            if (r.Equals(or) || !or.finalPosition) {
                                continue;
                            }

                            if (or.getDistance(r) < gap) {
                                fix = false;
                            }

                        }

                        if (fix) {
                            r.finalPosition = true;
                            print(Heading.Info, "{0} is now fixed.", new object[] { r });
                        }
                    }
                    r.shown = false;
                }
            }

            print(Heading.Attention, "Shift finished.");
            print(Heading.Attention, "Deleting {0} random rooms.", new object[] { (excess - 1) * param.roomsCount });

            for (int i = 0; i < (excess - 1) * param.roomsCount; i++) {
                Room r = rooms[rg.Next(rooms.Count)];
                rooms.Remove(r);
            }

            print(Heading.Attention, "Done. Displaying.");
            Application.Run(new DungeonDisplayer(this));
        }

        public Rectangle getTrimmedSize() {
            int minX = 0, minY = 0, maxX = 0, maxY = 0;
            bool set = false;
            foreach (var room in rooms) {
                foreach (var cb in room.getCollisionBoxes()) {
                    minX = (cb.x < minX || !set) ? cb.x : minX;
                    minY = (cb.y < minY || !set) ? cb.y : minY;
                    maxX = (cb.x > maxX || !set) ? cb.x : maxX;
                    maxY = (cb.y > maxY || !set) ? cb.y : maxY;
                    set = true;
                }
            }
            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            //return new Rectangle(-30, -30, 100, 100);
        }

        public Color[][] getColored() {
            //Rectangle size = getTrimmedSize();
            Tile[][] tiles = getTiles();
            Color[][] colors = new Color[tiles.Length][];

            for (int i = 0; i < tiles.Length; i++) {
                colors[i] = new Color[tiles[i].Length];
                for (int j = 0; j < tiles[i].Length; j++) {
                    colors[i][j] = tiles[i][j].getColor();
                }
            }

            return colors;
        }

        public Tile[][] getTiles() {
            Rectangle size = getTrimmedSize();
            Tile[][] theTiles = new Tile[size.h][];

            for (int i = 0; i < size.h; i++) {
                theTiles[i] = new Tile[size.w];
                for (int j = 0; j < size.w; j++) {
                    theTiles[i][j] = new TileEmpty(i, j);
                }
            }

            foreach (var room in rooms) {
                Tile[][] roomTiles = room.getTiles();
                for (int i = 0; i < roomTiles.Length; i++) {
                    for (int j = 0; j < roomTiles[i].Length; j++) {
                        Tile tile = roomTiles[i][j];
                        tile.x -= size.x;
                        tile.y -= size.y;
                        theTiles[tile.y][tile.x] = tile;
                    }
                }
            }

            return theTiles;
        }

        private bool generateRoom() {
            //Let's do a probabilistic distribution that tends towards the basic room
            // this is not very effective for many room types, and is very likely to change as more types are added.
            int num = Room.TYPES_COUNT - ((int)Math.Log(rg.NextDouble() * Math.Pow(Math.E, Room.TYPES_COUNT - 1) + 1)) - 1;
            print(Heading.Info, "Trying to generate room of type: {0}", new object[] { num });

            Room newRoom = null;

            switch (num) {
                case 0:
                    newRoom = BasicRoom.generate(Room.RoomSize.Huge, rg, 30);
                    print(Heading.Info, "Generated: {0}", new object[] { newRoom });
                    break;
            }

            rooms.Add(newRoom);

            return true;
        }
    }
}
