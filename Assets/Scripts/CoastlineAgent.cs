using System;
using System.Collections.Generic;
using UnityEngine;

public class CoastlineAgent {
    private float[,] heights;
    private bool[,] raised;
    private int maxTokens = 1000;
    private int tokens;
    private Point repulsor;
    private Point attractor;
    private Point startingPoint;
    private int xMax;
    private int yMax;

    public CoastlineAgent(int tokens, Point startingPoint) {


        this.tokens = tokens;
        this.startingPoint = startingPoint;
        checkForDivide();
        this.heights = TerrainGenerator.getTerrainData();
        xMax = heights.GetLength(0) - 1;
        yMax = heights.GetLength(1) - 1;
        int rndX1 = RandomsBySeed.getNextRandom(0, xMax);
        int rndX2 = RandomsBySeed.getNextRandom(0, xMax);
        int rndY1 = RandomsBySeed.getNextRandom(0, yMax);
        int rndY2 = RandomsBySeed.getNextRandom(0, yMax);

        attractor = new Point(rndX1, rndY1);
        repulsor = new Point(rndX2, rndY2);

        raised = new bool[xMax + 1, yMax + 1];
        for (int i = 0;i < xMax + 1;i++) {
            for (int j = 0;j < yMax + 1;j++) {
                raised[i, j] = false;
            }
        }

    }

    public void move() {
        while (tokens > 0) {
            Point[] nachbarn = getNeighbours();
            if (nachbarn.Length == 0) {
                Point[] possibles = getAllNeighbours();
                startingPoint = possibles[RandomsBySeed.getNextRandom(0, possibles.Length - 1)];
            } else {
                int[] scores = new int[nachbarn.Length];
                for (int i = 0;i < nachbarn.Length;i++) {
                    int t1 = approxDistanceToPoint(nachbarn[i], repulsor);
                    int t2 = approxDistanceToPoint(nachbarn[i], attractor);
                    int t3 = -distanceToMiddle(nachbarn[i]);
                    scores[i] = t1 - t2 + 3 * t3;
                }
                int max = Mathf.Max(scores);
                for (int i = 0;i < scores.Length;i++) {
                    if (max == scores[i]) {
                        //Set new starting Point as where the agents moves to
                        raised[startingPoint.getX(), startingPoint.getY()] = true;
                        startingPoint = nachbarn[i];
                        tokens--;
                        break;
                    }
                }

            }
        }
        //Irgendwo gehen noch tokens verloren!
        int count = 0;
        for (int i = 0;i < heights.GetLength(0);i++) {
            for (int j = 0;j < heights.GetLength(1);j++) {
                if (raised[i, j]) {
                    heights[i, j] = 60.0f;
                    count++;
                }
            }
        }
        Debug.Log(count);
        TerrainGenerator.updateHeights(heights);
    }

    void checkForDivide() {
        if (tokens > maxTokens * 1.4) {
            CoastlineAgent c = new CoastlineAgent(tokens / 2, startingPoint);
            c.move();
            tokens = tokens / 2;
            checkForDivide();
        }
    }


    // Returns ALL 4 Neighbours
    Point[] getAllNeighbours() {
        int x = startingPoint.getX();
        int y = startingPoint.getY();
        Point[] nachbarn = new Point[4];
        nachbarn[0] = new Point(Point.mod(x - 1, xMax), y);
        nachbarn[1] = new Point(Point.mod(x + 1, xMax), y);
        nachbarn[2] = new Point(x, Point.mod(y - 1, yMax));
        nachbarn[3] = new Point(x, Point.mod(y + 1, yMax));
        return nachbarn;
    }

    /**
     *  Returns all neighbours under sea level
     */
    Point[] getNeighbours() {
        Point[] candidates = getAllNeighbours();
        List<Point> nachbarn = new List<Point>();
        for (int i = 0;i < 4;i++) {
            int candX = candidates[i].getX();
            int candY = candidates[i].getY();
            if (!raised[candX, candY]) {
                nachbarn.Add(candidates[i]);
            }
        }
        return nachbarn.ToArray();
    }

    int distanceToEdge(Point p) {
        int min1 = Math.Min(p.getX(), xMax - p.getX());
        int min2 = Math.Min(p.getY(), yMax - p.getY());
        return Math.Min(min1, min2);
    }

    int distanceToMiddle(Point p) {
        //int abstand = Mathf.Min(p.getX() + p.getY(), p.getY() + heights.GetLength(0) - p.getX(), p.getX() + heights.GetLength(1) - p.getY(), heights.GetLength(1) - p.getY() + heights.GetLength(0) - p.getX());
        Point middle = new Point(heights.GetLength(0) / 2, heights.GetLength(1) / 2);

        return approxDistanceToPoint(p, middle);
    }

    public static int approxDistanceToPoint(Point p1, Point p2) {
        int abstand = (int)(Math.Sqrt(Math.Pow(Math.Abs(p1.getX() - p2.getX()), 2) + Math.Pow(Math.Abs(p1.getY() - p2.getY()), 2)));
        return abstand;
    }
}