using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSystem : MonoBehaviour
{
    [System.Serializable]
    public class LevelButtonEntry
    {
        [Tooltip("Exakter Szenenname, z.B. Level01 oder LevelBonus01.")]
        public string levelName;

        [Tooltip("Button für dieses Level.")]
        public Button button;
    }

    [Header("🎮 Normale Level")]
    public LevelButtonEntry[] levelEntries;

    [Header("⭐ Bonus-Level")]
    public LevelButtonEntry[] bonusEntries;

    [Header("🎨 Farben")]
    public Color unlockedButtonColor = Color.white;
    public Color unlockedTextColor = Color.white;

    public Color lockedButtonColor = Color.black;
    public Color lockedTextColor = Color.black;

    [Header("⚙️ Einstellungen")]
    [Tooltip("Das erste normale Level ist immer freigeschaltet.")]
    public bool firstNormalLevelAlwaysUnlocked = true;

    private void Start()
    {
        UpdateNormalLevelButtons();
        UpdateBonusLevelButtons();
    }

    public void RefreshLevelButtons()
    {
        UpdateNormalLevelButtons();
        UpdateBonusLevelButtons();
    }

    private void UpdateNormalLevelButtons()
    {
        if (levelEntries == null)
            return;

        for (int i = 0; i < levelEntries.Length; i++)
        {
            LevelButtonEntry entry = levelEntries[i];

            if (entry == null || entry.button == null)
                continue;

            bool isUnlocked = IsLevelUnlocked(entry.levelName);

            // Nur erstes NORMALES Level immer frei
            if (i == 0 && firstNormalLevelAlwaysUnlocked)
            {
                isUnlocked = true;
            }

            UpdateButtonVisual(entry.button, isUnlocked);
        }
    }

    private void UpdateBonusLevelButtons()
    {
        if (bonusEntries == null)
            return;

        for (int i = 0; i < bonusEntries.Length; i++)
        {
            LevelButtonEntry entry = bonusEntries[i];

            if (entry == null || entry.button == null)
                continue;

            // Bonus-Level müssen wirklich freigeschaltet sein
            bool isUnlocked = IsLevelUnlocked(entry.levelName);

            UpdateButtonVisual(entry.button, isUnlocked);
        }
    }

    private bool IsLevelUnlocked(string levelName)
    {
        if (string.IsNullOrWhiteSpace(levelName))
            return false;

        string key = levelName + "_Unlocked";

        return PlayerPrefs.HasKey(key) &&
               PlayerPrefs.GetInt(key, 0) == 1;
    }

    private void UpdateButtonVisual(Button button, bool isUnlocked)
    {
        if (button == null)
            return;

        // Button bleibt klickbar.
        // TryLoadScene entscheidet, ob geladen werden darf.
        button.interactable = true;

        ColorBlock colors = button.colors;

        if (isUnlocked)
        {
            colors.normalColor = unlockedButtonColor;
            colors.highlightedColor = unlockedButtonColor;
            colors.pressedColor = new Color(
                unlockedButtonColor.r * 0.8f,
                unlockedButtonColor.g * 0.8f,
                unlockedButtonColor.b * 0.8f,
                unlockedButtonColor.a
            );

            colors.selectedColor = unlockedButtonColor;
            colors.disabledColor = Color.gray;
        }
        else
        {
            colors.normalColor = lockedButtonColor;
            colors.highlightedColor = lockedButtonColor;
            colors.pressedColor = lockedButtonColor;
            colors.selectedColor = lockedButtonColor;
            colors.disabledColor = lockedButtonColor;
        }

        button.colors = colors;

        // Normaler Unity UI Text
        Text buttonText = button.GetComponentInChildren<Text>(true);

        if (buttonText != null)
        {
            buttonText.color =
                isUnlocked
                    ? unlockedTextColor
                    : lockedTextColor;
        }
    }

    // Diese Methode beim Button OnClick verwenden.
    public void TryLoadScene(string levelName)
    {
        if (string.IsNullOrWhiteSpace(levelName))
        {
            Debug.LogWarning(
                "[LevelSystem] Kein Levelname angegeben."
            );

            return;
        }

        bool isUnlocked = IsLevelUnlocked(levelName);

        // Level01 immer erlauben
        if (firstNormalLevelAlwaysUnlocked &&
            levelName == "Level01")
        {
            isUnlocked = true;
        }

        if (!isUnlocked)
        {
            Debug.LogWarning(
                $"🔒 Zugriff verweigert: {levelName} ist gesperrt."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(levelName))
        {
            Debug.LogError(
                $"❌ Szene '{levelName}' ist nicht in den Build Settings."
            );

            return;
        }

        Debug.Log(
            $"✅ Lade Level: {levelName}"
        );

        SceneManager.LoadScene(levelName);
    }
}