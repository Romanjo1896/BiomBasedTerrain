using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class BiomTerrainGenerator : EditorWindow {

    private bool coastLineAgent = false;
    private int mountainAgents = 10;

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
        _TerrainData.size = new Vector3(1500f , 600f, 1500f);
        _TerrainData.heightmapResolution = 1500;
        _TerrainData.baseMapResolution = 1500;
        _TerrainData.SetDetailResolution(1500, 8);
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
        return TerrainObj; // Terrain.CreateTerrainGameObject(_TerrainData);
    }

    private void OnGUI() {
        GUILayout.Label("Biom Terrain Generator", EditorStyles.boldLabel);

        mountainAgents = EditorGUILayout.IntField("Number Mountain agents", mountainAgents);
        coastLineAgent = GUILayout.Toggle(coastLineAgent, "Coast line");
        if (GUILayout.Button("Create")) {
            Debug.Log("Creating terrain ...");
            RandomsBySeed.reset();
            TerrainGenerator tg1 = new TerrainGenerator();
            /*GameObject terrainGo = createTerrain();
            tg1.myTerrain = terrainGo.GetComponent<Terrain>();*/

            GameObject terrainGo = GameObject.Find("TerrainObj");
            if (terrainGo != null) {
                DestroyImmediate(terrainGo);
            }
            terrainGo = createTerrain();
            tg1.myTerrain = terrainGo.GetComponent<Terrain>();


            tg1.Start(coastLineAgent,mountainAgents);
            applyTextures(tg1.myTerrain.terrainData);
        }
    }

    //
    private void applyTextures(TerrainData terrainData) {

        //Texture2D texStreet = Resources.Load("Cliff") as Texture2D;
        var flatSplat = new SplatPrototype();
        var steepSplat = new SplatPrototype();

        //Texture2D grass = Resources.Load("Assets/Textures/Grass") as Texture2D;
        Texture2D grass = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/Grass.psd", typeof(Texture2D));
        Texture2D cliff = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/Cliff.psd", typeof(Texture2D));

        //  Texture2D cliff = Resources.Load("Assets/Textures/Cliff") as Texture2D;

        flatSplat.texture = grass;
        steepSplat.texture = cliff;
        SplatPrototype[] splats = { flatSplat, steepSplat };
        terrainData.splatPrototypes = splats;
        //terrainData.splatPrototypes = new SplatPrototype[2];
        //terrainData.splatPrototypes.SetValue(flatSplat, 0);
        //terrainData.splatPrototypes.SetValue(steepSplat, 1);


        terrainData.RefreshPrototypes();
        var splatMap = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 2];
        for (var zRes = 0;zRes < terrainData.alphamapHeight;zRes++) {
            for (var xRes = 0;xRes < terrainData.alphamapWidth;xRes++) {
                var normalizedX = (float)xRes / (terrainData.alphamapWidth - 1);
                var normalizedZ = (float)zRes / (terrainData.alphamapHeight - 1);
                var steepness = terrainData.GetSteepness(normalizedX, normalizedZ);
                //var steepnessNormalized = Mathf.Clamp(steepness / 1.5f, 0, 1.0f);
                var steepnessNormalized = steepness / 90.0f;
                splatMap[zRes, xRes, 0] = 1f - steepnessNormalized;
                splatMap[zRes, xRes, 1] = steepnessNormalized;
            }
        }
        terrainData.SetAlphamaps(0, 0, splatMap);

    }
}
