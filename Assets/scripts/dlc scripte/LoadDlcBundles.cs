using System.IO;
using UnityEngine;

public class LoadDlcBundles : MonoBehaviour
{
    private void Start()
    {
        string exeDir = Path.GetDirectoryName(Application.dataPath); // z. B. .../JumpVerse/
        string bundleDirectory = Path.Combine(exeDir, "StreamingAssets");

        if (!Directory.Exists(bundleDirectory))
        {
            Debug.LogWarning("📁 DLC StreamingAssets folder not found: " + bundleDirectory);
            return;
        }

        string[] bundleFiles = Directory.GetFiles(bundleDirectory);

        foreach (string filePath in bundleFiles)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".meta" || extension == ".manifest" || filePath.Contains(".DS_Store"))
                continue;

            Debug.Log("📦 Loading DLC AssetBundle: " + Path.GetFileName(filePath));

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
                Debug.Log("✅ Instantiated DLC Asset: " + obj.name);
            }

            bundle.Unload(false);
        }
    }
}
