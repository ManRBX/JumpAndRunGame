using TMPro;
using System;
using UnityEngine;
using UnityEngine.Localization.Components;

public class KeyBindUI : MonoBehaviour
{
    [Header("📝 Text für Rebind-Popup")]
    public TMP_Text rebindPopupLabel; // Das echte Textfeld
    public string waitingText = "Bitte drücken Sie eine Taste...";

    [Header("🔤 UI Labels")]
    public TMP_Text jumpLabel;
    public TMP_Text leftLabel;
    public TMP_Text rightLabel;
    public TMP_Text climbUpLabel;
    public TMP_Text climbDownLabel;
    public TMP_Text dropPlatformLabel;
    public TMP_Text shootLabel;
    public TMP_Text inventoryLabel;
    public TMP_Text doorLabel;
    public TMP_Text slot2Label;
    public TMP_Text slot3Label;
    public TMP_Text slot4Label;

    [Header("🔁 Rebind Popup")]
    public GameObject rebindPopup;
    public LocalizeStringEvent rebindPopupText;

    [Header("🛡️ Input Blocker während Rebind")]
    public GameObject inputBlocker;

    [Header("⚠️ Doppelbelegung")]
    public GameObject duplicateWarning;
    public LocalizeStringEvent duplicateWarningText;

    [Header("🧼 Reset Confirmation")]
    public GameObject resetConfirmation;

    private string currentAction = "";
    private bool waitingForKey = false;
    private int skipFrame = -1;

    private void Start()
    {
        UpdateLabels();

        if (rebindPopup != null) rebindPopup.SetActive(false);
        if (duplicateWarning != null) duplicateWarning.SetActive(false);
        if (inputBlocker != null) inputBlocker.SetActive(false);
        if (resetConfirmation != null) resetConfirmation.SetActive(false);
    }

    private void Update()
    {
        if (!waitingForKey || Time.frameCount == skipFrame) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelRebind();
            return;
        }

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                if (IsKeyAlreadyUsed(key))
                {
                    Debug.LogWarning($"⚠️ Taste '{key}' ist bereits belegt!");
                    if (duplicateWarning != null) duplicateWarning.SetActive(true);
                    if (duplicateWarningText != null) duplicateWarningText.RefreshString();
                }

                KeyBindManager.Instance.SetKey(currentAction, key);
                FinishRebind();
                break;
            }
        }
    }

    // 🔁 Rebind-Trigger Methoden
    public void Rebind_Jump() => StartRebind("Jump");
    public void Rebind_Left() => StartRebind("MoveLeft");
    public void Rebind_Right() => StartRebind("MoveRight");
    public void Rebind_ClimbUp() => StartRebind("ClimbUp");
    public void Rebind_ClimbDown() => StartRebind("ClimbDown");
    public void Rebind_DropPlatform() => StartRebind("DropPlatform");
    public void Rebind_Shoot() => StartRebind("Shoot");
    public void Rebind_Inventory() => StartRebind("OpenInventory");
    public void Rebind_Door() => StartRebind("OpenDoor");
    public void Rebind_Slot2() => StartRebind("UseSlot2");
    public void Rebind_Slot3() => StartRebind("UseSlot3");
    public void Rebind_Slot4() => StartRebind("UseSlot4");

    private void StartRebind(string action)
    {
        if (waitingForKey) return;

        waitingForKey = true;
        skipFrame = Time.frameCount;
        currentAction = action;

        if (inputBlocker != null) inputBlocker.SetActive(true);

        if (rebindPopup != null)
        {
            rebindPopup.SetActive(true); // ✅ Popup sichtbar machen
            if (rebindPopupLabel != null)
                rebindPopupLabel.text = waitingText;
        }

        Debug.Log($"🎮 Warte auf neue Taste für '{action}'...");
    }

    private void CancelRebind()
    {
        Debug.Log("⛔ Rebind abgebrochen.");
        waitingForKey = false;
        currentAction = "";
        skipFrame = -1;

        if (inputBlocker != null) inputBlocker.SetActive(false);
        if (rebindPopup != null) rebindPopup.SetActive(false); // ❌ Popup ausblenden
    }

    private void FinishRebind()
    {
        Debug.Log($"✅ Neue Taste gesetzt für '{currentAction}'");
        waitingForKey = false;
        currentAction = "";
        skipFrame = -1;

        if (inputBlocker != null) inputBlocker.SetActive(false);
        if (rebindPopup != null) rebindPopup.SetActive(false); // ❌ Popup ausblenden

        UpdateLabels();
    }

    private bool IsKeyAlreadyUsed(KeyCode key)
    {
        foreach (var pair in KeyBindManager.Instance.GetAllBindings())
        {
            if (pair.Value == key) return true;
        }
        return false;
    }

    public void CloseDuplicateWarning()
    {
        if (duplicateWarning != null)
            duplicateWarning.SetActive(false);

        if (waitingForKey && inputBlocker != null)
            inputBlocker.SetActive(true);
    }

    // 🧼 Reset
    public void ResetToDefaults()
    {
        if (resetConfirmation != null)
            resetConfirmation.SetActive(true);

        if (inputBlocker != null)
            inputBlocker.SetActive(true);
    }

    public void ConfirmReset()
    {
        KeyBindManager.Instance.ResetToDefaults();
        UpdateLabels();

        if (resetConfirmation != null)
            resetConfirmation.SetActive(false);

        if (inputBlocker != null)
            inputBlocker.SetActive(false);
    }

    public void CancelReset()
    {
        if (resetConfirmation != null)
            resetConfirmation.SetActive(false);

        if (inputBlocker != null)
            inputBlocker.SetActive(false);
    }

    private void UpdateLabels()
    {
        var mgr = KeyBindManager.Instance;
        if (mgr == null) return;

        if (jumpLabel != null) jumpLabel.text = "Jump: " + mgr.GetKeyCodeForAction("Jump");
        if (leftLabel != null) leftLabel.text = "Move Left: " + mgr.GetKeyCodeForAction("MoveLeft");
        if (rightLabel != null) rightLabel.text = "Move Right: " + mgr.GetKeyCodeForAction("MoveRight");
        if (climbUpLabel != null) climbUpLabel.text = "Climb Up: " + mgr.GetKeyCodeForAction("ClimbUp");
        if (climbDownLabel != null) climbDownLabel.text = "Climb Down: " + mgr.GetKeyCodeForAction("ClimbDown");
        if (dropPlatformLabel != null) dropPlatformLabel.text = "Drop Platform: " + mgr.GetKeyCodeForAction("DropPlatform");
        if (shootLabel != null) shootLabel.text = "Shoot: " + mgr.GetKeyCodeForAction("Shoot");
        if (inventoryLabel != null) inventoryLabel.text = "Inventar: " + mgr.GetKeyCodeForAction("OpenInventory");
        if (doorLabel != null) doorLabel.text = "Door: " + mgr.GetKeyCodeForAction("OpenDoor");
        if (slot2Label != null) slot2Label.text = "Slot 2: " + mgr.GetKeyCodeForAction("UseSlot2");
        if (slot3Label != null) slot3Label.text = "Slot 3: " + mgr.GetKeyCodeForAction("UseSlot3");
        if (slot4Label != null) slot4Label.text = "Slot 4: " + mgr.GetKeyCodeForAction("UseSlot4");
    }
}
