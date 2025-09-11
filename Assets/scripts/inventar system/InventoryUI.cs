using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InventorySlot
{
    public InventoryItem item;
    public Button button;
    public TMP_Text text;
    public Image icon;
}

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("📦 Inventar Slots")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    [Header("🧾 Inventar Panel")]
    [SerializeField] private GameObject inventoryUI;

    [Header("🎮 Tastenzuweisung")]
    public TMP_Text inventoryKeyLabel;

    private bool isOpen = false;
    private bool canUsePowerUp = true;
    private InventoryItem activePowerUpItem;

    private KeyBindManager keyBindManager;

    // 🔄 Globale Cooldown-Steuerung
    private bool isGlobalCooldown = false;
    private float cooldownDuration = 20f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        keyBindManager = KeyBindManager.Instance;

        if (inventoryUI != null)
            inventoryUI.SetActive(false);

        UpdateInventoryKeyLabel();
    }

    private void Update()
    {
        if (keyBindManager == null) return;

        HandleInventoryToggle();
        UpdateInventoryKeyLabel();
    }

    private void HandleInventoryToggle()
    {
        KeyCode inventoryKey = keyBindManager.GetKeyCodeForAction("OpenInventory");

        if (inventoryKey != KeyCode.None && Input.GetKeyDown(inventoryKey))
        {
            isOpen = !isOpen;

            if (inventoryUI != null)
                inventoryUI.SetActive(isOpen);

            GameStateManager.IsGamePaused = isOpen;

            if (isOpen)
                RefreshUI();
        }
    }

    private void UpdateInventoryKeyLabel()
    {
        if (inventoryKeyLabel != null)
        {
            KeyCode key = keyBindManager.GetKeyCodeForAction("OpenInventory");
            inventoryKeyLabel.text = "Inventar: " + key;
        }
    }

    public void RefreshUI()
    {
        foreach (var slot in slots)
        {
            if (slot.item == null) continue;

            InventoryManager.Instance.itemStacks.TryGetValue(slot.item, out int amount);

            if (slot.text != null)
                slot.text.text = $"{slot.item.itemName} x{amount}";

            if (slot.icon != null && slot.item.icon != null)
                slot.icon.sprite = slot.item.icon;

            if (slot.button != null)
            {
                bool canUse = amount > 0 && CanUseThisItem(slot.item);
                slot.button.interactable = canUse;

                slot.button.onClick.RemoveAllListeners();

                if (canUse)
                {
                    InventoryItem capturedItem = slot.item;
                    Button capturedButton = slot.button;

                    slot.button.onClick.AddListener(() =>
                    {
                        UseItemWithRules(capturedItem, capturedButton);
                    });
                }
            }
        }
    }

    private bool CanUseThisItem(InventoryItem item)
    {
        if (item == null) return false;

        string name = item.itemName.ToLower();

        if (name.Contains("ammo"))
        {
            var shooter = FindObjectOfType<PlayerShooting>();
            if (shooter != null)
            {
                int currentAmmo = PlayerPrefs.GetInt("GlobalAmmo", 0);
                return currentAmmo < shooter.maxAmmo;
            }

            return false;
        }

        if (name.Contains("health"))
            return true;

        // alle anderen PowerUps nur wenn kein Global-Cooldown läuft
        return canUsePowerUp && !isGlobalCooldown;
    }

    private void UseItemWithRules(InventoryItem item, Button buttonToDisable)
    {
        if (item == null) return;

        string name = item.itemName.ToLower();
        bool isHealthOrAmmo = name.Contains("health") || name.Contains("ammo");

        if (!isHealthOrAmmo)
        {
            if (!canUsePowerUp && item != activePowerUpItem)
                return;

            // 🚫 Global Cooldown starten
            isGlobalCooldown = true;
            canUsePowerUp = false;
            activePowerUpItem = item;

            DisableAllButtons();
            StartCoroutine(GlobalCooldownRoutine());
        }

        InventoryManager.Instance.UseItem(item);
        RefreshUI();
    }

    // ❌ Alle Buttons deaktivieren
    private void DisableAllButtons()
    {
        foreach (var slot in slots)
        {
            if (slot.button != null)
                slot.button.interactable = false;
        }
    }

    // ⏱️ 20 Sekunden warten, dann wieder freigeben
    private IEnumerator GlobalCooldownRoutine()
    {
        yield return new WaitForSeconds(cooldownDuration);
        isGlobalCooldown = false;
        canUsePowerUp = true;
        activePowerUpItem = null;
        RefreshUI();
    }

    public void TryUseSlot(int index)
    {
        if (index < 0 || index >= slots.Count) return;

        var slot = slots[index];
        if (slot != null && CanUseThisItem(slot.item))
        {
            UseItemWithRules(slot.item, slot.button);
        }
    }
}
