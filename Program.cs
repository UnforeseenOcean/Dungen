using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator {
    /**
     * Holds all the dungeon information.
     */
    class Dungeon {

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

        public Dungeon(int width, int height, int floors, int chestCount, int roomsNumber, bool allowLockedDoors) {
            print(Heading.Info, "Init new dungeon. Parameters:\n Width: {0}\n Height: {1}\n Floors: {2}\n Chests: {3}\n Rooms: {4}\n Locked doors: {5}\n",
                new object[] {
                    width, height, floors, chestCount, roomsNumber, allowLockedDoors ? "true" : "false"
                });
        }

        /**
         * The plan is: Generate roomsNumber rooms, outshift them and then connect with corridors, adding locked doors and making
         * sure that the it's possible to reach every room.
         * And I don't fukin' know how to do all of that.
         * 
         * Okay, let's start by creating the Dungeon itself. This object will contain all the map data, that is, the map itself as well as
         * any object metadata (for monster spawners, mechanisms, chest contents, and so on.
         * 
         * Each object that has metadata will hava a unique ID, as the metadata for said object will be stored separetely, in JSON format.
         * The map itself won't retain the Room information, and will be turned into a tileset, or so it will be for DunGen 1.0 file specification.
         * 
         * Also, this comments will be very helpful, as I'm most likely going to forget everything I just said, and will come back very
         * often to remember what it is I have to implement next.
         */

        public void generate() {
            print(Heading.Attention, "Starting dungeon generation...", null);
        }
    }

    class Program {
        private static ConsoleColor[] CustomColors = new ConsoleColor[] { 
            ConsoleColor.Black, //0
            ConsoleColor.White, //1
            ConsoleColor.Red, //2
            ConsoleColor.Green, //3
            ConsoleColor.Blue, //4
            ConsoleColor.DarkBlue, //5
            ConsoleColor.DarkGreen, //6
            ConsoleColor.Gray, //7
            ConsoleColor.Yellow //8
        };
        private bool exitRequested;

        public static void print(string str) {
            printf(str, null);
        }

        public static void printf(string str, object[] p) {
            bool prevsim = false;
            bool prevCurly = false;
            bool curlyStart = false;
            string parambuffer = "";
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
                } else if (prevCurly) {
                    if (!curlyStart) {
                        parambuffer = "";
                        curlyStart = true;
                    }

                    if (c.Equals('}')) {
                        int k = int.Parse(parambuffer);
                        try {
                            print(p[k].ToString());
                        } catch (Exception e) {
                            continue;
                        }

                        curlyStart = false;
                        prevCurly = false;
                    } else if (c <= '9' && c >= '0') {
                        parambuffer += c;
                    }


                } else {
                    if (c.Equals('§')) {
                        prevsim = true;
                    } else if (c.Equals('{')) {
                        prevCurly = true;
                        curlyStart = false;
                    } else {
                        Console.Out.Write(c);
                    }
                }
            }
        }

        public static void println(string str) {
            print(str);
            Console.Out.WriteLine();
        }

        public static int getCharValue(char c) {
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
                new Dungeon(100, 100, 1, 10, 10, true).generate();
                return;
            }
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
