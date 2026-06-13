using UnityEngine;
using TMPro;

[System.Serializable]
public class CharacterEntry
{
    [Tooltip("Muss gleich sein wie der Button-Wert, z.B. 0, 1, 2...")]
    public int characterIndex;

    [Tooltip("Name, der im Menü angezeigt wird.")]
    public string characterName;
}

public class CharacterSelectMenu : MonoBehaviour
{
    [Header("💾 Speicher-Key")]
    public string selectedCharacterKey = "GlobalSelectedCharacter";

    [Header("🧾 UI")]
    public TMP_Text selectedCharacterText;

    [Header("🎭 Charaktere")]
    [Tooltip("Hier trägst du Index + Namen ein. Index muss mit dem Button-OnClick-Wert zusammenpassen.")]
    public CharacterEntry[] characters;

    private void Start()
    {
        RefreshUI();
    }

    public void SelectCharacter(int characterIndex)
    {
        PlayerPrefs.SetInt(selectedCharacterKey, characterIndex);
        PlayerPrefsKeyTracker.TrackKey(selectedCharacterKey);
        PlayerPrefs.Save();

        Debug.Log($"🎭 Charakter gewählt: Index {characterIndex} | Name: {GetCharacterName(characterIndex)}");

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (selectedCharacterText == null) return;

        int selectedIndex = PlayerPrefs.GetInt(selectedCharacterKey, 0);
        selectedCharacterText.text = "Ausgewählt: " + GetCharacterName(selectedIndex);
    }

    private string GetCharacterName(int characterIndex)
    {
        if (characters != null)
        {
            foreach (CharacterEntry character in characters)
            {
                if (character != null && character.characterIndex == characterIndex)
                {
                    if (!string.IsNullOrWhiteSpace(character.characterName))
                        return character.characterName;
                }
            }
        }

        return "Charakter " + characterIndex;
    }
}