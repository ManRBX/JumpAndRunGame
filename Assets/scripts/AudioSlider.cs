using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;  // Only the percentage display

    void Start()
    {
        // Initialize the slider with the saved volume (default: 50%)
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 0.5f);
            UpdateVolumeText(volumeSlider.value);

            volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChange(); });
        }
    }

    public void OnVolumeChange()
    {
        float newVolume = volumeSlider.value;

        // If an AudioManager exists, update the volume
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetVolume(newVolume);
        }

        // Save the volume setting
        PlayerPrefs.SetFloat("Volume", newVolume);
        PlayerPrefs.Save();

        UpdateVolumeText(newVolume);
    }

    void UpdateVolumeText(float volume)
    {
        int volumePercent = Mathf.RoundToInt(volume * 100);
        volumeText.text = volumePercent + "%";
    }
}
