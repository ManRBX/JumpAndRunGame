using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject inventoryUI;
    private bool isOpen = false;

    public Dictionary<InventoryItem, int> itemStacks = new Dictionary<InventoryItem, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadInventoryFromPrefs();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isOpen = !isOpen;
            inventoryUI.SetActive(isOpen);

            // Spiel pausieren oder fortsetzen
            GameStateManager.IsGamePaused = isOpen;

            if (isOpen)
                InventoryUI.Instance.RefreshUI();
        }
    }

    public void AddItem(InventoryItem newItem)
    {
        if (itemStacks.ContainsKey(newItem))
            itemStacks[newItem]++;
        else
            itemStacks[newItem] = 1;

        SaveItemToPrefs(newItem, itemStacks[newItem]);

        Debug.Log($"📦 {newItem.itemName} hinzugefügt. Jetzt x{itemStacks[newItem]}");
        InventoryUI.Instance.RefreshUI();
    }

    public void UseItem(InventoryItem item)
    {
        if (!itemStacks.ContainsKey(item)) return;

        item.Use();
        itemStacks[item]--;

        if (itemStacks[item] <= 0)
            itemStacks.Remove(item);

        SaveItemToPrefs(item, itemStacks.ContainsKey(item) ? itemStacks[item] : 0);

        InventoryUI.Instance.RefreshUI();
    }

    private void SaveItemToPrefs(InventoryItem item, int amount)
    {
        string key = $"Inventory_{item.itemName}";
        PlayerPrefs.SetInt(key, amount);
        PlayerPrefsKeyTracker.TrackKey(key);
        PlayerPrefs.Save();
    }

    public void LoadInventoryFromPrefs()
    {
        itemStacks.Clear();

        if (InventoryUI.Instance == null)
        {
            Debug.LogWarning("⚠️ InventoryUI.Instance ist null beim Laden!");
            return;
        }

        foreach (InventorySlot slot in InventoryUI.Instance.slots)
        {
            string key = $"Inventory_{slot.item.itemName}";
            int amount = PlayerPrefs.GetInt(key, 0);

            if (amount > 0)
            {
                itemStacks[slot.item] = amount;
            }
        }

        InventoryUI.Instance.RefreshUI();
    }
}
