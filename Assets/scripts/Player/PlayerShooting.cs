using UnityEngine;
using TMPro;  // For TextMeshPro (shot count in UI)

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public int maxAmmo = 30; // Global maximum ammo
    private int currentAmmo;

    [Header("Cooldown Settings")]
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("UI Settings")]
    public TMP_Text ammoText;  // UI for ammo count
    public TMP_Text shotText;  // UI for total shots fired

    private void Start()
    {
        // Load global ammo from PlayerPrefs or set default value
        currentAmmo = PlayerPrefs.GetInt("GlobalAmmo", maxAmmo);

        // Load total shots fired from PlayerPrefs
        int shotsFired = PlayerPrefs.GetInt("ShotsFired", 0);

        UpdateAmmoUI();
        UpdateShotsUI(shotsFired);
    }

    void Update()
    {
        // Check if the player can shoot (KeyBindManager or default key)
        KeyCode shootKey = KeyBindManager.Instance?.GetKeyCodeForAction("Shoot") ?? KeyCode.Mouse0;

        if (Input.GetKeyDown(shootKey) && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Shooting logic
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        bullet.transform.Rotate(0f, 0f, 90f);
        if (transform.localScale.x < 0)
        {
            bullet.transform.Rotate(0f, 0f, 180f);
            rb.AddForce(-firePoint.right * bulletForce, ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(firePoint.right * bulletForce, ForceMode2D.Impulse);
        }

        // Reduce ammo and save to PlayerPrefs
        currentAmmo--;
        PlayerPrefs.SetInt("GlobalAmmo", currentAmmo);

        // Increase shots fired count and save to PlayerPrefs
        int shotsFired = PlayerPrefs.GetInt("ShotsFired", 0) + 1;
        PlayerPrefs.SetInt("ShotsFired", shotsFired);

        PlayerPrefs.Save();

        UpdateAmmoUI();
        UpdateShotsUI(shotsFired);

        Debug.Log("Shot fired! Total: " + shotsFired + " | Remaining ammo: " + currentAmmo);
    }

    // Updates the UI for ammo count
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString(); // Display only the number
        }
    }

    // Updates the UI for total shots fired
    void UpdateShotsUI(int shots)
    {
        if (shotText != null)
        {
            shotText.text = shots.ToString(); // Display only the number
        }
    }

    // Reloads ammo (e.g., from pickups)
    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }

        // Save new ammo count
        PlayerPrefs.SetInt("GlobalAmmo", currentAmmo);
        PlayerPrefs.Save();

        UpdateAmmoUI();
        Debug.Log("Ammo reloaded! Current: " + currentAmmo);
    }
}
