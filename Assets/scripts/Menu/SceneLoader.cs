using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads a scene via button click.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Check if the scene exists in the build settings
            if (SceneExists(sceneName))
            {
                SceneManager.LoadScene(sceneName);
                Debug.Log("Scene loaded: " + sceneName);
            }
            else
            {
                Debug.LogError("Scene '" + sceneName + "' is not added to the Build Settings!");
            }
        }
        else
        {
            Debug.LogWarning("No scene name provided!");
        }
    }

    /// <summary>
    /// Checks if the scene exists in the Build Settings.
    /// </summary>
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

        Debug.LogWarning("Scene '" + sceneName + "' not found.");
        return false;
    }

    /// <summary>
    /// Debugging: Outputs a list of all scenes included in the build.
    /// </summary>
    void Start()
    {
        Debug.Log("Scenes in Build:");
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log("Scene: " + sceneInBuild);
        }
    }
}
