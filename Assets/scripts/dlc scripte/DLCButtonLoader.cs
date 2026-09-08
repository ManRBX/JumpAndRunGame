using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DLCButtonLoader : MonoBehaviour
{
    [Serializable]
    public class DLCLevel
    {
        [Header("Level")]
        [Tooltip("Eindeutige ID für dieses Level, z.B. haunted_woods")]
        public string levelId;

        [Tooltip("Scene-Name innerhalb des AssetBundles, z.B. Halloween1")]
        public string sceneName;
    }

    [Serializable]
    public class DLCEntry
    {
        [Header("DLC")]
        [Tooltip("Eindeutige DLC-ID, z.B. halloween2026")]
        public string dlcId;

        [Tooltip("Dateiname des AssetBundles, z.B. halloween2026")]
        public string bundleFileName;

        [Header("Levels")]
        [Tooltip("Hier kannst du beliebig viele Levels für diesen DLC hinzufügen.")]
        public List<DLCLevel> levels = new List<DLCLevel>();
    }

    [Header("DLC Liste")]
    [Tooltip("Hier kannst du beliebig viele DLCs hinzufügen.")]
    [SerializeField]
    private List<DLCEntry> dlcs = new List<DLCEntry>();

    private AssetBundle loadedBundle;

    /// <summary>
    /// Wird vom Unity Button aufgerufen.
    ///
    /// Beispiel:
    /// DLC ID: halloween2026
    /// Level ID: haunted_woods
    /// </summary>
    public void LoadDLCLevel(string dlcId, string levelId)
    {
        if (string.IsNullOrWhiteSpace(dlcId))
        {
            Debug.LogError("[DLC] DLC ID ist leer.");
            return;
        }

        if (string.IsNullOrWhiteSpace(levelId))
        {
            Debug.LogError("[DLC] Level ID ist leer.");
            return;
        }

        DLCEntry dlc = FindDLC(dlcId);

        if (dlc == null)
        {
            Debug.LogError(
                $"[DLC] DLC mit ID '{dlcId}' wurde nicht gefunden."
            );

            return;
        }

        DLCLevel level = FindLevel(dlc, levelId);

        if (level == null)
        {
            Debug.LogError(
                $"[DLC] Level '{levelId}' wurde im DLC '{dlcId}' nicht gefunden."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(dlc.bundleFileName))
        {
            Debug.LogError(
                $"[DLC] Für DLC '{dlcId}' wurde kein Bundle-Dateiname angegeben."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(level.sceneName))
        {
            Debug.LogError(
                $"[DLC] Für Level '{levelId}' wurde kein Scene-Name angegeben."
            );

            return;
        }

        string bundlePath = Path.Combine(
            Application.streamingAssetsPath,
            dlc.bundleFileName
        );

        Debug.Log($"[DLC] DLC ID: {dlcId}");
        Debug.Log($"[DLC] Level ID: {levelId}");
        Debug.Log($"[DLC] Scene: {level.sceneName}");
        Debug.Log($"[DLC] Bundle Pfad: {bundlePath}");

        if (!File.Exists(bundlePath))
        {
            Debug.LogWarning(
                $"[DLC] AssetBundle wurde nicht gefunden.\n" +
                $"Pfad: {bundlePath}"
            );

            return;
        }

        loadedBundle = FindLoadedBundle(dlc.bundleFileName);

        if (loadedBundle == null)
        {
            loadedBundle = AssetBundle.LoadFromFile(bundlePath);

            if (loadedBundle == null)
            {
                Debug.LogError(
                    $"[DLC] AssetBundle '{dlc.bundleFileName}' konnte nicht geladen werden."
                );

                return;
            }

            Debug.Log(
                $"[DLC] Bundle '{dlc.bundleFileName}' wurde erfolgreich geladen."
            );
        }
        else
        {
            Debug.Log(
                $"[DLC] Bundle '{dlc.bundleFileName}' ist bereits geladen."
            );
        }

        string[] scenePaths = loadedBundle.GetAllScenePaths();

        if (scenePaths == null || scenePaths.Length == 0)
        {
            Debug.LogError(
                $"[DLC] Bundle '{dlc.bundleFileName}' enthält keine Scenes."
            );

            return;
        }

        bool sceneFound = false;

        foreach (string scenePath in scenePaths)
        {
            string foundSceneName =
                Path.GetFileNameWithoutExtension(scenePath);

            Debug.Log(
                $"[DLC] Scene im Bundle: {foundSceneName}"
            );

            if (string.Equals(
                foundSceneName,
                level.sceneName,
                StringComparison.OrdinalIgnoreCase))
            {
                sceneFound = true;
                break;
            }
        }

        if (!sceneFound)
        {
            Debug.LogError(
                $"[DLC] Scene '{level.sceneName}' wurde im Bundle " +
                $"'{dlc.bundleFileName}' nicht gefunden."
            );

            return;
        }

        Debug.Log(
            $"[DLC] Lade Scene '{level.sceneName}'..."
        );

        SceneManager.LoadSceneAsync(level.sceneName);
    }

    private DLCEntry FindDLC(string dlcId)
    {
        foreach (DLCEntry dlc in dlcs)
        {
            if (dlc == null)
                continue;

            if (string.Equals(
                dlc.dlcId,
                dlcId,
                StringComparison.OrdinalIgnoreCase))
            {
                return dlc;
            }
        }

        return null;
    }

    private DLCLevel FindLevel(DLCEntry dlc, string levelId)
    {
        if (dlc.levels == null)
            return null;

        foreach (DLCLevel level in dlc.levels)
        {
            if (level == null)
                continue;

            if (string.Equals(
                level.levelId,
                levelId,
                StringComparison.OrdinalIgnoreCase))
            {
                return level;
            }
        }

        return null;
    }

    private AssetBundle FindLoadedBundle(string bundleFileName)
    {
        string expectedName =
            Path.GetFileNameWithoutExtension(bundleFileName);

        foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (bundle == null)
                continue;

            if (string.Equals(
                bundle.name,
                expectedName,
                StringComparison.OrdinalIgnoreCase))
            {
                return bundle;
            }
        }

        return null;
    }

    /// <summary>
    /// Optional:
    /// Lädt aktuell geladenes DLC-Bundle wieder aus dem Speicher.
    /// Kann später z.B. beim Zurückkehren ins Menü verwendet werden.
    /// </summary>
    public void UnloadCurrentBundle()
    {
        if (loadedBundle == null)
            return;

        loadedBundle.Unload(false);
        loadedBundle = null;

        Debug.Log("[DLC] Aktuelles Bundle wurde entladen.");
    }
}