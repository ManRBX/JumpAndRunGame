using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public PowerUpEffect effect;

    public void Use()
    {
        Debug.Log($"⚡ Power-Up aktiviert: {itemName}");
        effect.ApplyEffect();
    }
}
