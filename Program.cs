using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator {

    abstract class Item {
        public ItemType itemID;
        public long itemData;
        public int stackSize;

        public enum ItemType {
            Key = 1024
        }
    }

    abstract class Tile {
        public TileType tileID;
        public long tileData;

        public enum TileType {
            Air,
            GenericFloor,
            GenericWall,
            Door,
            Chest,
            Spawn,
            StairsUp,
            StairsDown,
            Undefined
        }
    }

    class TileDoor : Tile {
        private int doorNumber;

        public TileDoor(int id, long data) {
            tileID = (TileType)id;
            tileData = data;

            doorNumber = (int)data;
        }

        public TileDoor(int number) {
            tileID = Tile.TileType.Door;
            doorNumber = number;
        }

        public int getDoorNumber() {
            return doorNumber;
        }
    }

    abstract class Room {
        abstract public Rectangle[] getCollisionBox();
    }


    /* 
     * Basic rooms are rectangular, and always have one explicit entrance. Other rooms may attach to it.
     * Their collision box is a rectangle of the same dimensions as the room.
     */
    class BasicRoom : Room {
        private int x, y, w, h;

        public BasicRoom(int x, int y, int w, int h) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public Rectangle[] getCollisionBox() {
            Rectangle[] r = new Rectangle[w * h];


            for (int i = 0; i < r.Length; i++) {
                r[i] = new Rectangle(x + i % w, y + i / w, 1, 1);
            }

            return r;
        }
    }

    class TileChest : Tile {
        public Item[] chestContents;

        public TileChest(int id, long data) {
            tileID = (TileType)id;
            tileData = data;
        }
    }



    class ItemKey : Item {
        public ItemKey(int id, long data) {
            itemID = (ItemType)id;
            itemData = data;
        }

        public ItemKey(TileDoor door) {
            itemData = door.getDoorNumber();
        }
    }

    /**
     * Holds all the dungeon information.
     */
    class Dungeon {
        private Room[] rooms;

    }

    class Program {
        private ConsoleColor[] CustomColors = new ConsoleColor[] { 
            ConsoleColor.Black, //0
            ConsoleColor.White, //1
            ConsoleColor.Red, //2
            ConsoleColor.Green, //3
            ConsoleColor.Blue, //4
            ConsoleColor.DarkBlue, //5
            ConsoleColor.DarkGreen, //6
            ConsoleColor.Gray //7
        };
        private bool exitRequested;

        /**
         * This is the main method of the program, where it all happens.
         */
        private void generateDungeon(int width, int height, int floors, int chestCount, int roomsNumber, bool allowLockedDoors) {
            /**
             * The plan is: Generate roomsNumber rooms, outshift them and then connect with corridors, adding locked doors and making
             * sure that the it's possible to reach every room.
             * And I don't fukin' know how to do all of that.
             **/

            /**
             * Okay, let's start by creating the Dungeon itself. This object will contain all the map data, that is, the map itself as well as
             * any object metadata (for monster spawners, mechanisms, chest contents, and so on.
             * 
             * Each object that has metadata will hava a unique ID, as the metadata for said object will be stored separetely, in JSON format.
             * The map itself won't retain the Room information, and will be turned into a tileset, or so it will be for DunGen 1.0 file specification.
             * 
             * Also, this comments will be very helpful, as I'm most likely going to forget everything I just said, and will come back very
             * often to remember what it is I have to implement next.
             */


        }

        private void print(string str) {
            bool prevsim = false;
            ConsoleColor originalColor = Console.ForegroundColor;
            for (int i = 0; i < str.Length; i++) {
                char c = str.ToCharArray(i, 1)[0];
                int cv = getCharValue(c);
                if (prevsim) {
                    switch (c) {
                        case 'r':
                            Console.ForegroundColor = originalColor;
                            break;
                        default:
                            if (cv > 0 && cv < CustomColors.Length) {
                                Console.ForegroundColor = CustomColors[cv];
                            }
                            break;
                    }


                    prevsim = false;
                } else {
                    if (c.Equals('§')) {
                        prevsim = true;
                    } else {
                        Console.Out.Write(c);
                    }
                }
            }
        }

        private void println(string str) {
            print(str);
            Console.Out.WriteLine();
        }

        private int getCharValue(char c) {
            if (c >= '0' && c <= '9') {
                return c - '0';
            } else if (c >= 'a' && c <= 'f') {
                return c - 'a' + '9' - '0';
            }

            return -1;
        }

        private void doRun() {
            exitRequested = false;
            while (!exitRequested) {
                print("§1:>§7 ");
                inputAnalize(Console.In.ReadLine());
            }
        }

        private char[] inputDelimiters = new char[] { ' ' };

        private void inputAnalize(string line) {
            string[] pieces = line.ToLower().Split(inputDelimiters);
            switch (pieces[0]) {
                case "generate":
                    generateDungeon(pieces);
                    break;
            }
        }

        private void generateDungeon(string[] pieces) {
            if (pieces.Length < 2) {
                println("Uso: §3generate§r {§4width height§r | §4demo§r}");
                return;
            }

            if (pieces[1] == "demo") {
                println("Generating a demo dungeon");
                generateDungeon(100, 100, 1, 10, true);
                return;
            }
        }



        private bool intersects(Room a, Room b) {
            Rectangle ra[] = a.getCollisionBox();
        }

        public int init() {
            println("§6Init...§r");
            doRun();
            return 0;
        }

        static int Main(string[] args) {
            Program a = new Program();
            return a.init();
        }
    }

    struct Rectangle {
        public int x, y, w, h;

        public Rectangle(int x, int y, int w, int h) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }
    }
}
