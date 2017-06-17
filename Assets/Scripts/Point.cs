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

    public static int mod(int x, int y) {
        return ((x % y) + y) % y;
    }


    public static int approxDistanceToPoint(Point p1, Point p2) {
        return Math.Abs(p1.getX() - p2.getX()) + Math.Abs(p1.getY() - p2.getY());
    }

    public static int exactDistanceToPoint(Point p1, Point p2) {
        return (int)(Math.Sqrt(Math.Pow((p1.getX() - p2.getX()), 2) + Math.Pow((p1.getY() - p2.getY()), 2)));
    }
}

