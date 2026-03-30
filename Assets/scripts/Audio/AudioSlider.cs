using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class AudioVolumeSlider : MonoBehaviour
{
    [Header("🎛 UI")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    [Header("🎚 Mixer")]
    public AudioMixer audioMixer;
    public string exposedParameter = "MasterVolume";

    [Header("💾 Save Key")]
    public string volumeKey = "Volume_Master";

    [Header("⚙ Default")]
    [Range(0f, 1f)]
    public float defaultVolume = 0.5f;

    private float currentVolume;

    private void Start()
    {
        PlayerPrefsKeyTracker.TrackKey(volumeKey);

        if (PlayerPrefs.HasKey(volumeKey))
        {
            currentVolume = PlayerPrefs.GetFloat(volumeKey);
        }
        else
        {
            currentVolume = defaultVolume;
        }

        ApplyVolume(currentVolume);

        volumeSlider.onValueChanged.RemoveAllListeners();
        volumeSlider.onValueChanged.AddListener(OnVolumeChange);

        volumeSlider.value = currentVolume;

        UpdateText(currentVolume);
    }

    private void OnVolumeChange(float value)
    {
        currentVolume = value;

        ApplyVolume(currentVolume);

        PlayerPrefs.SetFloat(volumeKey, currentVolume);
        PlayerPrefs.Save();

        UpdateText(currentVolume);
    }

    private void ApplyVolume(float volume)
    {
        if (audioMixer == null)
            return;

        float dB;

        if (volume <= 0.0001f)
            dB = -80f;
        else
            dB = Mathf.Log10(volume) * 20f;

        audioMixer.SetFloat(exposedParameter, dB);
    }

    private void UpdateText(float volume)
    {
        if (volumeText != null)
        {
            int percent = Mathf.RoundToInt(volume * 100f);
            volumeText.text = percent + "%";
        }
    }
}