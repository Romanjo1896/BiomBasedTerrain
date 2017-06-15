using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MountainAgent {
    private float[,] heights;
    private int tokens;
    private Point startingPoint;
    private Point directionPoint;
    private const float MAX_HEIGHT = 80.0f;
    private const int MAX_WIDTH = 50;
    private Stopwatch stopWatch;

    //performance bei etwa 1min
    public MountainAgent(int tokens, Point startingPoint) {
        stopWatch = new Stopwatch();
        this.startingPoint = startingPoint;
        this.tokens = tokens;
        heights = TerrainGenerator.getTerrainData();
        directionPoint = RandomsBySeed.getNextDirectionPoint(heights.GetLength(0) - 1, heights.GetLength(1) - 1);
        walk();
    }

    public System.TimeSpan getElapsedTime() {
        return stopWatch.Elapsed;
    }

    private void walk() {
        stopWatch.Start();
        Point curLocation = startingPoint;
        while (tokens > 0) {
            Point[] neighbours = getAllNeighbours(curLocation).ToArray();
            if (neighbours.Length < 4) {
                return;
            }
            foreach (Point n in neighbours) {
                int[] scores = new int[neighbours.Length];
                for (int i = 0;i < neighbours.Length;i++) {
                    scores[i] = Point.approxDistanceToPoint(neighbours[i], directionPoint) * RandomsBySeed.getNextRandom(8, 10);
                }
                int max = Mathf.Max(scores);
                for (int i = 0;i < scores.Length;i++) {
                    if (max == scores[i]) {
                        //Set new starting Point as where the agents moves to
                        curLocation = neighbours[i];
                        raiseTerrain(curLocation);
                        tokens--;
                        break;
                    }
                }
            }
        }
        TerrainGenerator.updateHeights(heights);
        stopWatch.Stop();
    }

    //Bottleneck!
    private void raiseTerrain(Point p) {
        int wide = (int)(getMaxWidth() * RandomsBySeed.getNextRandom(8, 10) / 10.0);
        List<Point> candidates = new List<Point>();
        for (int x = p.getX() - wide / 2;x < p.getX() + wide / 2;x++) {
            if (x > 0 && x < heights.GetLength(0)) {
                for (int y = p.getY() - wide / 2;y < p.getY() + wide / 2;y++) {
                    if (y > 0 && y < heights.GetLength(1)) {
                        candidates.Add(new Point(x, y));
                    }
                }
            }
        }

        //sehr teuer!
        float abstand;
        float h;
        foreach (Point c in candidates) {
            //abstand etwa 1/4 der Zeit
            abstand = Mathf.Sqrt(Mathf.Pow(c.getX() - p.getX(), 2) + Mathf.Pow(c.getY() - p.getY(), 2));
            h = getMaxHeight() - getMaxHeight() / getMaxWidth() * abstand;
            //heights += auch sehr teuer
            // /(0.25*wide*wide) da ansonsten zu hoch, wegen mehrfacherfühung
            heights[c.getX(), c.getY()] += h / (0.25f * wide * wide);
        }
        //ab sehr teuer 50% der kosten des gesamten Prozesses

    }
    //sehr schnell, insgesamt weniger als 1s 
    List<Point> getAllNeighbours(Point p) {
        int x = p.getX();
        int y = p.getY();
        List<Point> nachbarn = new List<Point>();
        if (x > 0) {
            nachbarn.Add(new Point(x - 1, y));
        }
        if (x < heights.GetLength(0) - 2) {
            nachbarn.Add(new Point(x + 1, y));
        }
        if (y > 0) {
            nachbarn.Add(new Point(x, y - 1));
        }
        if (y < heights.GetLength(1) - 2) {
            nachbarn.Add(new Point(x, y + 1));
        }
        return nachbarn;
    }

    private int getMaxWidth() {
        return MAX_WIDTH;
    }

    private float getMaxHeight() {
        return MAX_HEIGHT;
    }
}
