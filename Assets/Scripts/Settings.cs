using System;
using UnityEditor;
using UnityEngine;

public class Settings : MonoBehaviour {
    public static Texture2D flat;
    public static Texture2D steep;
    static bool didIt = false;

    public static Texture2D getSteepTexture() {
        if (steep == null) {
            string path = "/Assets/Textures/CliffAlbedoSpecular.psd";
            steep = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        }
        if (!didIt) {
            Debug.Log(steep);
        }
        return steep;
    }

    public static Texture2D getFlatTexture() {
        if (flat == null) {
            string path = "/Assets/Textures/GrassHillAlbedo.psd";
            flat = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
        }
        return flat;
    }
}
