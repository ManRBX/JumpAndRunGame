using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;
using TMPro;

public class AutoSecretTileWall : MonoBehaviour
{
    [Header("Tilemap-Einstellungen")]
    public Tilemap tilemap;
    public List<TileBase> secretTiles;
    public float fadeDuration = 0.5f;

    [Header("Geheimer Raum Licht")]
    public List<Light2D> secretRoomLights;

    [Header("Taschenlampe des Spielers")]
    public List<Light2D> playerFlashlights;

    [Header("Globales Licht")]
    public Light2D globalLight;
    public float globalLightIntensityInside = 0.004f;
    public float globalLightFadeDuration = 0.5f;
    private float globalLightIntensityOutside;

    [Header("Secret Name Anzeige")]
    public bool displaySecretName = true;
    public string secretName = "Geheimer Raum";
    public TMP_Text secretNameText;
    public float secretNameDisplayDuration = 5f;

    private List<Vector3Int> secretTilePositions = new List<Vector3Int>();
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();
    private Dictionary<Vector3Int, Color> originalColors = new Dictionary<Vector3Int, Color>();

    private bool isPlayerInside = false;
    private Coroutine fadeTilesCoroutine;
    private Coroutine fadeGlobalLightCoroutine;
    private Coroutine secretNameCoroutine;

    void Start()
    {
        if (globalLight != null)
            globalLightIntensityOutside = globalLight.intensity;

        if (tilemap == null) { Debug.LogError("Tilemap nicht zugewiesen!"); return; }

        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null && secretTiles.Contains(tile))
                {
                    secretTilePositions.Add(pos);
                    originalTiles[pos] = tile;
                    originalColors[pos] = tilemap.GetColor(pos);
                }
            }
        }

        foreach (var l in secretRoomLights) if (l != null) l.enabled = false;
        foreach (var l in playerFlashlights) if (l != null) l.enabled = false;
        if (secretNameText != null) secretNameText.gameObject.SetActive(false);

        // einmalig nach einem Frame prüfen ob Player bereits im Trigger ist
        Invoke(nameof(CheckPlayerAlreadyInside), 0.1f);
    }

    private void CheckPlayerAlreadyInside()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;
        Collider2D[] hits = Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) { OnTriggerEnter2D(hit); break; }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isPlayerInside) return;
        isPlayerInside = true;

        foreach (var l in secretRoomLights) if (l != null) l.enabled = true;
        foreach (var l in playerFlashlights) if (l != null) l.enabled = true;

        if (globalLight != null)
        {
            if (fadeGlobalLightCoroutine != null) StopCoroutine(fadeGlobalLightCoroutine);
            fadeGlobalLightCoroutine = StartCoroutine(FadeGlobalLight(globalLight.intensity, globalLightIntensityInside, globalLightFadeDuration));
        }

        if (displaySecretName && secretNameText != null)
        {
            if (secretNameCoroutine != null) StopCoroutine(secretNameCoroutine);
            secretNameCoroutine = StartCoroutine(DisplaySecretName());
        }

        if (fadeTilesCoroutine != null) StopCoroutine(fadeTilesCoroutine);
        fadeTilesCoroutine = StartCoroutine(FadeAndDisableTiles());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !isPlayerInside) return;
        isPlayerInside = false;

        foreach (var l in secretRoomLights) if (l != null) l.enabled = false;
        foreach (var l in playerFlashlights) if (l != null) l.enabled = false;

        if (globalLight != null)
        {
            if (fadeGlobalLightCoroutine != null) StopCoroutine(fadeGlobalLightCoroutine);
            fadeGlobalLightCoroutine = StartCoroutine(FadeGlobalLight(globalLight.intensity, globalLightIntensityOutside, globalLightFadeDuration));
        }

        if (fadeTilesCoroutine != null) StopCoroutine(fadeTilesCoroutine);
        fadeTilesCoroutine = StartCoroutine(FadeAndRestoreTiles());
    }

    private IEnumerator DisplaySecretName()
    {
        secretNameText.text = secretName;
        secretNameText.gameObject.SetActive(true);
        yield return new WaitForSeconds(secretNameDisplayDuration);
        secretNameText.gameObject.SetActive(false);
        secretNameCoroutine = null;
    }

    private IEnumerator FadeAndDisableTiles()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            foreach (var pos in secretTilePositions)
            {
                Color c = tilemap.GetColor(pos);
                c.a = alpha;
                tilemap.SetColor(pos, c);
            }
            yield return null;
        }
        foreach (var pos in secretTilePositions)
            tilemap.SetTile(pos, null);
        fadeTilesCoroutine = null;
    }

    private IEnumerator FadeAndRestoreTiles()
    {
        foreach (var pos in secretTilePositions)
            if (originalTiles.ContainsKey(pos))
                tilemap.SetTile(pos, originalTiles[pos]);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            foreach (var pos in secretTilePositions)
            {
                Color c = originalColors.ContainsKey(pos) ? originalColors[pos] : Color.white;
                c.a = alpha;
                tilemap.SetColor(pos, c);
            }
            yield return null;
        }
        foreach (var pos in secretTilePositions)
            if (originalColors.ContainsKey(pos))
                tilemap.SetColor(pos, originalColors[pos]);
        fadeTilesCoroutine = null;
    }

    private IEnumerator FadeGlobalLight(float startIntensity, float targetIntensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }
        globalLight.intensity = targetIntensity;
        fadeGlobalLightCoroutine = null;
    }
}