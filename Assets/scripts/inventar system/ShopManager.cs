using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ShopEntry
{
    public InventoryItem itemToBuy;
    public int price = 10;
    public Button button;
    public TMP_Text priceText;
}

public class ShopManager : MonoBehaviour
{
    [Header("🛒 Alle Shop-Einträge")]
    public List<ShopEntry> shopEntries = new List<ShopEntry>();

    private void Start()
    {
        foreach (ShopEntry entry in shopEntries)
        {
            if (entry.priceText != null)
                entry.priceText.text = $"{entry.price} Coins";

            if (entry.button != null)
            {
                entry.button.onClick.RemoveAllListeners();

                // Lokale Kopien für den Lambda
                InventoryItem item = entry.itemToBuy;
                int price = entry.price;

                entry.button.onClick.AddListener(() =>
                {
                    TryBuyItem(item, price);
                });
            }
        }
    }

    private void TryBuyItem(InventoryItem item, int price)
    {
        int coins = PlayerPrefs.GetInt("GlobalCoins", 0);

        if (coins >= price)
        {
            // 💰 Coins abziehen
            coins -= price;
            PlayerPrefs.SetInt("GlobalCoins", coins);
            PlayerPrefsKeyTracker.TrackKey("GlobalCoins");

            // 🎁 PowerUp um +1 erhöhen
            string key = $"Inventory_{item.itemName}";
            int currentAmount = PlayerPrefs.GetInt(key, 0);
            PlayerPrefs.SetInt(key, currentAmount + 1);
            PlayerPrefsKeyTracker.TrackKey(key);

            PlayerPrefs.Save();

            // ⟳ UI aktualisieren
            InventoryManager.Instance.LoadInventoryFromPrefs();

            Debug.Log($"✅ {item.itemName} gekauft! Jetzt x{currentAmount + 1} | Neue Coins: {coins}");
        }
        else
        {
            Debug.Log("❌ Nicht genug Coins!");
        }
    }
}
