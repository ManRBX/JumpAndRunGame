using UnityEngine;
using TMPro;
using System;

/// <summary>
/// A UI script that contains buttons and labels for all actions (Jump, MoveLeft, MoveRight, etc.).
/// This allows you to rebind keys by waiting for the next key press when you click the button.
/// </summary>
public class KeyBindUI : MonoBehaviour
{
    [Header("UI Labels for Actions")]

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
        // Update the labels at the start so the player can see the current key bindings
        UpdateKeyLabels();
    }

    // -------------------------------------------------------------------
    // These button methods are called via the OnClick in the Inspector:
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
    /// Central entry point for rebinding an action.
    /// Specifies which action will be changed and which label will display "Press a key now...".
    /// </summary>
    private void StartRebind(string actionName, TMP_Text labelToChange)
    {
        if (waitingForKey)
        {
            Debug.Log("Already waiting for a key press – please press a key first!");
            return;
        }

        currentAction = actionName;
        waitingForKey = true;

        if (labelToChange != null)
        {
            labelToChange.text = "Press a key now...";
        }

        Debug.Log($"Rebind '{actionName}': Please press a key...");
    }

    private void Update()
    {
        if (!waitingForKey) return;

        // Loop through all KeyCodes and check if one was pressed
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                Debug.Log($"New key for {currentAction}: {key}");

                // Pass to the KeyBindManager if available
                if (KeyBindManager.Instance != null)
                {
                    KeyBindManager.Instance.SetKey(currentAction, key);
                }

                // Update the labels
                UpdateKeyLabels();

                // Stop waiting for key press
                waitingForKey = false;
                currentAction = "";
                break;
            }
        }
    }

    /// <summary>
    /// Updates the UI labels to show the currently bound keys.
    /// </summary>
    private void UpdateKeyLabels()
    {
        if (KeyBindManager.Instance == null) return;

        // Jump
        if (jumpKeyLabel != null)
        {
            jumpKeyLabel.text = "Jump: " +
                KeyBindManager.Instance.GetKeyCodeForAction("Jump");
        }

        // Left
        if (leftKeyLabel != null)
        {
            leftKeyLabel.text = "Move Left: " +
                KeyBindManager.Instance.GetKeyCodeForAction("MoveLeft");
        }

        // Right
        if (rightKeyLabel != null)
        {
            rightKeyLabel.text = "Move Right: " +
                KeyBindManager.Instance.GetKeyCodeForAction("MoveRight");
        }

        // ClimbUp
        if (climbUpKeyLabel != null)
        {
            climbUpKeyLabel.text = "Climb Up: " +
                KeyBindManager.Instance.GetKeyCodeForAction("ClimbUp");
        }

        // ClimbDown
        if (climbDownKeyLabel != null)
        {
            climbDownKeyLabel.text = "Climb Down: " +
                KeyBindManager.Instance.GetKeyCodeForAction("ClimbDown");
        }

        // DropPlatform
        if (dropPlatformKeyLabel != null)
        {
            dropPlatformKeyLabel.text = "Drop Platform: " +
                KeyBindManager.Instance.GetKeyCodeForAction("DropPlatform");
        }

        // Shoot
        if (shootKeyLabel != null)
        {
            shootKeyLabel.text = "Shoot: " +
                KeyBindManager.Instance.GetKeyCodeForAction("Shoot");
        }
    }
}
