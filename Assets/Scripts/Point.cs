using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
    private int x;
    private int y;

    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }

    public static int approxDistanceToPoint(Point p1, Point p2) {
        int abstand = (int)(Math.Sqrt(Math.Pow(Math.Abs(p1.getX() - p2.getX()), 2) + Math.Pow(Math.Abs(p1.getY() - p2.getY()), 2)));
        return abstand;
    }

}

