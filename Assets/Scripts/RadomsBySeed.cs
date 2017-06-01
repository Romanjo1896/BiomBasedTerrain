using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomsBySeed {
    private static System.Random rand;

    public static int getNextRandom(int min, int max) {
        if (rand == null) {
            rand = new System.Random(21092015);
        }
        return rand.Next(min, max + 1);
    }

    public static void reset() {
        rand = null;
    }

    public static Point getNextRandomPoint(float[,] h) {
        int x = getNextRandom(0, h.GetLength(0) - 1);
        int y = getNextRandom(0, h.GetLength(1) - 1);
        return new Point(x, y);
    }

    public static float getFloat(float min, float max) {
        if (rand == null) {
            rand = new System.Random(21092015);
        }
        float erg = (float)(rand.NextDouble());
        erg = erg * (max - min) + min;
        return erg;
    }

    public static Point getNextDirectionPoint(int xMax, int yMax) {
        int rnd = getNextRandom(0, 2 * xMax + 2 * yMax);
        Point p;
        if (rnd < xMax) {
            p = new Point(rnd, 0);
        } else if (rnd < xMax + yMax) {
            p = new Point(xMax, rnd - xMax);
        } else if (rnd < 2 * xMax + yMax) {
            p = new Point(rnd - xMax - yMax, yMax);
        } else {
            p = new Point(0, rnd - 2 * xMax - yMax);
        }
        return p;
    }
}
