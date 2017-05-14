using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Performance gut, unter 1s
public class TerraformingAgent {
    private float[,] heights;
    private const float MAX_HEIGHT = 0.04f;
    private const int MAX_WIDTH = 70;
    private float peakHeight;

    public TerraformingAgent() {
        heights = TerrainGenerator.getTerrainData();
    }

    public void performTerraforming() {
        List<Point> nachbarn;
        float sum;
        float[,] addHeight = new float[heights.GetLength(0), heights.GetLength(1)];
        float height = MAX_HEIGHT;
        float frequency = 3;

        peakHeight = 0.0f;
        for (int i = 0;i < heights.GetLength(0);i++) {
            for (int j = 0;j < heights.GetLength(1);j++) {
                if (heights[i, j] > peakHeight) {
                    peakHeight = heights[i, j];
                }
            }
        }

        for (int o = 0;o < 8;o++) {
            height = height / 2;
            frequency = frequency * 2;
            for (int i = 0;i < heights.GetLength(0);i++) {
                for (int j = 0;j < heights.GetLength(1);j++) {
                    nachbarn = getAllNeighbours(i, j);
                    sum = 0;
                    //foreach (Point item in nachbarn) {
                    //    sum = sum + heights[item.getX(), item.getY()];
                    //}
                    //sum = sum / nachbarn.Count;
                    //float variation = Mathf.PerlinNoise(((float)i / (float)heights.GetLength(0)) * 10.0f, ((float)j / (float)heights.GetLength(1)) * 10.0f);
                    //sum = sum + sum * 0.05f * variation;
                    //sum = sum + heights[i, j] * getVariation(i, j) * (Mathf.PerlinNoise(((float)i / (float)heights.GetLength(0)) * frequency, ((float)j / (float)heights.GetLength(1)) * frequency));
                    sum = sum + getVariation(i, j) * (Mathf.PerlinNoise(((float)i / (float)heights.GetLength(0)) * frequency, ((float)j / (float)heights.GetLength(1)) * frequency));
                    addHeight[i, j] += sum;
                }
            }
        }
        for (int i = 0;i < heights.GetLength(0);i++) {
            for (int j = 0;j < heights.GetLength(1);j++) {
                heights[i, j] += addHeight[i, j];
            }
        }

        TerrainGenerator.updateHeights(heights);
    }

    //0.05 is fitting
    private float getVariation(int i, int j) {
        float f = getVariationCoefficient(i, j) * (float)(0.5 * (1 + Math.Tanh(2*heights[i, j] / peakHeight)));
        return f;
    }

    private float getVariationCoefficient(int x, int y) {
        return 0.05f;
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
