using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MountainAgent {
    private float[,] heights;
    private int tokens;
    private Point startingPoint;
    private Point repulsor;
    private const float MAX_HEIGHT = 80.0f;
    private const int MAX_WIDTH = 50;
    private Stopwatch stopWatch;
    private static float[,] mountainHeights;
    private static float[,] mountainWides;

    //performance bei etwa 1min
    public MountainAgent(int tokens, Point startingPoint) {
        stopWatch = new Stopwatch();
        this.startingPoint = startingPoint;
        this.tokens = tokens;
        heights = TerrainGenerator.getTerrainData();
        repulsor = RandomsBySeed.getNextDirectionPoint(heights.GetLength(0) - 1, heights.GetLength(1) - 1);
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
                    scores[i] = Point.exactDistanceToPoint(neighbours[i], repulsor) * RandomsBySeed.getNextRandom(8, 10);
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
        float pHoehe = getHeight(p.getX(), p.getY()) * MAX_HEIGHT;
        float pBreite = getWidth(p.getX(), p.getY()) * MAX_WIDTH;
        int wide = (int)(getWidth(p.getX(), p.getY()) * RandomsBySeed.getNextRandom(8, 10) / 10.0 * MAX_WIDTH);
        List<Point> candidates = new List<Point>();
        for (int x = p.getX() - (int)pBreite / 2;x < p.getX() + pBreite / 2;x++) {
            if (x > 0 && x < heights.GetLength(0)) {
                for (int y = p.getY() - (int)pBreite / 2;y < p.getY() + pBreite / 2;y++) {
                    if (y > 0 && y < heights.GetLength(1)) {
                        candidates.Add(new Point(x, y));
                    }
                }
            }
        }

        //sehr teuer!
        float abstand;
        float h;
        float add;

        foreach (Point c in candidates) {
            //abstand etwa 1/4 der Zeit
            //abstand = Mathf.Sqrt(Mathf.Pow(c.getX() - p.getX(), 2) + Mathf.Pow(c.getY() - p.getY(), 2));
            abstand = Mathf.Abs(c.getX() - p.getX()) + Mathf.Abs(c.getY() - p.getY());
            h = pHoehe - pHoehe / pBreite * abstand;
            //heights += auch sehr teuer
            // /(0.25*wide*wide) da ansonsten zu hoch, wegen mehrfacherfühung
            add = h / (0.25f * pBreite * pBreite) ;
            heights[c.getX(), c.getY()] += add;
            if (add < 0) {
                UnityEngine.Debug.Log("found it!!!!!!!!!!!!!!!!!");
                return;
            }
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

    private float getWidth(int x, int y) {
        if (mountainWides == null) {
            mountainWides = Parameters.getMountainWides();
        }
        return mountainWides[x, y];
    }

    private float getHeight(int x, int y) {
        if (mountainWides == null) {
            mountainWides = Parameters.getMountainWides();
        }
        return mountainWides[x, y];
    }
}
