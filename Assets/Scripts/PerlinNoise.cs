using System;
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

    public static void generatePerlinNoise(float[,] map, float frequency, int oktaves, int verschiebungX, int verschiebungY) {
        verschiebungX = verschiebungX * 500;
        verschiebungY = verschiebungY * 500;

        float height = 1;
        xMax = (map.GetLength(0) - 1) * 100000;
        yMax = (map.GetLength(1) - 1) * 100000;
        for (int i = 0;i < map.GetLength(0);i++) {
            for (int j = 0;j < map.GetLength(1);j++) {
                map[i, j] = 0;
            }
        }
        float pi = Mathf.PI;
        for (int o = 0;o < oktaves;o++) {
            for (int i = 0;i < map.GetLength(0);i++) {
                for (int j = 0;j < map.GetLength(1);j++) {
                    //     map[i, j] += height * Mathf.PerlinNoise(100000 * (i * frequency / xMax + verschiebungX / xMax), 100000 * (j * frequency / yMax + verschiebungY / yMax));
                    map[i, j] += height * Mathf.PerlinNoise((i * frequency / pi + verschiebungX) / (100 * pi), (j * frequency / pi + verschiebungY) / (100 * pi));
                }
            }
            height = height / 2;
            frequency = frequency * 2;
        }
        normalize(map);
    }

    public static void normalize(float[,] map) {
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
        for (int i = 0;i < map.GetLength(0);i++) {
            for (int j = 0;j < map.GetLength(1);j++) {
                map[i, j] = (map[i, j] - min) / (max - min);
            }
        }
        max = 0;
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
        Debug.Log("min=" + min + " max=" + max);
    }

}
