using UnityEngine;
using Unity.Cinemachine;

public class CharacterSceneSelector : MonoBehaviour
{
    [Header("🎮 Player Varianten in dieser Szene")]
    [Tooltip("Alle Player-Objekte, die bereits in der Szene liegen. Reihenfolge muss gleich sein wie im Auswahlmenü.")]
    public GameObject[] playerCharacters;

    [Header("📍 Optionaler Spawnpunkt")]
    [Tooltip("Wenn gesetzt, wird der aktive Charakter an diese Position gesetzt.")]
    public Transform spawnPoint;

    [Header("🎥 Cinemachine")]
    public CinemachineCamera virtualCamera;

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
            Debug.LogWarning("⚠️ Keine Player-Charaktere im CharacterSceneSelector eingetragen!");
            return;
        }

        int selectedIndex = PlayerPrefs.GetInt(selectedCharacterKey, defaultCharacterIndex);

        if (selectedIndex < 0 || selectedIndex >= playerCharacters.Length)
        {
            Debug.LogWarning($"⚠️ Ungültiger Charakter-Index {selectedIndex}. Nutze Default {defaultCharacterIndex}.");
            selectedIndex = defaultCharacterIndex;
        }

        GameObject activePlayer = null;

        for (int i = 0; i < playerCharacters.Length; i++)
        {
            if (playerCharacters[i] == null) continue;

            bool shouldBeActive = i == selectedIndex;
            playerCharacters[i].SetActive(shouldBeActive);

            if (shouldBeActive)
            {
                activePlayer = playerCharacters[i];

                if (spawnPoint != null)
                    activePlayer.transform.position = spawnPoint.position;
            }
        }

        if (virtualCamera != null && activePlayer != null)
        {
            virtualCamera.Follow = activePlayer.transform;
            virtualCamera.LookAt = activePlayer.transform;
            Debug.Log("🎥 Cinemachine folgt jetzt: " + activePlayer.name);
        }

        Debug.Log($"✅ Aktiver Charakter: Index {selectedIndex}");
    }
}