using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Performance gut, unter 1s
public class TerraformingAgent {
    private float[,] heights;
    private const float MAX_HEIGHT = 0.05f;
    private const int MAX_WIDTH = 70;

   
    public TerraformingAgent() {
        heights = TerrainGenerator.getTerrainData();
        performTerraforming();
    }

    private void performTerraforming() {
        List<Point> nachbarn;
        float sum;
        for (int i = 0;i < heights.GetLength(0);i++) {
            for (int j = 0;j < heights.GetLength(1);j++) {
                nachbarn = getAllNeighbours(i, j);
                sum = 0;
                foreach (Point item in nachbarn) {
                    sum = sum + heights[item.getX(), item.getY()];
                }
                sum = sum / nachbarn.Count;
                float dif = getVariation(i, j);
                sum = sum * RandomsBySeed.getFloat(1 - dif, 1 + dif);
                heights[i, j] = sum;
            }
        }

    }

    private float getVariation(int i, int j) {
        return 0.01f;
    }


    List<Point> getAllNeighbours(int x, int y) {
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
}
