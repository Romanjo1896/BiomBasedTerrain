using UnityEditor;
using UnityEngine;
using System;

public class BiomTerrainGenerator : EditorWindow {

    private bool coastLineAgent = false;
    private int mountainAgents = 10;
    private bool terrainSquare = true;


    private int sizeX = 5;
    private int sizeY = 5;
    private static float frequency = 1;
    private static int octaves = 4;
    private bool terraforming = true;
    private static int verschiebungX = 1;
    private static int verschiebungY = 1;
    private static bool biomesActive = true;
    private static int blockSize = 50;
    private static double rating = -1;
    private static string name = "TerrainObj";


    [MenuItem("Window/Terrain/BiomTerrainGenerator")]
    public static void ShowWindow() {
        EditorWindow wnd = EditorWindow.GetWindow(typeof(BiomTerrainGenerator));
        wnd.titleContent = new GUIContent("Biom Terrain Generator");
    }

    internal static bool getBiomesActive() {
        return biomesActive;
    }

    private GameObject createTerrain() {
        GameObject TerrainObj = new GameObject("TerrainObj");
        TerrainData _TerrainData = new TerrainData();
        //_TerrainData.size = new Vector3(1500f/12, 600f, 1500f/12);
        //_TerrainData.heightmapResolution = 513;
        //_TerrainData.baseMapResolution = 1024;
        //_TerrainData.SetDetailResolution(1024, 8);
        _TerrainData.size = new Vector3(25.0f * sizeX, 600f, 25.0f * sizeY);
        Debug.Log("Size: " + _TerrainData.size);
        int res = 2;
        while (res + 1 < 4 * 25 * Math.Max(sizeX, sizeY)) {
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
        terraforming = GUILayout.Toggle(terraforming, "terraforming");
        biomesActive = GUILayout.Toggle(biomesActive, "Biomes active");
        sizeX = EditorGUILayout.IntField("X factor size", sizeX);
        octaves = EditorGUILayout.IntField("octaves", octaves);
        frequency = EditorGUILayout.FloatField("frequency", frequency);
        verschiebungX = EditorGUILayout.IntField("verschiebungX", verschiebungX);
        verschiebungY = EditorGUILayout.IntField("verschiebungY", verschiebungY);
        name = EditorGUILayout.TextField(name);
        blockSize = EditorGUILayout.IntField("blockSize", blockSize);
        rating = EditorGUILayout.DoubleField("Rating", rating);


        if (GUILayout.Button("Rate Terrain")) {
            GameObject terrainGo = GameObject.Find(name);
            Terrain myTerrain = terrainGo.GetComponent<Terrain>();

            int heightmapWidth = myTerrain.terrainData.heightmapWidth;
            int heightmapHeight = myTerrain.terrainData.heightmapHeight;

            float[,] map = myTerrain.terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
            rating = Rating.rateTerrain(map);
        }

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
            tg1.Start(coastLineAgent, mountainAgents, terraforming);
            applyTextures(tg1.myTerrain.terrainData);
        }
        if (GUILayout.Button("ParameterPerlin Test")) {
            RandomsBySeed.reset();
            GameObject terrainGo = GameObject.Find("TerrainObj");
            DestroyImmediate(terrainGo);
            terrainGo = createTerrain();

            Terrain myTerrain = terrainGo.GetComponent<Terrain>();

            int heightmapWidth = myTerrain.terrainData.heightmapWidth;
            int heightmapHeight = myTerrain.terrainData.heightmapHeight;

            float[,] mapMaxHeight = myTerrain.terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);

            PerlinNoise.generatePerlinNoise(mapMaxHeight, frequency, octaves, verschiebungX, verschiebungY);
            myTerrain.terrainData.SetHeights(0, 0, mapMaxHeight);
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

    public static int getVerschiebungX() {
        return verschiebungX;
    }

    public static int getVerschiebungY() {
        return verschiebungY;
    }

    public static int getOctaves() {
        return octaves;
    }

    public static float getFrequency() {
        return frequency;
    }

    internal static int getBlockSize() {
        return blockSize;
    }

}
