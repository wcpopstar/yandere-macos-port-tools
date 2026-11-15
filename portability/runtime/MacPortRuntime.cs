using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;

public static class MacPortPaths
{
    private static string dataPath;
    public static string DataPath
    {
        get
        {
            if (dataPath != null) return dataPath;
            string destination = Path.Combine(Application.persistentDataPath, "StreamingAssets");
            string marker = Path.Combine(destination, ".mac-port-seeded-v1");
            if (!File.Exists(marker))
            {
                Directory.CreateDirectory(destination);
                string source = Application.streamingAssetsPath;
                foreach (string directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(Path.Combine(destination, directory.Substring(source.Length + 1)));
                foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                {
                    string target = Path.Combine(destination, file.Substring(source.Length + 1));
                    if (!File.Exists(target)) File.Copy(file, target);
                }
                File.WriteAllText(marker, "Seeded from the recovered build; existing user files are preserved.");
            }
            dataPath = destination;
            return dataPath;
        }
    }
    public static string StreamingAssetsUrl => new Uri(DataPath).AbsoluteUri;
}

// Temporary compatibility mode until the recovered post-processing shaders are restored.
public sealed class MacPortRuntime : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        var obj = new GameObject("Mac port compatibility");
        DontDestroyOnLoad(obj);
        obj.AddComponent<MacPortRuntime>();
        SceneManager.sceneLoaded += (scene, mode) => Debug.Log("MAC_PORT_SCENE: " + scene.name);
    }
    private readonly Dictionary<Type, bool> screenEffects = new Dictionary<Type, bool>();
    private void LateUpdate()
    {
        foreach (var camera in Camera.allCameras)
        {
            foreach (var effect in camera.GetComponents<MonoBehaviour>())
            {
                if (effect == null || !effect.enabled) continue;
                var type = effect.GetType();
                if (!screenEffects.TryGetValue(type, out bool isScreenEffect))
                {
                    isScreenEffect = type.GetMethod("OnRenderImage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) != null;
                    screenEffects[type] = isScreenEffect;
                }
                if (isScreenEffect) effect.enabled = false;
            }
        }
    }
}
