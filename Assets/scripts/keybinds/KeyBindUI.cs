using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Ein UI-Skript, das Buttons & Labels für alle Actions (Jump, MoveLeft, MoveRight, usw.) enthält.
/// So kannst du sie mit einem Klick neu belegen, indem das Skript auf den nächsten Tastendruck wartet.
/// </summary>
public class KeyBindUI : MonoBehaviour
{
    [Header("UI-Labels für Aktionen")]

    public TMP_Text jumpKeyLabel;
    public TMP_Text leftKeyLabel;
    public TMP_Text rightKeyLabel;
    public TMP_Text climbUpKeyLabel;
    public TMP_Text climbDownKeyLabel;
    public TMP_Text dropPlatformKeyLabel;
    public TMP_Text shootKeyLabel;

    private bool waitingForKey = false;
    private string currentAction = "";

    private void Start()
    {
        // Direkt beim Start die Labels aktualisieren, damit der Spieler sieht,
        // welche Tasten derzeit gebunden sind.
        UpdateKeyLabels();
    }

    // -------------------------------------------------------------------
    // Diese Button-Methoden rufst du je im OnClick der Buttons im Inspector auf:
    //   "Rebind Jump"          -> OnClick_RebindJump()
    //   "Rebind Left"          -> OnClick_RebindLeft()
    //   "Rebind Right"         -> OnClick_RebindRight()
    //   "Rebind ClimbUp"       -> OnClick_RebindClimbUp()
    //   "Rebind ClimbDown"     -> OnClick_RebindClimbDown()
    //   "Rebind DropPlatform"  -> OnClick_RebindDropPlatform()
    //   "Rebind Shoot"         -> OnClick_RebindShoot()
    // -------------------------------------------------------------------

    public void OnClick_RebindJump()
    {
        StartRebind("Jump", jumpKeyLabel);
    }

    public void OnClick_RebindLeft()
    {
        StartRebind("MoveLeft", leftKeyLabel);
    }

    public void OnClick_RebindRight()
    {
        StartRebind("MoveRight", rightKeyLabel);
    }

    public void OnClick_RebindClimbUp()
    {
        StartRebind("ClimbUp", climbUpKeyLabel);
    }

    public void OnClick_RebindClimbDown()
    {
        StartRebind("ClimbDown", climbDownKeyLabel);
    }

    public void OnClick_RebindDropPlatform()
    {
        StartRebind("DropPlatform", dropPlatformKeyLabel);
    }

    public void OnClick_RebindShoot()
    {
        StartRebind("Shoot", shootKeyLabel);
    }

    /// <summary>
    /// Zentraler Einstieg fürs Neu-Belegen einer Aktion.
    /// Ich lege fest, welche Aktion geändert wird und welches Label
    /// währenddessen "Drücke jetzt eine Taste..." anzeigen soll.
    /// </summary>
    private void StartRebind(string actionName, TMP_Text labelToChange)
    {
        if (waitingForKey)
        {
            Debug.Log("Warte bereits auf einen Tastendruck – bitte erst eine Taste drücken!");
            return;
        }

        currentAction = actionName;
        waitingForKey = true;

        if (labelToChange != null)
        {
            labelToChange.text = "Drücke jetzt eine Taste...";
        }

        Debug.Log($"Rebind '{actionName}': Bitte eine Taste drücken...");
    }

    private void Update()
    {
        if (!waitingForKey) return;

        // Durchlaufe alle KeyCodes und prüfe, ob einer gedrückt wurde
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                Debug.Log($"Neue Taste für {currentAction}: {key}");

                // An den KeyBindManager übergeben, falls vorhanden
                if (KeyBindManager.Instance != null)
                {
                    KeyBindManager.Instance.SetKey(currentAction, key);
                }

                // Labels aktualisieren
                UpdateKeyLabels();

                // Warten beenden
                waitingForKey = false;
                currentAction = "";
                break;
            }
        }
    }

    /// <summary>
    /// Aktualisiert die UI-Labels, damit der Spieler sieht,
    /// welche Tasten aktuell gebunden sind.
    /// </summary>
    private void UpdateKeyLabels()
    {
        if (KeyBindManager.Instance == null) return;

        // Jump
        if (jumpKeyLabel != null)
        {
            jumpKeyLabel.text = "Sprung: " +
                KeyBindManager.Instance.GetKeyCodeForAction("Jump");
        }

        // Left
        if (leftKeyLabel != null)
        {
            leftKeyLabel.text = "Links: " +
                KeyBindManager.Instance.GetKeyCodeForAction("MoveLeft");
        }

        // Right
        if (rightKeyLabel != null)
        {
            rightKeyLabel.text = "Rechts: " +
                KeyBindManager.Instance.GetKeyCodeForAction("MoveRight");
        }

        // ClimbUp
        if (climbUpKeyLabel != null)
        {
            climbUpKeyLabel.text = "Hoch: " +
                KeyBindManager.Instance.GetKeyCodeForAction("ClimbUp");
        }

        // ClimbDown
        if (climbDownKeyLabel != null)
        {
            climbDownKeyLabel.text = "Runter: " +
                KeyBindManager.Instance.GetKeyCodeForAction("ClimbDown");
        }

        // DropPlatform
        if (dropPlatformKeyLabel != null)
        {
            dropPlatformKeyLabel.text = "Durchfallen: " +
                KeyBindManager.Instance.GetKeyCodeForAction("DropPlatform");
        }

        // Shoot
        if (shootKeyLabel != null)
        {
            shootKeyLabel.text = "Schießen: " +
                KeyBindManager.Instance.GetKeyCodeForAction("Shoot");
        }
    }
}
