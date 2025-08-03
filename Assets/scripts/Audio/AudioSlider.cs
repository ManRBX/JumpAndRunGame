using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AudioSlider : MonoBehaviour
{
    [Header("🎛 UI-Elemente")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    [Header("🎵 AudioQuellen (beliebig viele)")]
    public List<AudioSource> audioSources = new List<AudioSource>();

    [Header("🧠 Speicher-Key")]
    public string volumeKey = "Volume"; // Jeder Regler kann eigenen Key haben

    private float currentVolume = 0.5f; // Standardwert (50 %)
    private const float MIN_NON_ZERO = 0.0000001f;

    private void Awake()
    {
        PlayerPrefsKeyTracker.TrackKey(volumeKey);

        // Falls fälschlich als String gespeichert – löschen
        if (PlayerPrefs.HasKey(volumeKey))
        {
            try
            {
                currentVolume = PlayerPrefs.GetFloat(volumeKey);

                // Falls zuvor als Miniwert gespeichert: für Player wie 0 behandeln
                if (Mathf.Approximately(currentVolume, MIN_NON_ZERO))
                    currentVolume = 0f;
            }
            catch
            {
                Debug.LogWarning($"⚠️ PlayerPrefs-Key '{volumeKey}' war kein Float. Reset auf 0.5f.");
                PlayerPrefs.DeleteKey(volumeKey);
                currentVolume = 0.5f;
            }
        }
        else
        {
            currentVolume = 0.5f;
        }

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
        foreach (var source in audioSources)
        {
            if (source != null && !source.isPlaying)
            {
                source.Play();
            }
        }
    }

    private void OnVolumeChange(float newVolume)
    {
        currentVolume = newVolume;

        ApplyVolume(currentVolume);

        float savedVolume = Mathf.Approximately(currentVolume, 0f) ? MIN_NON_ZERO : currentVolume;

        PlayerPrefs.SetFloat(volumeKey, savedVolume);
        PlayerPrefs.Save();

        PlayerPrefsKeyTracker.TrackKey(volumeKey);
        UpdateVolumeText(currentVolume);
    }

    private void ApplyVolume(float volume)
    {
        foreach (var source in audioSources)
        {
            if (source != null)
                source.volume = volume;
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
