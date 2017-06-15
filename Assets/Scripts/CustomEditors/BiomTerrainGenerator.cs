using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class BiomTerrainGenerator : EditorWindow {

    private bool coastLineAgent = false;
    private int mountainAgents = 10;
    private bool terrainSquare = true;
    private int sizeX = 5;
    private int sizeY = 5;

    [MenuItem("Window/Terrain/BiomTerrainGenerator")]
    public static void ShowWindow() {
        EditorWindow wnd = EditorWindow.GetWindow(typeof(BiomTerrainGenerator));
        wnd.titleContent = new GUIContent("Biom Terrain Generator");
    }

    private GameObject createTerrain() {
        GameObject TerrainObj = new GameObject("TerrainObj");
        TerrainData _TerrainData = new TerrainData();
        //_TerrainData.size = new Vector3(1500f/12, 600f, 1500f/12);
        //_TerrainData.heightmapResolution = 513;
        //_TerrainData.baseMapResolution = 1024;
        //_TerrainData.SetDetailResolution(1024, 8);
        _TerrainData.size = new Vector3(25.0f * sizeX, 600f, 25.0f * sizeY);
        int res = 2;
        while (res + 1 < 4* 25 * Math.Max(sizeX, sizeY)) {
            res = res * 2;
        }
        //grater than 4097 not possible
        res = Math.Min(4097, res + 1);
        
        _TerrainData.heightmapResolution = res;
        _TerrainData.baseMapResolution = 1024;
        _TerrainData.SetDetailResolution(1024, 8);

        int _heightmapWidth = _TerrainData.heightmapWidth;
        int _heightmapHeight = _TerrainData.heightmapHeight;

        TerrainCollider _TerrainCollider = TerrainObj.AddComponent<TerrainCollider>();
        Terrain _Terrain2 = TerrainObj.AddComponent<Terrain>();

        _TerrainCollider.terrainData = _TerrainData;
        _Terrain2.terrainData = _TerrainData;
        return TerrainObj;
    }

    private void OnGUI() {
        GUILayout.Label("Biom Terrain Generator", EditorStyles.boldLabel);

        mountainAgents = EditorGUILayout.IntField("Number Mountain agents", mountainAgents);
        coastLineAgent = GUILayout.Toggle(coastLineAgent, "Coast line");
        sizeX = EditorGUILayout.IntField("X factor size", sizeX);
        if (GUILayout.Toggle(terrainSquare, "Terrain as a square")) {
            terrainSquare = true;
            sizeY = sizeX;
        } else {
            terrainSquare = false;
            sizeY = EditorGUILayout.IntField("Y factor size", sizeY);
        }
        if (GUILayout.Button("Create")) {
            Debug.Log("Creating terrain ...");
            RandomsBySeed.reset();
            TerrainGenerator tg1 = new TerrainGenerator();
            GameObject terrainGo = GameObject.Find("TerrainObj");
            DestroyImmediate(terrainGo);
            terrainGo = createTerrain();
            tg1.myTerrain = terrainGo.GetComponent<Terrain>();


            tg1.Start(coastLineAgent, mountainAgents);
            applyTextures(tg1.myTerrain.terrainData);
        }
        //test purposes, remove later
        if (GUILayout.Button("ResTest")) {
    
            TerrainResolutionTest trt = new TerrainResolutionTest();
            trt.start();
        }
    }

    private void applyTextures(TerrainData terrainData) {

        var flatSplat = new SplatPrototype();
        var steepSplat = new SplatPrototype();

        Texture2D grass = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/Grass.psd", typeof(Texture2D));
        Texture2D cliff = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/Cliff.psd", typeof(Texture2D));

        flatSplat.texture = grass;
        steepSplat.texture = cliff;
        SplatPrototype[] splats = { flatSplat, steepSplat };
        terrainData.splatPrototypes = splats;

        terrainData.RefreshPrototypes();
        var splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 2];
        for (var zRes = 0;zRes < terrainData.alphamapHeight;zRes++) {
            for (var xRes = 0;xRes < terrainData.alphamapWidth;xRes++) {
                var normalizedX = (float)xRes / (terrainData.alphamapWidth - 1);
                var normalizedZ = (float)zRes / (terrainData.alphamapHeight - 1);
                var steepness = terrainData.GetSteepness(normalizedX, normalizedZ);
                var steepnessNormalized = steepness / 90.0f;
                splatMap[zRes, xRes, 0] = 1f - steepnessNormalized;
                splatMap[zRes, xRes, 1] = steepnessNormalized;
            }
        }
        terrainData.SetAlphamaps(0, 0, splatMap);

    }
}
