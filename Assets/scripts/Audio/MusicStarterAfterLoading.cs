using UnityEngine;

public class MusicStarterAfterLoading : MonoBehaviour
{
    public AudioSource musicSource;

    private void Start()
    {
        musicSource.volume = PlayerPrefs.GetFloat("Volume", 0.5f);
        musicSource.Play(); // ⏯️ Musik starten nach dem Laden
    }
}
