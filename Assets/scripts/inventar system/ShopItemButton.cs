using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemButton : MonoBehaviour
{
    public InventoryItem itemToBuy;
    public int price = 10;
    public TMP_Text priceText;

    private void Start()
    {
        if (priceText != null)
            priceText.text = $"{price} Coins";

        GetComponent<Button>().onClick.AddListener(BuyItem);
    }

    private void BuyItem()
    {
        int currentCoins = PlayerPrefs.GetInt("GlobalCoins", 0);

        if (currentCoins >= price)
        {
            // 👛 Coins abziehen
            currentCoins -= price;
            PlayerPrefs.SetInt("GlobalCoins", currentCoins);
            PlayerPrefsKeyTracker.TrackKey("GlobalCoins");
            PlayerPrefs.Save();

            // 🧠 Item zum Inventar hinzufügen
            InventoryManager.Instance.AddItem(itemToBuy);

            Debug.Log($"✅ {itemToBuy.itemName} gekauft!");
        }
        else
        {
            Debug.Log("❌ Nicht genug Coins!");
        }
    }
}
