﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Performance gut, unter 1s
public class TerraformingAgent {
    float[,] heights;
    private const float MAX_HEIGHT = 1; //0,2f war gut
    private int xMax, yMax;
    private const int OKTAVES = 12;
    private const float MAX_POSSIBLE_HEIGHT = 1.565f;
    private const float PERLIN_WEIGHT = 1.0f;
    private Stopwatch stopWatch;

    public TerraformingAgent() {
        heights = TerrainGenerator.getTerrainData();
        stopWatch = new Stopwatch();
    }

    public void changeTerrain() {
        stopWatch.Start();
        float[,] perlinFacors = new float[heights.GetLength(0), heights.GetLength(1)];
        float height = MAX_HEIGHT;
        float frequency = 4;
        xMax = heights.GetLength(0) - 1;
        yMax = heights.GetLength(1) - 1;
        for (int i = 0;i < perlinFacors.GetLength(0);i++) {
            for (int j = 0;j < perlinFacors.GetLength(1);j++) {
                perlinFacors[i, j] = 0;
            }
        }
        float pi = Mathf.PI;
        for (int o = 0;o < OKTAVES;o++) {
            for (int i = 0;i < perlinFacors.GetLength(0);i++) {
                for (int j = 0;j < perlinFacors.GetLength(1);j++) {
                    //float perlinAddition = height * (Mathf.PerlinNoise(i * frequency / xMax, j * frequency / yMax));
                   float perlinAddition = height * Mathf.PerlinNoise((i * frequency / pi) / (100 * pi), (j * frequency / pi) / (100 * pi));
                    perlinFacors[i, j] += perlinAddition;
                }
            }
            height = height / 2;
            frequency = frequency * 2;
        }
        for (int i = 0;i < heights.GetLength(0);i++) {
            for (int j = 0;j < heights.GetLength(1);j++) {
                perlinFacors[i, j] = perlinFacors[i, j] / MAX_POSSIBLE_HEIGHT * PERLIN_WEIGHT + 0.1f;
                heights[i, j] = heights[i, j] * (1.0f + perlinFacors[i, j]);
            }
        }
        float min = float.MaxValue;
        float minH = float.MaxValue;
        float max = float.MinValue;
        float maxH = float.MinValue;
        for (int i = 0;i < perlinFacors.GetLength(0);i++) {
            for (int j = 0;j < perlinFacors.GetLength(1);j++) {
                if (perlinFacors[i, j] < min) {
                    min = perlinFacors[i, j];
                }
                if (perlinFacors[i, j] > max) {
                    max = perlinFacors[i, j];
                }
                if (heights[i, j] < minH) {
                    minH = heights[i, j];
                }
                if (heights[i, j] > maxH) {
                    maxH = heights[i, j];
                }
            }
        }
        UnityEngine.Debug.Log("PerlinFactors: min=" + min + "   max=" + max);
        TerrainGenerator.updateHeights(heights);
        stopWatch.Stop();
    }
    private float getVariation(int i, int j) {
        float f = getVariationCoefficient(i, j) * heights[i, j] * (float)(0.5 * (1 + System.Math.Tanh(2 * i * j / (xMax * yMax))));
        return f;
    }

    private float getVariationCoefficient(int x, int y) {
        return 0.05f;
    }

    public TimeSpan getElapsedTime() {
        return stopWatch.Elapsed;
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
