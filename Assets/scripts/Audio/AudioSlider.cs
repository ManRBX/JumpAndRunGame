using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class AudioSlider : MonoBehaviour
{
    [Header("🎛 UI-Elemente")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    [Header("🎚️ AudioMixer")]
    public AudioMixer audioMixer; // Im Inspector setzen
    public string exposedParameter = "MusicVolume"; // z. B. MusicVolume, SFXVolume

    [Header("🧠 Speicher-Key")]
    public string volumeKey = "Volume_Music"; // Jeder Regler hat eigenen Key

    private float currentVolume = 0.5f; // 0.5 = 50%
    private const float MIN_NON_ZERO = 0.0000001f;

    private void Awake()
    {
        PlayerPrefsKeyTracker.TrackKey(volumeKey);

        // Load gespeicherte Lautstärke
        if (PlayerPrefs.HasKey(volumeKey))
        {
            try
            {
                currentVolume = PlayerPrefs.GetFloat(volumeKey);

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

        // Mixer-Wert setzen
        SetMixerVolume(currentVolume);

        // Slider initialisieren
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnVolumeChange);
            volumeSlider.value = currentVolume;
        }

        UpdateVolumeText(currentVolume);
    }

    private void OnVolumeChange(float newVolume)
    {
        currentVolume = newVolume;

        SetMixerVolume(currentVolume);

        float savedVolume = Mathf.Approximately(currentVolume, 0f) ? MIN_NON_ZERO : currentVolume;

        PlayerPrefs.SetFloat(volumeKey, savedVolume);
        PlayerPrefs.Save();

        PlayerPrefsKeyTracker.TrackKey(volumeKey);
        UpdateVolumeText(currentVolume);
    }

    private void SetMixerVolume(float volume)
    {
        if (audioMixer == null || string.IsNullOrEmpty(exposedParameter))
            return;

        float dB = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20f;

        bool result = audioMixer.SetFloat(exposedParameter, dB);

        Debug.Log($"🔊 Set Mixer '{exposedParameter}' auf {dB} dB | Erfolg: {result}");
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
