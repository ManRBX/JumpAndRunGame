using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public GameObject LoadingScreen;
    public Image LoadingBarFill;
    public Image Spinner;
    public TMP_Text LoadingText;
    public TMP_Text TipText;

    public float loadingDuration = 3f;          // Dauer des Fake-Loadings in Sekunden
    public string[] randomLoadingTexts;         // Deine Lade-Texte
    public string[] randomTips;                 // Deine Tipps
    public bool disableLoadingScreenAfterLoad = true;

    private float loadingTimer = 0f;
    private string selectedLoadingText = "";

    void Start()
    {
        SelectRandomLoadingText();
        ShowRandomTip();
        StartCoroutine(FakeLoading());
    }

    void Update()
    {
        // Spinner rotiert immer wenn LoadingScreen aktiv ist
        if (Spinner != null && LoadingScreen.activeSelf)
        {
            Spinner.transform.Rotate(Vector3.forward * -180f * Time.deltaTime);
        }
    }

    void SelectRandomLoadingText()
    {
        if (randomLoadingTexts != null && randomLoadingTexts.Length > 0)
        {
            int randomIndex = Random.Range(0, randomLoadingTexts.Length);
            selectedLoadingText = randomLoadingTexts[randomIndex];

            if (LoadingText != null)
                LoadingText.text = selectedLoadingText; // <-- Text wird EINMAL gesetzt, keine Punkte, nix
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
            loadingTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(loadingTimer / loadingDuration);

            if (LoadingBarFill != null)
                LoadingBarFill.fillAmount = progress;

            yield return null;
        }

        if (disableLoadingScreenAfterLoad && LoadingScreen != null)
        {
            LoadingScreen.SetActive(false);
        }
    }
}
