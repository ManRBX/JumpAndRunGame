using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class VersionManager : MonoBehaviour
{
    public static VersionManager Instance;

    public enum VersionAnchor
    {
        BottomLeft,
        BottomRight,
        TopLeft,
        TopRight,
        CenterBottom,
        CenterTop
    }

    [Header("🔢 Version")]
    public string gameVersion = "v0.0.1a";

    [Header("🧾 Text")]
    public string prefix = "Version ";
    public Color textColor = Color.white;
    public int fontSize = 24;

    [Header("📍 Position")]
    public VersionAnchor anchor = VersionAnchor.BottomRight;
    public Vector2 anchoredPosition = new Vector2(-40f, 30f);

    [Header("⚙️ Verhalten")]
    public bool persistAcrossScenes = true;
    public bool applyStyleFromManager = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
            DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        UpdateAllVersionTexts();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAllVersionTexts();
    }

    public void UpdateAllVersionTexts()
    {
        VersionText[] versionTexts = FindObjectsByType<VersionText>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (VersionText versionText in versionTexts)
        {
            versionText.ApplyVersion(
                prefix + gameVersion,
                textColor,
                fontSize,
                anchor,
                anchoredPosition,
                applyStyleFromManager
            );
        }
    }
}