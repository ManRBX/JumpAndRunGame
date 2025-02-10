using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Szene über Button-Click laden
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Prüfen, ob die Szene im Build existiert
            if (SceneExists(sceneName))
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log("Szene geladen: " + sceneName);
            }
            else
            {
                Debug.LogError("Szene '" + sceneName + "' ist nicht im Build Settings hinzugefügt!");
            }
        }
        else
        {
            Debug.LogWarning("Kein Szenenname angegeben!");
        }
    }

    // Szene überprüfen, ob sie im Build Settings vorhanden ist
    private bool SceneExists(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneInBuild == sceneName)
            {
                return true;
            }
        }

        Debug.LogWarning("Szene '" + sceneName + "' nicht gefunden.");
        return false;
    }

    // Debugging: Liste aller Szenen im Build ausgeben
    void Start()
    {
        Debug.Log("Szenen im Build:");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log("Scene: " + sceneInBuild);
        }
    }
}
