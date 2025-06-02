using TMPro;
using System;
using UnityEngine.Localization.Components;
using UnityEngine;

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

    [Header("Reset Confirmation UI")]
    public GameObject resetConfirmationPanel;

    [Header("Popup für Rebinding")]
    public GameObject rebindPopupPanel;
    public LocalizeStringEvent rebindPopupLocalizedText;

    [Header("Blocker für alle anderen UI-Elemente")]
    public GameObject inputBlocker;

    [Header("Doppelbelegung-Warnung")]
    public GameObject duplicateWarningPanel;
    public LocalizeStringEvent duplicateWarningLocalizedText;

    private bool waitingForKey = false;
    private string currentAction = "";
    private int skipInputFrame = -1;

    private void Start()
    {
        UpdateKeyLabels();

        if (resetConfirmationPanel != null)
            resetConfirmationPanel.SetActive(false);

        if (rebindPopupPanel != null)
            rebindPopupPanel.SetActive(false);

        if (inputBlocker != null)
            inputBlocker.SetActive(false);

        if (duplicateWarningPanel != null)
            duplicateWarningPanel.SetActive(false);
    }

    // --- Rebind Buttons ---
    public void OnClick_RebindJump() => StartRebind("Jump");
    public void OnClick_RebindLeft() => StartRebind("MoveLeft");
    public void OnClick_RebindRight() => StartRebind("MoveRight");
    public void OnClick_RebindClimbUp() => StartRebind("ClimbUp");
    public void OnClick_RebindClimbDown() => StartRebind("ClimbDown");
    public void OnClick_RebindDropPlatform() => StartRebind("DropPlatform");
    public void OnClick_RebindShoot() => StartRebind("Shoot");

    private void StartRebind(string actionName)
    {
        if (waitingForKey) return;

        waitingForKey = true;
        skipInputFrame = Time.frameCount;
        currentAction = actionName;

        if (inputBlocker != null)
            inputBlocker.SetActive(true);

        if (rebindPopupPanel != null)
        {
            rebindPopupPanel.SetActive(true);
            if (rebindPopupLocalizedText != null)
                rebindPopupLocalizedText.RefreshString();
        }

        Debug.Log($"🎮 Rebinding '{actionName}' – warte auf Tastendruck...");
    }

    private void Update()
    {
        if (!waitingForKey) return;
        if (Time.frameCount == skipInputFrame) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelRebind();
            return;
        }

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                if (IsKeyAlreadyBound(key))
                {
                    Debug.LogWarning($"⚠️ Hinweis: Taste '{key}' ist bereits belegt.");

                    if (duplicateWarningPanel != null)
                        duplicateWarningPanel.SetActive(true);

                    if (duplicateWarningLocalizedText != null)
                        duplicateWarningLocalizedText.RefreshString();
                }

                Debug.Log($"✅ Neue Taste für {currentAction}: {key}");

                if (KeyBindManager.Instance != null)
                    KeyBindManager.Instance.SetKey(currentAction, key);

                waitingForKey = false;
                currentAction = "";
                skipInputFrame = -1;

                if (rebindPopupPanel != null)
                    rebindPopupPanel.SetActive(false);

                if (inputBlocker != null)
                    inputBlocker.SetActive(false);

                UpdateKeyLabels();
                break;
            }
        }
    }

    private bool IsKeyAlreadyBound(KeyCode key)
    {
        if (KeyBindManager.Instance == null) return false;

        foreach (var binding in KeyBindManager.Instance.GetAllBindings())
        {
            if (binding.Value == key)
                return true;
        }
        return false;
    }

    public void CloseDuplicateWarning()
    {
        if (duplicateWarningPanel != null)
            duplicateWarningPanel.SetActive(false);

        if (inputBlocker != null && waitingForKey)
            inputBlocker.SetActive(true);
    }

    private void CancelRebind()
    {
        Debug.Log("⛔️ Rebinding abgebrochen.");
        waitingForKey = false;
        currentAction = "";
        skipInputFrame = -1;

        if (rebindPopupPanel != null)
            rebindPopupPanel.SetActive(false);

        if (inputBlocker != null)
            inputBlocker.SetActive(false);

        UpdateKeyLabels();
    }

    private void UpdateKeyLabels()
    {
        if (KeyBindManager.Instance == null) return;

        jumpKeyLabel.text = "Jump: " + KeyBindManager.Instance.GetKeyCodeForAction("Jump");
        leftKeyLabel.text = "Move Left: " + KeyBindManager.Instance.GetKeyCodeForAction("MoveLeft");
        rightKeyLabel.text = "Move Right: " + KeyBindManager.Instance.GetKeyCodeForAction("MoveRight");
        climbUpKeyLabel.text = "Climb Up: " + KeyBindManager.Instance.GetKeyCodeForAction("ClimbUp");
        climbDownKeyLabel.text = "Climb Down: " + KeyBindManager.Instance.GetKeyCodeForAction("ClimbDown");
        dropPlatformKeyLabel.text = "Drop Platform: " + KeyBindManager.Instance.GetKeyCodeForAction("DropPlatform");
        shootKeyLabel.text = "Shoot: " + KeyBindManager.Instance.GetKeyCodeForAction("Shoot");
    }

    // --- Reset-Menü ---
    public void OnClick_ResetToDefaults()
    {
        if (inputBlocker != null)
            inputBlocker.SetActive(true);

        if (resetConfirmationPanel != null)
            resetConfirmationPanel.SetActive(true);
    }

    public void ConfirmReset()
    {
        if (KeyBindManager.Instance != null)
        {
            KeyBindManager.Instance.ResetToDefaults();
            UpdateKeyLabels();
        }

        if (resetConfirmationPanel != null)
            resetConfirmationPanel.SetActive(false);

        if (inputBlocker != null)
            inputBlocker.SetActive(false);
    }

    public void CancelReset()
    {
        if (resetConfirmationPanel != null)
            resetConfirmationPanel.SetActive(false);

        if (inputBlocker != null)
            inputBlocker.SetActive(false);
    }
}