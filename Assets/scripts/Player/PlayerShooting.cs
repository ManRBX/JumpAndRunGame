using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;
    public int maxAmmo = 60;
    private int currentAmmo;
    private const string AmmoKey = "GlobalAmmo";
    private const string ShotsFiredKey = "ShotsFired";

    [Header("Cooldown Settings")]
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("UI Settings")]
    public TMP_Text ammoText;
    public TMP_Text shotText;
    public GameObject gameOverPanel;

    [Header("Default Settings")]
    public int defaultAmmo = 30;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(AmmoKey))
        {
            PlayerPrefs.SetInt(AmmoKey, defaultAmmo);
            PlayerPrefsKeyTracker.TrackKey(AmmoKey);
        }

        if (!PlayerPrefs.HasKey(ShotsFiredKey))
        {
            PlayerPrefs.SetInt(ShotsFiredKey, 0);
            PlayerPrefsKeyTracker.TrackKey(ShotsFiredKey);
        }

        PlayerPrefs.Save();

        SetAmmoFromPrefs();
        UpdateShotsUI(PlayerPrefs.GetInt(ShotsFiredKey, 0));
    }

    private void Update()
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

    private void Shoot()
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
        PlayerPrefs.SetInt(AmmoKey, currentAmmo);
        PlayerPrefsKeyTracker.TrackKey(AmmoKey);

        int shotsFired = PlayerPrefs.GetInt(ShotsFiredKey, 0) + 1;
        PlayerPrefs.SetInt(ShotsFiredKey, shotsFired);
        PlayerPrefsKeyTracker.TrackKey(ShotsFiredKey);

        PlayerPrefs.Save();

        UpdateAmmoUI();
        UpdateShotsUI(shotsFired);

        Debug.Log($"🔫 Shot fired! Total shots: {shotsFired}, Ammo left: {currentAmmo}");
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        if (currentAmmo > maxAmmo)
            currentAmmo = maxAmmo;

        PlayerPrefs.SetInt(AmmoKey, currentAmmo);
        PlayerPrefsKeyTracker.TrackKey(AmmoKey);
        PlayerPrefs.Save();

        UpdateAmmoUI();
        Debug.Log($"🧨 Munition aufgeladen: {currentAmmo}");
    }

    public void SetAmmoFromPrefs()
    {
        currentAmmo = PlayerPrefs.GetInt(AmmoKey, defaultAmmo);
        UpdateAmmoUI();
        Debug.Log($"🔁 Munition gesetzt aus PlayerPrefs: {currentAmmo}");
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("💀 Kein Schuss mehr! Game Over Panel geöffnet.");
        }

        Time.timeScale = 0f;
    }

    public void ContinueAfterGameOver()
    {
        PlayerPrefs.SetInt(AmmoKey, defaultAmmo);
        PlayerPrefsKeyTracker.TrackKey(AmmoKey);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("🔁 Neustart nach Game Over mit voller Munition.");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        Debug.Log("🏠 Zurück ins Hauptmenü.");
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo.ToString();
    }

    private void UpdateShotsUI(int shots)
    {
        if (shotText != null)
            shotText.text = shots.ToString();
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}
