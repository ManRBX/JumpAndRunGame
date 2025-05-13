using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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
    public string[] randomLoadingTexts;
    public string[] randomTips;
    public bool disableLoadingScreenAfterLoad = true;

    private float loadingTimer = 0f;
    private string selectedLoadingText = "";

    private MonoBehaviour[] scriptsToPause;

    void Start()
    {
        Cursor.visible = true; // Mauszeiger immer sichtbar machen
        Cursor.lockState = CursorLockMode.None;

        // Musikstatus respektieren
        GameObject musicObj = GameObject.Find("AudioManager"); // ggf. Name anpassen
        if (musicObj != null)
        {
            AudioSource music = musicObj.GetComponent<AudioSource>();
            if (music != null)
            {
                bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
                music.mute = !musicOn;
            }
        }

        SelectRandomLoadingText();
        ShowRandomTip();

        // Nur bestimmte Skripte deaktivieren: PlayerMovement und PlayerShooting
        scriptsToPause = FindObjectsOfType<MonoBehaviour>()
            .Where(s => s != this && s.enabled &&
                        (s.GetType().Name == "PlayerMovement" || s.GetType().Name == "PlayerShooting"))
            .ToArray();

        foreach (var script in scriptsToPause)
        {
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
        if (randomLoadingTexts != null && randomLoadingTexts.Length > 0)
        {
            int randomIndex = Random.Range(0, randomLoadingTexts.Length);
            selectedLoadingText = randomLoadingTexts[randomIndex];

            if (LoadingText != null)
                LoadingText.text = selectedLoadingText;
        }
    }

    void ShowRandomTip()
    {
        if (randomTips != null && randomTips.Length > 0 && TipText != null)
        {
            int randomIndex = Random.Range(0, randomTips.Length);
            TipText.text = randomTips[randomIndex];
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

        foreach (var script in scriptsToPause)
        {
            if (script != null)
                script.enabled = true;
        }

        Debug.Log("✅ Loading abgeschlossen. Spiel läuft weiter.");
    }
}