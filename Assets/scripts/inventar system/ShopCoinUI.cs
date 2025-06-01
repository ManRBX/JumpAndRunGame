using UnityEngine;
using TMPro;

public class ShopCoinUI : MonoBehaviour
{
    public TMP_Text coinText;

    private void Update()
    {
        int coins = PlayerPrefs.GetInt("GlobalCoins", 0);
        if (coinText != null)
            coinText.text = coins.ToString();
    }
}
