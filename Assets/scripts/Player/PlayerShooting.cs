using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public int maxAmmo = 60;
    private int currentAmmo;

    [Header("Cooldown Settings")]
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("UI Settings")]
    public TMP_Text ammoText;
    public TMP_Text shotText;

    void Start()
    {
        currentAmmo = PlayerPrefs.GetInt("GlobalAmmo", maxAmmo);
        PlayerPrefsKeyTracker.TrackKey("GlobalAmmo");

        int shotsFired = PlayerPrefs.GetInt("ShotsFired", 0);
        PlayerPrefsKeyTracker.TrackKey("ShotsFired");

        UpdateAmmoUI();
        UpdateShotsUI(shotsFired);
    }

    void Update()
    {
        if (GameStateManager.IsGamePaused)
            return;

        KeyCode shootKey = KeyBindManager.Instance?.GetKeyCodeForAction("Shoot") ?? KeyCode.Mouse0;

        if (Input.GetKeyDown(shootKey) && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
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

        currentAmmo--;
        PlayerPrefs.SetInt("GlobalAmmo", currentAmmo);
        PlayerPrefsKeyTracker.TrackKey("GlobalAmmo");

        int shotsFired = PlayerPrefs.GetInt("ShotsFired", 0) + 1;
        PlayerPrefs.SetInt("ShotsFired", shotsFired);
        PlayerPrefsKeyTracker.TrackKey("ShotsFired");

        PlayerPrefs.Save();

        UpdateAmmoUI();
        UpdateShotsUI(shotsFired);

        Debug.Log($"Shot fired! Total: {shotsFired} | Remaining ammo: {currentAmmo}");
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString();
        }
    }

    void UpdateShotsUI(int shots)
    {
        if (shotText != null)
        {
            shotText.text = shots.ToString();
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }

        PlayerPrefs.SetInt("GlobalAmmo", currentAmmo);
        PlayerPrefsKeyTracker.TrackKey("GlobalAmmo");
        PlayerPrefs.Save();

        UpdateAmmoUI();
        Debug.Log($"Ammo reloaded! Current: {currentAmmo}");
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}
