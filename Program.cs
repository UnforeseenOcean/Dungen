using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator {

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
                println("Uso: §3generate§r (§4width height§r) | (§4demo§r)");
                return;
            }

            if (pieces[1] == "demo") {
                println("Generating a demo dungeon");
                new Dungeon(100, 100, 1, 10, 10, true).generate();
                //new DungeonDisplayer(null);
                return;
            }
        }

        public int init() {
            println("§6Init...§r");
            doRun();
            //try {
            //    dorun();
            //} catch (exception e) {
            //    printf("§2fatal exception§r: §8{0}§r\n {1}\n\n§2program terminated.", new string[] { e.message, e.stacktrace });
            //    console.in.read();
            //}
            return 0;
        }

        static int Main(string[] args) {
            Program a = new Program();
            return a.init();
        }
    }
}
