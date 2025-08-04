using UnityEngine;
using UnityEngine.UI;

public class KeyPickup : MonoBehaviour
{
    [Header("🔑 Schlüsselname")]
    public string keyName;

    [Header("🔊 Einsammel-Sound")]
    public AudioSource pickupSound;

    [Header("🔮 Pickup Effekt")]
    public GameObject pickupEffect;

    [Header("🖼️ Schlüssel UI-Image")]
    public Image keyImage;

    private void Start()
    {
        if (PlayerPrefs.GetInt(keyName, 0) == 1)
        {
            if (keyImage != null)
                keyImage.enabled = true;

            Destroy(gameObject);
        }
        else
        {
            if (keyImage != null)
                keyImage.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetInt(keyName, 1);
            PlayerPrefs.Save();

            if (keyImage != null)
                keyImage.enabled = true;

            if (pickupSound != null)
                pickupSound.Play();

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}