using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSlider : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    [Header("Audio")]
    public AudioSource musicSource; // Zieh hier deine AudioSource rein

    private const string VolumeKey = "Volume";

    // Speichert die Lautstärke dauerhaft während Spiel läuft (über Szenen hinweg)
    private static bool volumeLoaded = false;
    private static float currentVolume = 0.5f; // Standard 50%, falls nix gespeichert ist

    private void Awake()
    {
        if (volumeSlider != null)
        {
            // Listener immer neu setzen, damit nichts doppelt ist
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnVolumeChange);
        }
    }

    private void Start()
    {
        // Nur beim ersten Laden die Lautstärke aus PlayerPrefs holen
        if (!volumeLoaded)
        {
            currentVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
            volumeLoaded = true;
        }

        ApplyVolume(currentVolume);

        if (volumeSlider != null)
        {
            volumeSlider.value = currentVolume;
        }

        UpdateVolumeText(currentVolume);

        // Optional: Tracke den Key (wenn du PlayerPrefsKeyTracker nutzt)
        PlayerPrefsKeyTracker.TrackKey(VolumeKey);
    }

    private void OnVolumeChange(float newVolume)
    {
        currentVolume = newVolume;

        ApplyVolume(currentVolume);

        PlayerPrefs.SetFloat(VolumeKey, currentVolume);
        PlayerPrefs.Save();

        PlayerPrefsKeyTracker.TrackKey(VolumeKey);
        UpdateVolumeText(currentVolume);
    }

    private void ApplyVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    private void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            int volumePercent = Mathf.RoundToInt(volume * 100f);
            volumeText.text = $"{volumePercent}%";
        }
    }
}
