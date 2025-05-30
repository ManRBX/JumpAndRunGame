using System.IO;
using UnityEngine;

public class LoadFromFile : MonoBehaviour
{
    private void Start()
    {
        string bundleDirectory = Application.streamingAssetsPath;

        if (!Directory.Exists(bundleDirectory))
        {
            Debug.LogWarning("📁 StreamingAssets folder not found.");
            return;
        }

        string[] bundleFiles = Directory.GetFiles(bundleDirectory);

        foreach (string filePath in bundleFiles)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            // ❌ Skip all unwanted files
            if (extension == ".meta" || extension == ".manifest" || filePath.Contains(".DS_Store"))
                continue;

            Debug.Log("📦 Loading AssetBundle: " + Path.GetFileName(filePath));

            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            if (bundle == null)
            {
                Debug.LogError("❌ Failed to load AssetBundle: " + filePath);
                continue;
            }

            GameObject[] loadedAssets = bundle.LoadAllAssets<GameObject>();
            foreach (var obj in loadedAssets)
            {
                Instantiate(obj);
                Debug.Log("✅ Instantiated: " + obj.name);
            }

            bundle.Unload(false);
        }
    }
}
