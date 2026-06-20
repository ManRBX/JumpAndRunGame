using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;

[System.Serializable]
public class SceneCharacter
{
    public string characterName;
    public GameObject characterObject;
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

        GameObject activePlayer = null;
        string activeName = "Unknown";

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

                if (spawnPoint != null)
                    activePlayer.transform.position = spawnPoint.position;
            }
        }

        // 🎥 Cinemachine aktualisieren
        if (virtualCamera != null && activePlayer != null)
        {
            virtualCamera.Follow = activePlayer.transform;
            virtualCamera.LookAt = activePlayer.transform;

            Debug.Log("🎥 Kamera folgt jetzt: " + activePlayer.name);
        }

        // 🧾 UI aktualisieren
        if (characterNameText != null)
        {
            characterNameText.text = namePrefix + activeName;
        }

        Debug.Log($"✅ Aktiver Charakter: {activeName} | Index {selectedIndex}");
    }
}