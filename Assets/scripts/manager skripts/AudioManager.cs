using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Range(0f, 1f)]
    public float volume = 0.5f;

    private const string VolumeKey = "Volume";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // FIX: Remove any UI blocking (falls AudioManager fälschlich mit UI spawnt)
            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                Destroy(canvas.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        volume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        PlayerPrefsKeyTracker.TrackKey(VolumeKey); // 🧠 Track den Key auch beim Laden
        ApplyVolume();
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume); // Sicherheits-Check
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefsKeyTracker.TrackKey(VolumeKey);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    void ApplyVolume()
    {
        AudioListener.volume = volume;
        Debug.Log("🔊 Volume set to: " + Mathf.RoundToInt(volume * 100) + "%");
    }
}
