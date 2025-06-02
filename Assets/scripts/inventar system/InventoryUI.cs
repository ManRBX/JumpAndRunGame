using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InventorySlot
{
    public InventoryItem item;           // z. B. SpeedBoost
    public Button button;                // Button im UI
    public TMP_Text text;                // Text „SpeedBoost x5“
    public Image icon;                   // Icon
}

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [Header("Slots manuell zuweisbar")]
    public List<InventorySlot> slots = new List<InventorySlot>();

    private bool canUsePowerUp = true;
    private InventoryItem activePowerUpItem;

    private bool isOpen = false;
    [SerializeField] private GameObject inventoryUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isOpen = !isOpen;
            inventoryUI.SetActive(isOpen);
            GameStateManager.IsGamePaused = isOpen;

            if (isOpen)
                RefreshUI();
        }
    }

    public void RefreshUI()
    {
        foreach (var slot in slots)
        {
            InventoryManager.Instance.itemStacks.TryGetValue(slot.item, out int amount);

            // Text aktualisieren
            if (slot.text != null)
                slot.text.text = $"{slot.item.itemName} x{amount}";

            // Icon aktualisieren
            if (slot.icon != null && slot.item.icon != null)
                slot.icon.sprite = slot.item.icon;

            // Button-Interaktion
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
        string name = item.itemName.ToLower();

        if (name.Contains("ammo"))
        {
            PlayerShooting shooter = GameObject.FindObjectOfType<PlayerShooting>();
            if (shooter != null)
            {
                int currentAmmo = PlayerPrefs.GetInt("GlobalAmmo", 0);
                return currentAmmo < shooter.maxAmmo;
            }
            return false;
        }

        if (name.Contains("health"))
            return true;

        return canUsePowerUp || item == activePowerUpItem;
    }

    private void UseItemWithRules(InventoryItem item, Button buttonToDisable)
    {
        string name = item.itemName.ToLower();
        bool isHealthOrAmmo = name.Contains("health") || name.Contains("ammo");

        if (!isHealthOrAmmo)
        {
            if (!canUsePowerUp && item != activePowerUpItem)
                return;

            if (!canUsePowerUp && item == activePowerUpItem)
                return;

            canUsePowerUp = false;
            activePowerUpItem = item;

            if (buttonToDisable != null)
                buttonToDisable.interactable = false;

            StartCoroutine(PowerUpCooldown());
        }

        InventoryManager.Instance.UseItem(item);
        RefreshUI();
    }

    private IEnumerator PowerUpCooldown()
    {
        yield return new WaitForSeconds(10f);
        canUsePowerUp = true;
        activePowerUpItem = null;
        RefreshUI();
    }
}
