using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For later purposes, not inbound yet
public class PerlinNoise {
    float[,] heights;
    private static int xMax, yMax;
    private const float MAX_POSSIBLE_HEIGHT = 1.565f;

    public static float[,] generatePerlinNoise(int x, int y, float frequency, int oktaves) {
        float[,] map = new float[x, y];
        generatePerlinNoise(map, frequency, oktaves);
        return map;
    }

    public static void generatePerlinNoise(float[,] map, float frequency, int oktaves) {
        float height = 1;
        xMax = map.GetLength(0) - 1;
        yMax = map.GetLength(1) - 1;
        for (int i = 0;i < map.GetLength(0);i++) {
            for (int j = 0;j < map.GetLength(1);j++) {
                map[i, j] = 0;
            }
        }
        for (int o = 0;o < oktaves;o++) {
            for (int i = 0;i < map.GetLength(0);i++) {
                for (int j = 0;j < map.GetLength(1);j++) {
                    map[i, j] += height * Mathf.PerlinNoise(i * frequency / xMax, j * frequency / yMax);
                }
            }
            height = height / 2;
            frequency = frequency * 2;
        }
        float min = float.MaxValue;
        float max = float.MinValue;
        for (int i = 0;i < map.GetLength(0);i++) {
            for (int j = 0;j < map.GetLength(1);j++) {
                if (map[i, j] < min) {
                    min = map[i, j];
                }
                if (map[i, j] > max) {
                    max = map[i, j];
                }
            }
        }
        Debug.Log("Perlin min=" + min + "  max=" + max);
        for (int i = 0;i < map.GetLength(0);i++) {
            for (int j = 0;j < map.GetLength(1);j++) {
                map[i, j] = (map[i, j] - min) / (max - min);
            }
        }

        //for (int i = 0;i < map.GetLength(0);i++) {
        //    for (int j = 0;j < map.GetLength(1);j++) {
        //        map[i, j] = map[i, j] / max;
        //    }
        //}

        for (int i = 0;i < map.GetLength(0);i++) {
            for (int j = 0;j < map.GetLength(1);j++) {
                if (map[i, j] < min) {
                    min = map[i, j];
                }
                if (map[i, j] > max) {
                    max = map[i, j];
                }
            }
        }
        Debug.Log("Perlin min=" + min + "  max=" + max);

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
