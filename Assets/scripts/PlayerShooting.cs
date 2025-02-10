using UnityEngine;
using TMPro;  // Für TextMeshPro (Schussanzahl im UI)

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public int maxAmmo = 30; // Globale maximale Munition
    private int currentAmmo;

    [Header("Cooldown Settings")]
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("UI Settings")]
    public TMP_Text ammoText;  // UI für Munition
    public TMP_Text shotText;  // UI für Schüsse

    private void Start()
    {
        // Lade globale Munition aus PlayerPrefs oder setze Standardwert
        currentAmmo = PlayerPrefs.GetInt("GlobalAmmo", maxAmmo);

        // Lade Schussanzahl aus PlayerPrefs
        int shotsFired = PlayerPrefs.GetInt("ShotsFired", 0);

        UpdateAmmoUI();
        UpdateShotsUI(shotsFired);
    }

    void Update()
    {
        // Prüfe, ob der Spieler schießen kann (KeyBindManager oder Standard-Taste)
        KeyCode shootKey = KeyBindManager.Instance?.GetKeyCodeForAction("Shoot") ?? KeyCode.Mouse0;

        if (Input.GetKeyDown(shootKey) && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Schuss-Logik
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

        // Munition reduzieren und speichern
        currentAmmo--;
        PlayerPrefs.SetInt("GlobalAmmo", currentAmmo);

        // Schüsse zählen und speichern
        int shotsFired = PlayerPrefs.GetInt("ShotsFired", 0) + 1;
        PlayerPrefs.SetInt("ShotsFired", shotsFired);

        PlayerPrefs.Save();

        UpdateAmmoUI();
        UpdateShotsUI(shotsFired);

        Debug.Log("Schuss abgefeuert! Gesamt: " + shotsFired + " | Verbleibende Munition: " + currentAmmo);
    }

    // Aktualisiert die UI für Munition
    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString(); // Zeigt nur die Zahl an
        }
    }

    // Aktualisiert die UI für Schüsse
    void UpdateShotsUI(int shots)
    {
        if (shotText != null)
        {
            shotText.text = shots.ToString(); // Zeigt nur die Zahl an
        }
    }

    // Munition aufladen (z. B. durch Items)
    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo)
        {
            currentAmmo = maxAmmo;
        }

        // Speichere neue Munition
        PlayerPrefs.SetInt("GlobalAmmo", currentAmmo);
        PlayerPrefs.Save();

        UpdateAmmoUI();
        Debug.Log("Munition aufgeladen! Aktuell: " + currentAmmo);
    }
}
