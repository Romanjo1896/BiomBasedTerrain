using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters {
    private static float[,] mountainHeights;
    private static float[,] mountainWides;

    public static void generateAllMaps(int xSize, int ySize) {
        mountainHeights = new float[xSize, ySize];
        mountainWides = new float[xSize, ySize];
        PerlinNoise.generatePerlinNoise(mountainHeights, 1, 3);
        PerlinNoise.generatePerlinNoise(mountainWides, 1, 3);
    }

    public static float[,] getMountainHeights() {
        return mountainHeights;
    }

    public static float[,] getMountainWides() {
        return mountainWides;
    }

}
