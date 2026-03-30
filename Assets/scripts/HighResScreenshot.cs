using UnityEngine;
using System.IO;

public class HighResScreenshot : MonoBehaviour
{
    public int width = 1920;
    public int height = 622;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            string folder = Application.dataPath + "/Screenshots/";

            // Ordner erstellen wenn nicht vorhanden
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = "steam_banner_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            string path = folder + fileName;

            ScreenCapture.CaptureScreenshot(path);

            Debug.Log("🔥 Screenshot gespeichert: " + path);
        }
    }
}