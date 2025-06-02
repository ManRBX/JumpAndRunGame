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
    private static float currentVolume = 0.5f; // Standardwert (50 %)

    private void Awake()
    {
        // Key für Export unbedingt sofort tracken, noch bevor irgendwas geladen wird
        PlayerPrefsKeyTracker.TrackKey(VolumeKey);

        currentVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        ApplyVolume(currentVolume);

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnVolumeChange);
            volumeSlider.value = currentVolume;
        }

        UpdateVolumeText(currentVolume);
    }

    private void Start()
    {
        // Falls AudioSource vorhanden ist und nicht automatisch spielt
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.volume = currentVolume;
            musicSource.Play(); // Nur wenn du möchtest, dass es nach dem Loading losgeht
        }
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
            musicSource.volume = volume;
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
