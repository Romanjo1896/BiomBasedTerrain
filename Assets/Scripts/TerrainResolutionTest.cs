using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainResolutionTest {
    public Terrain myTerrain;
    private static float[,] heights;

    public TerrainResolutionTest() {
        GameObject TerrainObj = new GameObject("TerrainObj");
        TerrainData _TerrainData = new TerrainData();
        GameObject terrainGo = GameObject.Find("TerrainObj");
        Object.DestroyImmediate(terrainGo);
        terrainGo = createTerrain(1, 1);
        myTerrain = terrainGo.GetComponent<Terrain>();

        _TerrainData.size = new Vector3(15f, 600f, 15f);
        _TerrainData.heightmapResolution = 15;
        _TerrainData.baseMapResolution = 1028;
        _TerrainData.SetDetailResolution(1028, 8);




        int heightmapWidth = myTerrain.terrainData.heightmapWidth;
        int heightmapHeight = myTerrain.terrainData.heightmapHeight;
        heights = myTerrain.terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

        heights[7, 7] = 1;
        heights[7, 8] = 1;
        heights[8, 7] = 2;
        heights[8, 8] = 2;
        myTerrain.terrainData.SetHeights(0, 0, heights);
    }

    private GameObject createTerrain(int sizeX, int sizeY) {
        GameObject TerrainObj = new GameObject("TerrainObj");
        TerrainData _TerrainData = new TerrainData();
        //_TerrainData.size = new Vector3(1500f/12, 600f, 1500f/12);
        //_TerrainData.heightmapResolution = 513;
        //_TerrainData.baseMapResolution = 1024;
        //_TerrainData.SetDetailResolution(1024, 8);
        _TerrainData.size = new Vector3(25.0f * sizeX, 600f, 25.0f * sizeY);
        _TerrainData.heightmapResolution = 25;
        _TerrainData.baseMapResolution = 1028;
        _TerrainData.SetDetailResolution(1028, 8);
        //SplatPrototype[] textures = new SplatPrototype[2];
        //textures[0] = new SplatPrototype();
        //textures[0].texture = Settings.getFlatTexture() ;
        //textures[1] = new SplatPrototype();
        //textures[1].texture = Settings.getSteepTexture();
        //_TerrainData.splatPrototypes = textures;

        int _heightmapWidth = _TerrainData.heightmapWidth;
        int _heightmapHeight = _TerrainData.heightmapHeight;

        TerrainCollider _TerrainCollider = TerrainObj.AddComponent<TerrainCollider>();
        Terrain _Terrain2 = TerrainObj.AddComponent<Terrain>();

        _TerrainCollider.terrainData = _TerrainData;
        _Terrain2.terrainData = _TerrainData;
        return TerrainObj;
    }
}
