using System.IO;
using UnityEditor;

public class createAssetBundle
{

    [MenuItem("Assets/Build AssetBundle")]
    static void build()
    {
        string abd = "Assets/StreamingAssets";

        if(!Directory.Exists(abd)) Directory.CreateDirectory(abd);

        BuildPipeline.BuildAssetBundles(abd, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

}
