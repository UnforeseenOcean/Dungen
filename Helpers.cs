
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGenerator {

    public struct Rectangle {
        public int x, y, w, h;

        public Rectangle(int x, int y, int w, int h) {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public Point getTopLeft() {
            return new Point(x, y);
        }

        public Point getBottomRight() {
            return new Point(x + w, y + h);
        }

        public override string ToString() {
            return "Rectangle: (" + x.ToString() + ", " + y.ToString() + ", " + w.ToString() + ", " + h.ToString() + ")";
        }
    }

    public struct Point {
        public int x, y;

        public Point(int x0, int y0) {
            x = x0;
            y = y0;
        }

        public override string ToString() {
            return "Point: (" + x.ToString() + ", " + y.ToString() + ")";
        }
    }

    abstract class Helpers {
        public static bool Contains(Rectangle a, Point p) {
            return (p.x >= a.x && p.x <= a.x + a.w && p.y >= a.y && p.y <= a.y + a.h);
        }

        public static bool Intersects(Rectangle a, Rectangle b) {
            return Contains(a, b.getTopLeft()) || Contains(a, b.getBottomRight());
        }

        public static double Distance(Rectangle r1, Rectangle r2) {
            return Math.Sqrt(Math.Pow(r1.x - r2.x, 2) + Math.Pow(r1.y - r2.y, 2));
        }

        public static double Distance(Point p1, Point p2) {
            return Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));
        }

        public static double Angle(Point centerR, Point centeOR) {
            return Math.Atan((centerR.y - centeOR.y) / (centerR.x - centeOR.x));
        }

        public static int[] IntegerProportion(double p1, double p2, int max) {
            double a1 = 0, a2 = 0;
            for (int i = 1; i < max; i++) {
                a1 = Math.Ceiling(i * p1);
                a2 = Math.Ceiling(i * p2);

                if (a1 - i * p1 <= 0.1 && a2 - i * p2 <= 0.1) {
                    break;
                }
            }

            return new int[] { (int) a1, (int) a2 };
        }
    }
}
