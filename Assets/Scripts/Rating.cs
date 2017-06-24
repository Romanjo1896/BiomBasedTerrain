using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rating {


    public static double rateTerrain(float[,] map) {
        int blockSize = BiomTerrainGenerator.getBlockSize();
        Block[,] blocks = new Block[map.GetLength(0) / blockSize, map.GetLength(1) / blockSize];
        for (int x = 0;x < map.GetLength(0) - blockSize;x = x + blockSize) {
            for (int y = 0;y < map.GetLength(1) - blockSize;y = y + blockSize) {
                blocks[x / blockSize, y / blockSize] = new Block(x, y, blockSize, map);
            }
        }


        double avMaxHeight = 0;
        float totalMax = -1;
        for (int i = 0;i < blocks.GetLength(0);i++) {
            for (int j = 0;j < blocks.GetLength(1);j++) {
                List<Block> nachbarn = new List<Block>();
                if (i > 0) {
                    nachbarn.Add(blocks[i - 1, j]);
                    if (j > 0) {
                        nachbarn.Add(blocks[i - 1, j - 1]);
                    }
                    if (j < blocks.GetLength(1) - 1) {
                        nachbarn.Add(blocks[i - 1, j + 1]);
                    }
                }
                if (j > 0) {
                    nachbarn.Add(blocks[i, j - 1]);
                }
                if (i < blocks.GetLength(0) - 1) {
                    nachbarn.Add(blocks[i + 1, j]);
                    if (j > 0) {
                        nachbarn.Add(blocks[i + 1, j - 1]);
                    }
                    if (j < blocks.GetLength(1) - 1) {
                        nachbarn.Add(blocks[i + 1, j + 1]);
                    }
                }
                if (j < blocks.GetLength(1) - 1) {
                    nachbarn.Add(blocks[i, j + 1]);
                }
                blocks[i, j].setNachbarn(nachbarn);
                avMaxHeight += blocks[i, j].getMax();
                if (blocks[i, j].getMax() > totalMax) {
                    totalMax = blocks[i, j].getMax();
                }
            }
        }
        avMaxHeight = avMaxHeight / (blocks.GetLength(0) * blocks.GetLength(1));
        Block.setAvgMaxHeight(avMaxHeight);
        Block.setTotalMax(totalMax);
        double score = 0;
        int count = 0;
        for (int i = 0;i < blocks.GetLength(0);i++) {
            for (int j = 0;j < blocks.GetLength(1);j++) {
                blocks[i, j].normalizeMax();
                blocks[i, j].calcBigBoxScore();
                blocks[i, j].calcSmallBoxScore();
                score = score + blocks[i, j].getBigScore() / blocks[i, j].getSmallScore();
                count++;
            }
        }
        return score / count;
    }
}

class Block {
    private int xStart, xEnd, yStart, yEnd, blockSize;
    private float[,] heights;
    private float maxHeight;
    private double smallScore, bigScore;
    private List<Block> nachbarn;
    private static double avgMaxHeight;
    private static float totalMax;

    public Block(int xStart, int yStart, int blockSize, float[,] heights) {
        this.blockSize = blockSize;

        this.xStart = xStart;
        this.yStart = yStart;

        xEnd = xStart + blockSize;
        yEnd = yStart + blockSize;

        this.heights = heights;
        calcMax();
    }

    public void normalizeMax() {
        maxHeight = maxHeight / totalMax;
    }

    public static void setAvgMaxHeight(double newAvgMaxHeight) {
        avgMaxHeight = newAvgMaxHeight;
    }

    public void calcBigBoxScore() {
        bigScore = Math.Abs(maxHeight - avgMaxHeight);
    }

    public void calcSmallBoxScore() {
        double sum = 0;
        foreach (Block b in nachbarn) {
            sum = sum + Math.Abs(b.getMax() - maxHeight);
        }
        sum = sum / nachbarn.Count;
        smallScore = sum;
    }

    private void calcMax() {
        maxHeight = float.MinValue;

        for (int i = xStart;i < xEnd;i++) {
            for (int j = yStart;j < yEnd;j++) {
                if (heights[i, j] > maxHeight) {
                    maxHeight = heights[i, j];
                }
            }
        }
    }


    public float getMax() {
        return maxHeight;
    }

    public double getSmallScore() {
        return smallScore;
    }

    public void setNachbarn(List<Block> nachbarn) {
        this.nachbarn = nachbarn;
    }

    public double getBigScore() {
        return bigScore;
    }

    internal static void setTotalMax(float newTotalMax) {
        totalMax = newTotalMax;
    }
}

