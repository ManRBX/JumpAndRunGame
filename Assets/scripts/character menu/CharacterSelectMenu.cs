using UnityEngine;
using TMPro;

public class CharacterSelectMenu : MonoBehaviour
{
    [Header("💾 Speicher-Key")]
    public string selectedCharacterKey = "GlobalSelectedCharacter";

    [Header("🧾 UI Optional")]
    public TMP_Text selectedCharacterText;

    [Header("🎭 Namen Optional")]
    public string[] characterNames;

    private void Start()
    {
        RefreshUI();
    }

    public void SelectCharacter(int characterIndex)
    {
        PlayerPrefs.SetInt(selectedCharacterKey, characterIndex);
        PlayerPrefsKeyTracker.TrackKey(selectedCharacterKey);
        PlayerPrefs.Save();

        Debug.Log($"🎭 Charakter gewählt: {characterIndex}");

        RefreshUI();
    }

    private void RefreshUI()
    {
        if (selectedCharacterText == null) return;

        int selectedIndex = PlayerPrefs.GetInt(selectedCharacterKey, 0);

        if (characterNames != null && selectedIndex >= 0 && selectedIndex < characterNames.Length)
            selectedCharacterText.text = "Ausgewählt: " + characterNames[selectedIndex];
        else
            selectedCharacterText.text = "Ausgewählt: Charakter " + selectedIndex;
    }
}