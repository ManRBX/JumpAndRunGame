using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{
    [System.Serializable]
    public class LevelButtonEntry
    {
        public string levelName;
        public Button button;
    }

    [Header("Normale Level")]
    public LevelButtonEntry[] levelEntries;

    [Header("Bonus-Level")]
    public LevelButtonEntry[] bonusEntries;

    private void Start()
    {
        UpdateLevelButtons(levelEntries);
        UpdateLevelButtons(bonusEntries);
    }

    private void UpdateLevelButtons(LevelButtonEntry[] entries)
    {
        for (int i = 0; i < entries.Length; i++)
        {
            string key = entries[i].levelName + "_Unlocked";
            bool keyExists = PlayerPrefs.HasKey(key);
            bool isUnlocked = keyExists && PlayerPrefs.GetInt(key) == 1;

            // Nur das erste Level ist immer freigeschaltet
            if (i == 0) isUnlocked = true;

            entries[i].button.interactable = true; // Immer interaktiv, Farbe regelt Anzeige

            // 🎨 Button-Hintergrundfarbe setzen
            ColorBlock colors = entries[i].button.colors;
            if (isUnlocked)
            {
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                colors.pressedColor = new Color(0.8f, 0.8f, 0.8f);
                colors.disabledColor = Color.gray;
            }
            else
            {
                colors.normalColor = Color.black;
                colors.highlightedColor = Color.black;
                colors.pressedColor = Color.black;
                colors.disabledColor = Color.black;
            }
            entries[i].button.colors = colors;

            // 📝 Text-Farbe setzen (Text Legacy)
            Text buttonText = entries[i].button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.color = isUnlocked ? Color.white : Color.black;
            }

            // Events nicht automatisch setzen – du nutzt TryLoadScene im OnClick!
        }
    }

    // 👉 Diese Methode kannst du im Button-OnClick eintragen!
    public void TryLoadScene(string levelName)
    {
        string key = levelName + "_Unlocked";
        bool isUnlocked = PlayerPrefs.HasKey(key) && PlayerPrefs.GetInt(key) == 1;

        if (isUnlocked || levelName == "Level01") // Level01 immer erlaubt
        {
            Debug.Log("✅ Lade Level: " + levelName);
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning("❌ Zugriff verweigert: " + levelName + " ist gesperrt.");
        }
    }
}
