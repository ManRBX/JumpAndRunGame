using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public string nextLevelName;  // Name des nächsten Levels (z.B. "Level02")
    public string currentLevelName;  // Aktuelles Level (z.B. "Level01")
    public string returnScene = "Menu";  // Szene, zu der zurückgekehrt wird

    private void Start()
    {
        // Level01 ist immer freigeschaltet
        if (currentLevelName == "Level01")
        {
            PlayerPrefs.SetInt(currentLevelName + "_Freigeschaltet", 1);
        }

        // Markiere das Level als besucht
        PlayerPrefs.SetInt(currentLevelName + "_Visited", 1);
        PlayerPrefs.Save();
        Debug.Log(currentLevelName + " betreten.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Speichere Level als abgeschlossen
            PlayerPrefs.SetInt(currentLevelName + "_Beendet", 1);
            Debug.Log(currentLevelName + " beendet!");

            // Nächstes Level freischalten (außer es ist das letzte Level)
            if (!string.IsNullOrEmpty(nextLevelName))
            {
                PlayerPrefs.SetInt(nextLevelName + "_Freigeschaltet", 1);
                Debug.Log(nextLevelName + " freigeschaltet!");
            }

            PlayerPrefs.Save();

            // Zurück ins Menü
            SceneManager.LoadScene(returnScene);
        }
    }
}
