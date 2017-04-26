using System;
using System.Collections.Generic;
using UnityEngine;

public class CoastlineAgent {
    private float[,] heights;
    private int maxTokens = 1000;
    private int tokens;
    private Point repulsor;
    private Point attractor;
    private Point startingPoint;

    public CoastlineAgent(int tokens, Point startingPoint) {
        this.heights = TerrainGenerator.getTerrainData();
        this.tokens = tokens;
        this.startingPoint = startingPoint;
        checkForDivide();
        int rndX1 = RandomsBySeed.getNextRandom(0, heights.GetLength(0) - 1);
        int rndX2 = RandomsBySeed.getNextRandom(0, heights.GetLength(0) - 1);
        int rndY1 = RandomsBySeed.getNextRandom(0, heights.GetLength(1) - 1);
        int rndY2 = RandomsBySeed.getNextRandom(0, heights.GetLength(1) - 1);

        attractor = new Point(rndX1, rndY1);
        repulsor = new Point(rndX2, rndY2);
        move();

    }

    private void move() {
        List<Int32> winners = new List<int>();
        while (tokens > 1) {
            Point[] nachbarn = getNeighbours().ToArray();
            if (nachbarn.Length == 0) {
                Point[] possibles = getAllNeighbours().ToArray();
                startingPoint = possibles[RandomsBySeed.getNextRandom(0, possibles.Length - 1)];
            }
            else {
                int[] scores = new int[nachbarn.Length];
                for (int i = 0;i < nachbarn.Length;i++) {
                    int t1 = approxDistanceToPoint(nachbarn[i], repulsor);
                    int t2 = approxDistanceToPoint(nachbarn[i], attractor);
                    int t3 = distanceToMiddle(nachbarn[i]);
                    scores[i] = t1 - t2 - 3 * t3;
                }
                int max = Mathf.Max(scores);
                for (int i = 0;i < scores.Length;i++) {
                    if (max == scores[i]) {
                        //Set new starting Point as where the agents moves to
                        startingPoint = nachbarn[i];
                        heights[startingPoint.getX(), startingPoint.getY()] = 0.1f;
                        tokens--;
                        break;
                    }
                }
                TerrainGenerator.updateHeights(heights);
            }
        }

    }

    void checkForDivide() {
        if (tokens > maxTokens * 1.4) {
            CoastlineAgent c = new CoastlineAgent(tokens / 2, startingPoint);
            tokens = tokens / 2;
            checkForDivide();
        }
    }

    
     // Returns ALL 8 Neighbours
    List<Point> getAllNeighbours() {
        int x = startingPoint.getX();
        int y = startingPoint.getY();
        List<Point> nachbarn = new List<Point>();
        if (x > 0) {
            nachbarn.Add(new Point(x - 1, y));
            //if (y > 0) {
            //    nachbarn.Add(new Point(x - 1, y - 1));
            //}
        }
        if (x < heights.GetLength(0) - 2) {
            nachbarn.Add(new Point(x + 1, y));
            //if (y < heights.GetLength(1) - 2) {
            //    nachbarn.Add(new Point(x + 1, y + 1));
            //}
        }
        if (y > 0) {
            nachbarn.Add(new Point(x, y - 1));
            //if (x < heights.GetLength(0) - 2) {
            //    nachbarn.Add(new Point(x + 1, y - 1));
            //}
        }
        if (y < heights.GetLength(1) - 2) {
            nachbarn.Add(new Point(x, y + 1));
            //if (x > 0) {
            //    nachbarn.Add(new Point(x - 1, y + 1));
            //}
        }
        return nachbarn;
    }

    /**
     *  Returns all neighbours under sea level
     */
    List<Point> getNeighbours() {
        int x = startingPoint.getX();
        int y = startingPoint.getY();
        List<Point> nachbarn = new List<Point>();
        if (x > 0 && heights[x - 1, y] == 0) {
            nachbarn.Add(new Point(x - 1, y));
        }
        if (x < heights.GetLength(0) - 2 && heights[x + 1, y] == 0) {
            nachbarn.Add(new Point(x + 1, y));
        }
        if (y > 0 && heights[x, y - 1] == 0) {
            nachbarn.Add(new Point(x, y - 1));
        }
        if (y < heights.GetLength(1) - 2 && heights[x, y + 1] == 0) {
            nachbarn.Add(new Point(x, y + 1));
        }
        return nachbarn;
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