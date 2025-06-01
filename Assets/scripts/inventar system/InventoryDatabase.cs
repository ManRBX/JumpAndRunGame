using System.Collections.Generic;
using UnityEngine;

public class InventoryDatabase : MonoBehaviour
{
    public static List<InventoryItem> AllItems = new List<InventoryItem>();

    public List<InventoryItem> itemsInGame = new List<InventoryItem>();

    private void Awake()
    {
        AllItems = itemsInGame;
    }
}
