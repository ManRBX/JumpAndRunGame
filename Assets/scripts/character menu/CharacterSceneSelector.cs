using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

[System.Serializable]
public class SceneCharacter
{
    public string characterName;
    public GameObject characterObject;
    public Sprite characterImage; // Bild das angezeigt wird wenn dieser Character aktiv ist
}

public class CharacterSceneSelector : MonoBehaviour
{
    [Header("🎮 Player Varianten in dieser Szene")]
    public SceneCharacter[] playerCharacters;

    [Header("📍 Optionaler Spawnpunkt")]
    public Transform spawnPoint;

    [Header("🎥 Cinemachine")]
    public CinemachineCamera virtualCamera;

    [Header("🧾 UI Anzeige")]
    public Text characterNameText;
    public string namePrefix = "Character: ";

    [Header("🖼️ Character Bild")]
    public Image characterImage; // Das Image das gewechselt wird

    [Header("⚙️ Einstellungen")]
    public string selectedCharacterKey = "GlobalSelectedCharacter";
    public int defaultCharacterIndex = 0;

    private void Start()
    {
        ActivateSelectedCharacter();
    }

    public void ActivateSelectedCharacter()
    {
        if (playerCharacters == null || playerCharacters.Length == 0)
        {
            Debug.LogWarning("⚠️ Keine Charaktere eingetragen!");
            return;
        }

        int selectedIndex = PlayerPrefs.GetInt(selectedCharacterKey, defaultCharacterIndex);

        if (selectedIndex < 0 || selectedIndex >= playerCharacters.Length)
        {
            Debug.LogWarning($"⚠️ Ungültiger Charakter-Index {selectedIndex}. Verwende Default {defaultCharacterIndex}.");
            selectedIndex = defaultCharacterIndex;
        }

        string unlockKey = "CharacterUnlocked_" + selectedIndex;
        bool isUnlocked = selectedIndex == defaultCharacterIndex || PlayerPrefs.GetInt(unlockKey, 0) == 1;

        if (!isUnlocked)
        {
            Debug.LogWarning($"⚠️ Charakter {selectedIndex} nicht freigeschaltet! Verwende Default.");
            selectedIndex = defaultCharacterIndex;
        }

        GameObject activePlayer = null;
        string activeName = "Unknown";
        Sprite activeSprite = null;

        for (int i = 0; i < playerCharacters.Length; i++)
        {
            if (playerCharacters[i] == null || playerCharacters[i].characterObject == null)
                continue;

            bool isSelected = i == selectedIndex;
            playerCharacters[i].characterObject.SetActive(isSelected);

            if (isSelected)
            {
                activePlayer = playerCharacters[i].characterObject;
                activeName = playerCharacters[i].characterName;
                activeSprite = playerCharacters[i].characterImage;

                if (spawnPoint != null)
                    activePlayer.transform.position = spawnPoint.position;
            }
        }

        // Character Bild wechseln
        if (characterImage != null && activeSprite != null)
            characterImage.sprite = activeSprite;

        // PlayerPrefs speichern
        PlayerPrefs.SetInt(selectedCharacterKey, selectedIndex);
        PlayerPrefs.SetString("GlobalSelectedCharacterName", activeName);
        PlayerPrefsKeyTracker.TrackKey(selectedCharacterKey);
        PlayerPrefsKeyTracker.TrackKey("GlobalSelectedCharacterName");
        PlayerPrefs.Save();

        // Cinemachine aktualisieren
        if (virtualCamera != null && activePlayer != null)
        {
            virtualCamera.Follow = activePlayer.transform;
            virtualCamera.LookAt = activePlayer.transform;
            Debug.Log("🎥 Kamera folgt jetzt: " + activePlayer.name);
        }

        // UI aktualisieren
        if (characterNameText != null)
            characterNameText.text = namePrefix + activeName;

        Debug.Log($"✅ Aktiver Charakter: {activeName} | Index {selectedIndex}");
    }

    public void SelectCharacter(int index)
    {
        string unlockKey = "CharacterUnlocked_" + index;
        bool isUnlocked = index == defaultCharacterIndex || PlayerPrefs.GetInt(unlockKey, 0) == 1;

        if (!isUnlocked)
        {
            Debug.LogWarning($"⚠️ Charakter {index} ist noch nicht freigeschaltet!");
            return;
        }

        PlayerPrefs.SetInt(selectedCharacterKey, index);
        PlayerPrefsKeyTracker.TrackKey(selectedCharacterKey);
        PlayerPrefs.Save();

        ActivateSelectedCharacter();
    }
}