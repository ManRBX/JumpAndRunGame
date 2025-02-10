using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;  // Nur die Prozentanzeige

    void Start()
    {
        // Initialisiere den Slider mit gespeicherter Lautstärke (Standard: 50%)
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

        // Falls du einen AudioManager hast, hier die Lautstärke setzen
        if (AudioManager.instance != null)
        {
            AudioManager.instance.SetVolume(newVolume);
        }

        // Lautstärke speichern
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
