using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using System.Linq;
using UnityEngine.Audio;

public class LoadingScene : MonoBehaviour
{
    [Header("UI References")]
    public GameObject LoadingScreen;
    public Image LoadingBarFill;
    public Image Spinner;
    public TMP_Text LoadingText;
    public TMP_Text TipText;

    [Header("Settings")]
    public float loadingDuration = 3f;
    public bool disableLoadingScreenAfterLoad = true;

    [Header("Localized Content")]
    [Tooltip("Füge hier Einträge hinzu und klicke dann auf das + Symbol in jedem Eintrag, um Sprachvarianten hinzuzufügen.")]
    public List<LocalizedString> localizedLoadingTexts = new List<LocalizedString>();

    [Tooltip("Füge hier Tipps hinzu und klicke pro Eintrag auf das +, um z. B. Englisch, Deutsch, Österreichisch etc. zu definieren.")]
    public List<LocalizedString> localizedTips = new List<LocalizedString>();

    public enum LoadingTextMode { Sequential, Random }

    [Header("Text Rotation")]
    public LoadingTextMode loadingTextMode = LoadingTextMode.Sequential;

    [Header("🔊 Audio Mixer")]
    public AudioMixer mixer; // Setze im Inspector (z. B. MasterMixer)

    private float loadingTimer = 0f;
    private int lastTextIndex = -1;
    private MonoBehaviour[] scriptsToPause;

    void Start()
    {
        // 🔇 SFX muten (z. B. Coin, UI-Click etc.)
        if (mixer != null)
            mixer.SetFloat("SFXVolume", -80f); // -80 dB = effektiv stumm

        SelectRandomLoadingText();
        ShowRandomTip();

        scriptsToPause = FindObjectsOfType<MonoBehaviour>()
            .Where(s => s != this && s.enabled &&
                (s.GetType().Name == "PlayerMovement" || s.GetType().Name == "PlayerShooting"))
            .ToArray();

        foreach (var script in scriptsToPause)
        {
            if (script != null)
                script.enabled = false;
        }

        StartCoroutine(FakeLoading());
    }

    void Update()
    {
        if (Spinner != null && LoadingScreen.activeSelf)
        {
            Spinner.transform.Rotate(Vector3.forward * -180f * Time.unscaledDeltaTime);
        }
    }

    void SelectRandomLoadingText()
    {
        if (localizedLoadingTexts == null || localizedLoadingTexts.Count == 0 || LoadingText == null)
            return;

        int index = 0;

        if (loadingTextMode == LoadingTextMode.Sequential)
        {
            index = (lastTextIndex + 1) % localizedLoadingTexts.Count;
        }
        else if (loadingTextMode == LoadingTextMode.Random)
        {
            do
            {
                index = Random.Range(0, localizedLoadingTexts.Count);
            } while (index == lastTextIndex && localizedLoadingTexts.Count > 1);
        }

        lastTextIndex = index;

        LocalizedString entry = localizedLoadingTexts[index];

        entry.StringChanged += (value) =>
        {
            LoadingText.text = value;
        };

        entry.RefreshString();
    }

    void ShowRandomTip()
    {
        if (localizedTips != null && localizedTips.Count > 0 && TipText != null)
        {
            int randomIndex = Random.Range(0, localizedTips.Count);
            LocalizedString tip = localizedTips[randomIndex];

            tip.StringChanged += (value) =>
            {
                TipText.text = value;
            };

            tip.RefreshString();
        }
    }

    IEnumerator FakeLoading()
    {
        LoadingScreen.SetActive(true);
        loadingTimer = 0f;

        while (loadingTimer < loadingDuration)
        {
            loadingTimer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(loadingTimer / loadingDuration);

            if (LoadingBarFill != null)
                LoadingBarFill.fillAmount = progress;

            yield return null;
        }

        if (disableLoadingScreenAfterLoad && LoadingScreen != null)
        {
            LoadingScreen.SetActive(false);
        }

        // 🎧 SFX wieder normal
        if (mixer != null)
            mixer.SetFloat("SFXVolume", 0f); // 0 dB = normale Lautstärke

        foreach (var script in scriptsToPause)
        {
            if (script != null)
                script.enabled = true;
        }

        Debug.Log("✅ Loading abgeschlossen. Spiel läuft weiter.");
    }
}
