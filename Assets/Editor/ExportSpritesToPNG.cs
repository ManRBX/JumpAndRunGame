using UnityEngine;
using UnityEditor;
using System.IO;

public class ExportSpritesToPNG
{
    [MenuItem("Tools/Export Sprites To PNG")]
    public static void Export()
    {
        string folder = "Assets/ExportedSprites/";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        Object[] selected = Selection.objects;

        int count = 0;

        foreach (Object obj in selected)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (Object asset in assets)
            {
                if (asset is Sprite sprite)
                {
                    Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

                    Color[] pixels = sprite.texture.GetPixels(
                        (int)sprite.rect.x,
                        (int)sprite.rect.y,
                        (int)sprite.rect.width,
                        (int)sprite.rect.height);

                    tex.SetPixels(pixels);
                    tex.Apply();

                    byte[] bytes = tex.EncodeToPNG();
                    File.WriteAllBytes(folder + sprite.name + ".png", bytes);

                    count++;
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("🔥 Export fertig! Anzahl: " + count);
    }
}