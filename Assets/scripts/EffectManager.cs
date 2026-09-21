using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour
{
    [System.Serializable]
    public class EffectCategory
    {
        [Header("📁 Kategorie")]
        public string categoryName = "Neue Kategorie";

        [Tooltip("Alle Effekte dieser Kategorie.")]
        public List<GameObject> effects = new List<GameObject>();

        [Header("📏 Entfernung")]
        [Tooltip("Ab welcher Entfernung die Effekte aktiviert werden.")]
        public float activationDistance = 20f;

        [Tooltip("Zusätzliche Entfernung bis zum Deaktivieren.")]
        public float deactivateBuffer = 5f;

        [Header("⚙️ Kategorie aktiv")]
        public bool enabled = true;
    }

    [Header("🎮 Player")]
    [Tooltip("Optional. Wenn leer, wird automatisch der aktive Player gesucht.")]
    public Transform player;

    [Header("📁 Effekt Kategorien")]
    public List<EffectCategory> categories = new List<EffectCategory>();

    [Header("⚙️ Performance")]
    [Tooltip("Wie oft die Entfernung geprüft wird. 0.25 = 4x pro Sekunde.")]
    [Range(0.05f, 2f)]
    public float checkInterval = 0.25f;

    [Tooltip("Beim Start alle verwalteten Effekte zunächst deaktivieren.")]
    public bool disableEffectsOnStart = true;

    [Tooltip("Falls der Player wechselt, automatisch nach dem aktiven Player suchen.")]
    public bool automaticallyFindActivePlayer = true;

    private float nextCheckTime;

    // Merkt sich den Zustand jedes Effekts
    private Dictionary<GameObject, bool> effectStates =
        new Dictionary<GameObject, bool>();


    private void Start()
    {
        if (disableEffectsOnStart)
        {
            DisableAllEffects();
        }

        FindActivePlayer();

        // Sofort erste Prüfung
        CheckAllEffects();
    }


    private void Update()
    {
        if (Time.time < nextCheckTime)
            return;

        nextCheckTime = Time.time + checkInterval;

        // Falls kein Player vorhanden oder der aktuelle
        // Player deaktiviert wurde
        if (
            player == null ||
            !player.gameObject.activeInHierarchy
        )
        {
            if (automaticallyFindActivePlayer)
            {
                FindActivePlayer();
            }
        }

        if (player == null)
            return;

        CheckAllEffects();
    }


    // =========================================================
    // PLAYER SUCHEN
    // =========================================================

    private void FindActivePlayer()
    {
        GameObject[] players;

        try
        {
            players =
                GameObject.FindGameObjectsWithTag("Player");
        }
        catch
        {
            Debug.LogWarning(
                "[EffectManager] Tag 'Player' existiert nicht."
            );

            return;
        }

        foreach (GameObject playerObject in players)
        {
            if (playerObject == null)
                continue;

            if (!playerObject.activeInHierarchy)
                continue;

            player = playerObject.transform;

            return;
        }

        player = null;
    }


    // =========================================================
    // ALLE EFFEKTE PRÜFEN
    // =========================================================

    private void CheckAllEffects()
    {
        if (player == null)
            return;

        Vector3 playerPosition = player.position;

        foreach (EffectCategory category in categories)
        {
            if (category == null)
                continue;

            // Kategorie komplett deaktiviert
            if (!category.enabled)
            {
                DisableCategory(category);
                continue;
            }

            float activationDistance =
                Mathf.Max(
                    0f,
                    category.activationDistance
                );

            float deactivationDistance =
                activationDistance +
                Mathf.Max(
                    0f,
                    category.deactivateBuffer
                );

            float activationDistanceSquared =
                activationDistance *
                activationDistance;

            float deactivationDistanceSquared =
                deactivationDistance *
                deactivationDistance;


            foreach (GameObject effect in category.effects)
            {
                if (effect == null)
                    continue;


                // =============================================
                // Entfernung zwischen PLAYER und DIESEM EFFEKT
                // =============================================

                Vector3 difference =
                    effect.transform.position -
                    playerPosition;

                float distanceSquared =
                    difference.sqrMagnitude;


                // =============================================
                // Aktuellen Zustand holen
                // =============================================

                bool currentlyActive;

                if (effectStates.TryGetValue(
                    effect,
                    out bool storedState))
                {
                    currentlyActive = storedState;
                }
                else
                {
                    currentlyActive =
                        effect.activeSelf;

                    effectStates[effect] =
                        currentlyActive;
                }


                // =============================================
                // WENN EFFEKT AKTIV IST
                // =============================================

                if (currentlyActive)
                {
                    // Erst weiter draußen deaktivieren
                    if (
                        distanceSquared >
                        deactivationDistanceSquared
                    )
                    {
                        SetEffectState(
                            effect,
                            false
                        );
                    }
                }

                // =============================================
                // WENN EFFEKT AUS IST
                // =============================================

                else
                {
                    if (
                        distanceSquared <=
                        activationDistanceSquared
                    )
                    {
                        SetEffectState(
                            effect,
                            true
                        );
                    }
                }
            }
        }
    }


    // =========================================================
    // EINZELNEN EFFEKT AN / AUS
    // =========================================================

    private void SetEffectState(
        GameObject effect,
        bool active)
    {
        if (effect == null)
            return;

        if (
            effect.activeSelf != active
        )
        {
            effect.SetActive(active);
        }

        effectStates[effect] = active;
    }


    // =========================================================
    // ALLES DEAKTIVIEREN
    // =========================================================

    public void DisableAllEffects()
    {
        foreach (EffectCategory category in categories)
        {
            if (category == null)
                continue;

            foreach (GameObject effect in category.effects)
            {
                if (effect == null)
                    continue;

                effect.SetActive(false);

                effectStates[effect] = false;
            }
        }
    }


    // =========================================================
    // KATEGORIE DEAKTIVIEREN
    // =========================================================

    private void DisableCategory(
        EffectCategory category)
    {
        if (category == null)
            return;

        foreach (GameObject effect in category.effects)
        {
            if (effect == null)
                continue;

            SetEffectState(
                effect,
                false
            );
        }
    }


    // =========================================================
    // MANUELL PLAYER SETZEN
    // =========================================================

    public void SetPlayer(
        Transform newPlayer)
    {
        player = newPlayer;

        CheckAllEffects();
    }


    // =========================================================
    // SOFORT ALLES NEU PRÜFEN
    // =========================================================

    public void RefreshEffects()
    {
        FindActivePlayer();

        CheckAllEffects();
    }


    // =========================================================
    // DEBUG GIZMOS
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (categories == null)
            return;

        foreach (EffectCategory category in categories)
        {
            if (category == null)
                continue;

            if (category.effects == null)
                continue;

            foreach (GameObject effect in category.effects)
            {
                if (effect == null)
                    continue;

                Gizmos.color = Color.green;

                Gizmos.DrawWireSphere(
                    effect.transform.position,
                    category.activationDistance
                );
            }
        }
    }
}