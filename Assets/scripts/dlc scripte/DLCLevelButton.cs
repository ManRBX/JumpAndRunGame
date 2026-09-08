using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DLCLevelButton : MonoBehaviour
{
    [Header("DLC Manager")]
    [SerializeField]
    private DLCButtonLoader dlcManager;

    [Header("DLC")]
    [SerializeField]
    private string dlcId;

    [Tooltip("AssetBundle-Dateiname, z.B. halloween2026")]
    [SerializeField]
    private string bundleFileName;

    [Header("Level")]
    [SerializeField]
    private string levelId;

    [Header("Level Freischaltung")]
    [Tooltip("PlayerPrefs-Key des vorherigen Levels. Leer = kein vorheriges Level nötig.")]
    [SerializeField]
    private string requiredCompletedPlayerPrefsKey;

    [Header("Darstellung")]
    [SerializeField]
    private Image buttonImage;

    [Tooltip("Normale Farbe, wenn das Level verfügbar ist.")]
    [SerializeField]
    private Color unlockedColor = Color.white;

    [Tooltip("Farbe, wenn DLC oder Level gesperrt ist.")]
    [SerializeField]
    private Color lockedColor = Color.black;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }

        UpdateButtonState();
    }

    private void OnEnable()
    {
        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        bool dlcInstalled = IsDLCInstalled();
        bool levelUnlocked = IsLevelUnlocked();

        bool available = dlcInstalled && levelUnlocked;

        if (button != null)
        {
            button.interactable = available;
        }

        if (buttonImage != null)
        {
            buttonImage.color =
                available ? unlockedColor : lockedColor;
        }

        Debug.Log(
            $"[DLC BUTTON] {levelId} | " +
            $"DLC installiert: {dlcInstalled} | " +
            $"Level freigeschaltet: {levelUnlocked}"
        );
    }

    public void LoadLevel()
    {
        if (!IsDLCInstalled())
        {
            Debug.LogWarning(
                $"[DLC] DLC '{dlcId}' ist nicht installiert."
            );

            return;
        }

        if (!IsLevelUnlocked())
        {
            Debug.LogWarning(
                $"[DLC] Level '{levelId}' ist noch gesperrt."
            );

            return;
        }

        if (dlcManager == null)
        {
            Debug.LogError(
                "[DLC] DLCButtonLoader wurde nicht gesetzt."
            );

            return;
        }

        dlcManager.LoadDLCLevel(
            dlcId,
            levelId
        );
    }

    private bool IsDLCInstalled()
    {
        if (string.IsNullOrWhiteSpace(bundleFileName))
        {
            return false;
        }

        string path = Path.Combine(
            Application.streamingAssetsPath,
            bundleFileName
        );

        return File.Exists(path);
    }

    private bool IsLevelUnlocked()
    {
        // Kein vorheriges Level erforderlich.
        if (string.IsNullOrWhiteSpace(
            requiredCompletedPlayerPrefsKey))
        {
            return true;
        }

        return PlayerPrefs.GetInt(
            requiredCompletedPlayerPrefsKey,
            0
        ) == 1;
    }
}