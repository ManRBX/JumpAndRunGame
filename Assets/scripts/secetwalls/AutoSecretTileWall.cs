using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal;

public class AutoSecretTileWall : MonoBehaviour
{
    [Header("Tilemap-Einstellungen")]
    public Tilemap tilemap;
    [Tooltip("Diese Tiles gelten als geheime Wände")]
    public List<TileBase> secretTiles;
    public float fadeDuration = 0.5f;

    [Header("Geheimer Raum Licht")]
    public Light2D secretRoomLight;

    [Header("Taschenlampe des Spielers")]
    public Light2D playerFlashlight;

    [Header("Globales Licht Settings")]
    public bool changeGlobalLight = false;
    public Light2D globalLight; // dein Global Light 2D
    public float globalLightIntensityInside = 0.5f; // Wie hell es im Secret Room sein soll
    public float globalLightIntensityOutside = 1f;  // Normale Helligkeit draußen
    public float globalLightFadeDuration = 0.5f;    // Dauer fürs sanfte Umstellen

    private List<Vector3Int> secretTilePositions = new List<Vector3Int>();
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();
    private Dictionary<Vector3Int, Color> originalColors = new Dictionary<Vector3Int, Color>();

    private bool isPlayerInside = false;

    void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap nicht zugewiesen!");
            return;
        }

        if (secretTiles == null || secretTiles.Count == 0)
        {
            Debug.LogWarning("Keine Secret Tiles angegeben!");
        }

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

        Debug.Log("Gefundene geheime Wände: " + secretTilePositions.Count);

        if (secretRoomLight != null)
            secretRoomLight.enabled = false;

        if (playerFlashlight != null)
            playerFlashlight.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlayerInside)
        {
            isPlayerInside = true;

            if (secretRoomLight != null)
                secretRoomLight.enabled = true;

            if (playerFlashlight != null)
                playerFlashlight.enabled = true;

            if (changeGlobalLight && globalLight != null)
                StartCoroutine(FadeGlobalLight(globalLight.intensity, globalLightIntensityInside, globalLightFadeDuration));

            StartCoroutine(FadeAndDisableTiles());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isPlayerInside)
        {
            isPlayerInside = false;

            if (secretRoomLight != null)
                secretRoomLight.enabled = false;

            if (playerFlashlight != null)
                playerFlashlight.enabled = false;

            if (changeGlobalLight && globalLight != null)
                StartCoroutine(FadeGlobalLight(globalLight.intensity, globalLightIntensityOutside, globalLightFadeDuration));

            StartCoroutine(FadeAndRestoreTiles());
        }
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
        {
            tilemap.SetTile(pos, null);
        }
    }

    private IEnumerator FadeAndRestoreTiles()
    {
        foreach (var pos in secretTilePositions)
        {
            tilemap.SetTile(pos, originalTiles[pos]);
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            foreach (var pos in secretTilePositions)
            {
                Color c = originalColors[pos];
                c.a = alpha;
                tilemap.SetColor(pos, c);
            }

            yield return null;
        }

        foreach (var pos in secretTilePositions)
        {
            tilemap.SetColor(pos, originalColors[pos]);
        }
    }

    private IEnumerator FadeGlobalLight(float startIntensity, float targetIntensity, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / duration);
            }
            yield return null;
        }

        if (globalLight != null)
        {
            globalLight.intensity = targetIntensity;
        }
    }
}
