using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;

    private const string VolumeKey = "Volume";

    void Start()
    {
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
            volumeSlider.value = savedVolume;
            PlayerPrefsKeyTracker.TrackKey(VolumeKey);
            UpdateVolumeText(savedVolume);

            volumeSlider.onValueChanged.AddListener(OnVolumeChange);
        }
    }

    public void OnVolumeChange(float newVolume)
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetVolume(newVolume);
        }

        PlayerPrefs.SetFloat(VolumeKey, newVolume);
        PlayerPrefsKeyTracker.TrackKey(VolumeKey);
        PlayerPrefs.Save();

        UpdateVolumeText(newVolume);
    }

    void UpdateVolumeText(float volume)
    {
        if (volumeText == null) return;

        int volumePercent = Mathf.RoundToInt(volume * 100);
        volumeText.text = volumePercent + "%";
    }
}
