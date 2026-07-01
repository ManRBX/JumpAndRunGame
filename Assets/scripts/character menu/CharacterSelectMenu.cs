using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterEntry
{
    [Header("🎭 Charakter Daten")]
    public int characterIndex;
    public string characterName;
    public bool unlockedByDefault = false;

    [Header("📝 Normaler Unity UI Text")]
    public Text characterText;
}

public class CharacterSelectMenu : MonoBehaviour
{
    [Header("💾 Speicher-Key")]
    public string selectedCharacterKey = "GlobalSelectedCharacter";
    public string unlockKeyPrefix = "CharacterUnlocked_";

    [Header("🧾 Anzeige")]
    public Text selectedCharacterText;

    [Header("💬 Texte")]
    public string selectedPrefix = "Ausgewählt: ";
    public string lockedText = "🔒 Gesperrt: ";
    public string unlockHintText = "Finde diesen Charakter im Level.";

    [Header("🎨 Farben")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.gray;
    public Color selectedColor = Color.green;

    [Header("▶ Auswahl-Markierung")]
    public string selectedMarker = "▶ ";

    [Header("🎭 Charaktere")]
    public CharacterEntry[] characters;

    private void Start()
    {
        EnsureDefaultUnlocks();
        RefreshUI();
        UpdateCharacterVisuals();
    }

    public void SelectCharacter(int characterIndex)
    {
        CharacterEntry character = GetCharacter(characterIndex);

        if (character == null)
        {
            Debug.LogWarning($"⚠️ Charakter Index {characterIndex} existiert nicht.");
            return;
        }

        if (!IsCharacterUnlocked(characterIndex))
        {
            if (selectedCharacterText != null)
            {
                selectedCharacterText.text =
                    lockedText + character.characterName + "\n" +
                    unlockHintText;
            }
            Debug.Log($"🔒 Charakter gesperrt: {character.characterName}");
            return;
        }

        // Auswahl speichern
        PlayerPrefs.SetInt(selectedCharacterKey, characterIndex);
        PlayerPrefsKeyTracker.TrackKey(selectedCharacterKey);

        // Namen des ausgewählten Characters speichern
        PlayerPrefs.SetString("GlobalSelectedCharacterName", character.characterName);
        PlayerPrefsKeyTracker.TrackKey("GlobalSelectedCharacterName");

        // Zeitstempel der letzten Auswahl speichern
        PlayerPrefs.SetString("GlobalSelectedCharacterTime", System.DateTime.Now.ToString());
        PlayerPrefsKeyTracker.TrackKey("GlobalSelectedCharacterTime");

        PlayerPrefs.Save();

        Debug.Log($"🎭 Charakter gewählt: {character.characterName} | Index {characterIndex}");

        RefreshUI();
        UpdateCharacterVisuals();
    }

    private void RefreshUI()
    {
        if (selectedCharacterText == null) return;

        int selectedIndex = PlayerPrefs.GetInt(selectedCharacterKey, 0);
        CharacterEntry character = GetCharacter(selectedIndex);

        if (character != null)
            selectedCharacterText.text = selectedPrefix + character.characterName;
        else
            selectedCharacterText.text = selectedPrefix + "Charakter " + selectedIndex;
    }

    private void UpdateCharacterVisuals()
    {
        if (characters == null) return;

        int selectedIndex = PlayerPrefs.GetInt(selectedCharacterKey, 0);

        foreach (CharacterEntry character in characters)
        {
            if (character == null || character.characterText == null) continue;

            bool unlocked = IsCharacterUnlocked(character.characterIndex);
            bool selected = unlocked && character.characterIndex == selectedIndex;

            if (selected)
            {
                character.characterText.color = selectedColor;
                character.characterText.text = selectedMarker + character.characterName;
            }
            else if (unlocked)
            {
                character.characterText.color = unlockedColor;
                character.characterText.text = character.characterName;
            }
            else
            {
                character.characterText.color = lockedColor;
                character.characterText.text = character.characterName;
            }
        }
    }

    private void EnsureDefaultUnlocks()
    {
        if (characters == null) return;

        foreach (CharacterEntry character in characters)
        {
            if (character == null) continue;

            if (character.characterIndex == 0 || character.unlockedByDefault)
            {
                string key = GetUnlockKey(character.characterIndex);
                PlayerPrefs.SetInt(key, 1);
                PlayerPrefsKeyTracker.TrackKey(key);

                // Name des freigeschalteten Characters speichern
                string nameKey = key + "_Name";
                PlayerPrefs.SetString(nameKey, character.characterName);
                PlayerPrefsKeyTracker.TrackKey(nameKey);
            }
        }

        PlayerPrefs.Save();
    }

    private bool IsCharacterUnlocked(int characterIndex)
    {
        if (characterIndex == 0) return true;
        return PlayerPrefs.GetInt(GetUnlockKey(characterIndex), 0) == 1;
    }

    private CharacterEntry GetCharacter(int characterIndex)
    {
        if (characters == null) return null;

        foreach (CharacterEntry character in characters)
        {
            if (character != null && character.characterIndex == characterIndex)
                return character;
        }

        return null;
    }

    private string GetUnlockKey(int characterIndex)
    {
        return unlockKeyPrefix + characterIndex;
    }
}