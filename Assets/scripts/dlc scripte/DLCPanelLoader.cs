using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DLCPanelLoader : MonoBehaviour
{
    [Serializable]
    public class DLCPanelEntry
    {
        [Header("DLC")]
        [Tooltip("Steam DLC App-ID, z.B. 3737900")]
        public string dlcId;

        [Tooltip("Dateiname des AssetBundles, z.B. halloween2026")]
        public string bundleFileName;

        [Header("Panel")]
        [Tooltip("Exakter Name des Panel-Prefabs im AssetBundle")]
        public string panelPrefabName;

        [Tooltip("Hier wird das Panel erzeugt, normalerweise dein Canvas oder ein Panel-Container.")]
        public Transform parent;
    }

    [Header("DLC Panels")]
    [Tooltip("Hier kannst du beliebig viele DLC-Panels hinzufügen.")]
    [SerializeField]
    private List<DLCPanelEntry> dlcPanels = new List<DLCPanelEntry>();

    private readonly Dictionary<string, GameObject> spawnedPanels =
        new Dictionary<string, GameObject>();

    private readonly Dictionary<string, AssetBundle> loadedBundles =
        new Dictionary<string, AssetBundle>();

    // ---------------------------------------------------------
    // PANEL LADEN
    // ---------------------------------------------------------

    public void LoadPanel(string dlcId)
    {
        DLCPanelEntry entry = FindEntry(dlcId);

        if (entry == null)
        {
            Debug.LogError(
                $"[DLC PANEL] DLC mit ID '{dlcId}' wurde nicht gefunden."
            );
            return;
        }

        // Panel wurde bereits erzeugt?
        if (spawnedPanels.TryGetValue(dlcId, out GameObject existingPanel))
        {
            if (existingPanel != null)
            {
                existingPanel.SetActive(true);

                Debug.Log(
                    $"[DLC PANEL] Panel für DLC '{dlcId}' war bereits geladen."
                );

                return;
            }

            spawnedPanels.Remove(dlcId);
        }

        if (string.IsNullOrWhiteSpace(entry.bundleFileName))
        {
            Debug.LogError(
                $"[DLC PANEL] Kein Bundle-Dateiname für DLC '{dlcId}' eingetragen."
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(entry.panelPrefabName))
        {
            Debug.LogError(
                $"[DLC PANEL] Kein Panel-Prefab für DLC '{dlcId}' eingetragen."
            );
            return;
        }

        string bundlePath = Path.Combine(
            Application.streamingAssetsPath,
            entry.bundleFileName
        );

        Debug.Log(
            $"[DLC PANEL] Suche Bundle unter: {bundlePath}"
        );

        if (!File.Exists(bundlePath))
        {
            Debug.LogWarning(
                $"[DLC PANEL] DLC-Bundle wurde nicht gefunden:\n{bundlePath}"
            );
            return;
        }

        AssetBundle bundle = GetOrLoadBundle(
            entry.bundleFileName,
            bundlePath
        );

        if (bundle == null)
        {
            Debug.LogError(
                $"[DLC PANEL] Bundle '{entry.bundleFileName}' konnte nicht geladen werden."
            );
            return;
        }

        GameObject panelPrefab =
            bundle.LoadAsset<GameObject>(entry.panelPrefabName);

        if (panelPrefab == null)
        {
            Debug.LogError(
                $"[DLC PANEL] Prefab '{entry.panelPrefabName}' " +
                $"wurde im Bundle '{entry.bundleFileName}' nicht gefunden."
            );

            Debug.Log(
                "[DLC PANEL] Vorhandene Assets im Bundle:"
            );

            foreach (string assetName in bundle.GetAllAssetNames())
            {
                Debug.Log(assetName);
            }

            return;
        }

        Transform targetParent = entry.parent;

        // Wenn kein Parent angegeben wurde, Canvas suchen.
        if (targetParent == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();

            if (canvas != null)
            {
                targetParent = canvas.transform;
            }
        }

        GameObject panelInstance;

        if (targetParent != null)
        {
            panelInstance = Instantiate(
                panelPrefab,
                targetParent,
                false
            );
        }
        else
        {
            panelInstance = Instantiate(panelPrefab);

            Debug.LogWarning(
                "[DLC PANEL] Kein Parent/Canvas gefunden. " +
                "Panel wurde ohne Parent erzeugt."
            );
        }

        panelInstance.name = entry.panelPrefabName;

        spawnedPanels[dlcId] = panelInstance;

        Debug.Log(
            $"[DLC PANEL] '{entry.panelPrefabName}' wurde erfolgreich geladen."
        );
    }

    // ---------------------------------------------------------
    // PANEL SCHLIESSEN
    // ---------------------------------------------------------

    public void ClosePanel(string dlcId)
    {
        if (!spawnedPanels.TryGetValue(
            dlcId,
            out GameObject panel))
        {
            return;
        }

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    // ---------------------------------------------------------
    // PANEL KOMPLETT ENTFERNEN
    // ---------------------------------------------------------

    public void DestroyPanel(string dlcId)
    {
        if (!spawnedPanels.TryGetValue(
            dlcId,
            out GameObject panel))
        {
            return;
        }

        if (panel != null)
        {
            Destroy(panel);
        }

        spawnedPanels.Remove(dlcId);
    }

    // ---------------------------------------------------------
    // DLC SUCHEN
    // ---------------------------------------------------------

    private DLCPanelEntry FindEntry(string dlcId)
    {
        foreach (DLCPanelEntry entry in dlcPanels)
        {
            if (entry == null)
                continue;

            if (string.Equals(
                entry.dlcId,
                dlcId,
                StringComparison.OrdinalIgnoreCase))
            {
                return entry;
            }
        }

        return null;
    }

    // ---------------------------------------------------------
    // ASSETBUNDLE LADEN
    // ---------------------------------------------------------

    private AssetBundle GetOrLoadBundle(
        string bundleFileName,
        string bundlePath)
    {
        string bundleKey =
            Path.GetFileNameWithoutExtension(bundleFileName);

        // Von diesem Loader bereits geladen?
        if (loadedBundles.TryGetValue(
            bundleKey,
            out AssetBundle cachedBundle))
        {
            if (cachedBundle != null)
                return cachedBundle;

            loadedBundles.Remove(bundleKey);
        }

        // Vielleicht wurde das Bundle bereits
        // vom DLCButtonLoader geladen.
        foreach (
            AssetBundle bundle
            in AssetBundle.GetAllLoadedAssetBundles())
        {
            if (bundle == null)
                continue;

            if (string.Equals(
                bundle.name,
                bundleKey,
                StringComparison.OrdinalIgnoreCase))
            {
                loadedBundles[bundleKey] = bundle;

                return bundle;
            }
        }

        AssetBundle newBundle =
            AssetBundle.LoadFromFile(bundlePath);

        if (newBundle != null)
        {
            loadedBundles[bundleKey] = newBundle;
        }

        return newBundle;
    }
}