using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Threading;

public class TerrainGenerator {
    public Terrain myTerrain;
    private static float[,] heights;
    private const float baseHeight = 70.0f;

    // Use this for initialization
    public void Start(bool generateCoastLine, int mountainAgentCount, bool terraforming) {

        int heightmapWidth = myTerrain.terrainData.heightmapWidth;
        int heightmapHeight = myTerrain.terrainData.heightmapHeight;

        heights = myTerrain.terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        Stopwatch stopWatch = new Stopwatch();
        for (int i = 0;i < heights.GetLength(0);i++) {
            for (int j = 0;j < heights.GetLength(1);j++) {
                heights[i, j] = baseHeight;
            }
        }

        Parameters.generateAllMaps(heights.GetLength(0), heights.GetLength(1));

        if (generateCoastLine) {
            stopWatch.Start();
            CoastlineAgent c = new CoastlineAgent(12000, new Point(heightmapWidth / 2, heightmapHeight / 2));
            c.move();
            stopWatch.Stop();
            printTime(stopWatch.Elapsed, "Coastline");
        }

        TimeSpan ts = new TimeSpan();
        for (int i = 0;i < mountainAgentCount;i++) {
            MountainAgent m = new MountainAgent(RandomsBySeed.getNextRandom(25000, 200000), RandomsBySeed.getNextRandomPoint(heights));
            ts = ts + m.getElapsedTime();
        }
        printTime(ts, "Mountains");
        if (terraforming) {
            TerraformingAgent tf = new TerraformingAgent();
            tf.changeTerrain();
            printTime(tf.getElapsedTime(), "Terraforming");
        }

        for (int i = 0;i < heights.GetLength(0);i++) {
            for (int j = 0;j < heights.GetLength(1);j++) {
                heights[i, j] = heights[i, j] / 600.0f;
            }
        }

        myTerrain.terrainData.SetHeights(0, 0, heights);
    }

    private void printTime(TimeSpan ts, String comp) {
        // Format and display the TimeSpan value.
        String elapsedTime = String.Format("{0:00}h {1:00}min {2:00},{3:00}s",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        UnityEngine.Debug.Log("RunTime " + comp + ": " + elapsedTime);
    }

    public static void updateHeights(float[,] newHeights) {
        heights = newHeights;
    }

    public static float[,] getTerrainData() {
        return heights;
    }
}
