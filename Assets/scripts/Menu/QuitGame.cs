using UnityEngine;

public class QuitGame : MonoBehaviour
{
    /// <summary>
    /// Method for the quit button.
    /// </summary>
    public void Quit()
    {
        Debug.Log("Game is quitting...");

        // Editor mode: Stop the play session
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Build mode: Exit the game
        Application.Quit();
#endif
    }
}
