using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters {
    private static float[,] mountainHeights;
    private static float[,] mountainWides;

    public static void generateAllMaps(int xSize, int ySize) {
        mountainHeights = new float[xSize, ySize];
        mountainWides = new float[xSize, ySize];
        int octaves = BiomTerrainGenerator.getOctaves();
        float frequency = BiomTerrainGenerator.getFrequency();
        int verschiebungX = BiomTerrainGenerator.getVerschiebungX();
        int verschiebungY = BiomTerrainGenerator.getVerschiebungY();
        if (BiomTerrainGenerator.getBiomesActive()) {
            PerlinNoise.generatePerlinNoise(mountainHeights, frequency, octaves, verschiebungX, verschiebungY);
            PerlinNoise.generatePerlinNoise(mountainWides, frequency, octaves);
        } else {
            for (int i = 0;i < mountainHeights.GetLength(0);i++) {
                for (int j = 0;j < mountainHeights.GetLength(1);j++) {
                    //Problem das aus der Pusedo kette werte genommen werden!
                    mountainHeights[i, j] = RandomsBySeed.getNextRandom(8, 10) / 10.0f ;
                }
            }
            for (int i = 0;i < mountainWides.GetLength(0);i++) {
                for (int j = 0;j < mountainWides.GetLength(1);j++) {
                    mountainWides[i, j] = RandomsBySeed.getNextRandom(8, 10) / 10.0f;
                }
            }
        }
    }

    public static float[,] getMountainHeights() {
        return mountainHeights;
    }

    public static float[,] getMountainWides() {
        return mountainWides;
    }

}
