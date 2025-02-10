using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text tutorialText;  // Textfeld für das Tutorial
    public string header = "STEUERUNG (E zum Aufklappen)";  // Überschrift
    private bool isExpanded = false;  // Status: Ausgeklappt oder nicht

    private string fullText = "- A / D → Bewegen\n" +
                              "- W / S → Klettern\n" +
                              "- SPACE → Springen\n" +
                              "- Linke Maustaste → Schießen";

    void Start()
    {
        // Setze nur die Überschrift am Anfang
        if (tutorialText != null)
        {
            tutorialText.text = header;
        }
        else
        {
            Debug.LogWarning("Kein TMP_Text zugewiesen!");
        }
    }

    void Update()
    {
        // Mit "E" auf- und zuklappen
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleTutorialText();
        }
    }

    void ToggleTutorialText()
    {
        if (tutorialText != null)
        {
            if (isExpanded)
            {
                // Nur Überschrift anzeigen (eingeklappt)
                tutorialText.text = header;
            }
            else
            {
                // Vollständige Steuerung anzeigen (ausgeklappt)
                tutorialText.text = header + "\n" + fullText;
            }
            isExpanded = !isExpanded;  // Status umkehren
        }
    }
}
