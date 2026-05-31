using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VersionText : MonoBehaviour
{
    private TMP_Text tmpText;
    private Text uiText;
    private RectTransform rectTransform;

    private void Awake()
    {
        CacheComponents();
    }

    private void Start()
    {
        if (VersionManager.Instance != null)
        {
            VersionManager.Instance.UpdateAllVersionTexts();
        }
    }

    private void CacheComponents()
    {
        tmpText = GetComponent<TMP_Text>();
        uiText = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void ApplyVersion(
        string version,
        Color color,
        int fontSize,
        VersionManager.VersionAnchor anchor,
        Vector2 anchoredPosition,
        bool applyStyle
    )
    {
        CacheComponents();

        if (tmpText != null)
        {
            tmpText.text = version;

            if (applyStyle)
            {
                tmpText.color = color;
                tmpText.fontSize = fontSize;
            }
        }
        else if (uiText != null)
        {
            uiText.text = version;

            if (applyStyle)
            {
                uiText.color = color;
                uiText.fontSize = fontSize;
            }
        }
        else
        {
            Debug.LogWarning("VersionText: Kein TMP_Text oder normales UI Text gefunden!");
            return;
        }

        if (applyStyle && rectTransform != null)
        {
            ApplyAnchor(anchor, anchoredPosition);
        }
    }

    private void ApplyAnchor(VersionManager.VersionAnchor anchor, Vector2 anchoredPosition)
    {
        switch (anchor)
        {
            case VersionManager.VersionAnchor.BottomLeft:
                rectTransform.anchorMin = new Vector2(0f, 0f);
                rectTransform.anchorMax = new Vector2(0f, 0f);
                rectTransform.pivot = new Vector2(0f, 0f);
                break;

            case VersionManager.VersionAnchor.BottomRight:
                rectTransform.anchorMin = new Vector2(1f, 0f);
                rectTransform.anchorMax = new Vector2(1f, 0f);
                rectTransform.pivot = new Vector2(1f, 0f);
                break;

            case VersionManager.VersionAnchor.TopLeft:
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);
                break;

            case VersionManager.VersionAnchor.TopRight:
                rectTransform.anchorMin = new Vector2(1f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.pivot = new Vector2(1f, 1f);
                break;

            case VersionManager.VersionAnchor.CenterBottom:
                rectTransform.anchorMin = new Vector2(0.5f, 0f);
                rectTransform.anchorMax = new Vector2(0.5f, 0f);
                rectTransform.pivot = new Vector2(0.5f, 0f);
                break;

            case VersionManager.VersionAnchor.CenterTop:
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                break;
        }

        rectTransform.anchoredPosition = anchoredPosition;
    }
}