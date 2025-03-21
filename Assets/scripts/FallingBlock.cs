using UnityEngine;
using System.Collections;

public class FallingBlock : MonoBehaviour
{
    private Vector3 originalPosition;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isFalling = false;

    public float shakeDuration = 5f;
    public float fallDelay = 5f;
    public float respawnDelay = 30f;
    public Color warningColor = Color.red;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalPosition = transform.position;
        originalColor = spriteRenderer.color;

        rb.isKinematic = true;  // Block f�llt erst nach Aktivierung
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            StartCoroutine(WarningPhase());
        }
    }

    private IEnumerator WarningPhase()
    {
        isFalling = true;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            spriteRenderer.color = warningColor;
            transform.position = originalPosition + (Vector3)Random.insideUnitCircle * 0.1f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        yield return new WaitForSeconds(fallDelay - shakeDuration);

        StartCoroutine(FallAndRespawn());
    }

    private IEnumerator FallAndRespawn()
    {
        col.enabled = false;
        rb.isKinematic = false;  // Schwerkraft aktivieren
        yield return new WaitForSeconds(2f);  // Block kann vollst�ndig fallen

        rb.isKinematic = true;
        rb.linearVelocity = Vector2.zero;
        transform.position = new Vector3(1000, 1000, 0);  // Block verstecken
        yield return new WaitForSeconds(respawnDelay);

        transform.position = originalPosition;
        spriteRenderer.color = originalColor;
        col.enabled = true;
        isFalling = false;
    }
}
