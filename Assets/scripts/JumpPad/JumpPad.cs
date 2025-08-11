using Unity.VisualScripting;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float bounce = 20f;
    [SerializeField] private Animator animator; // Ziehe hier deinen Animator rein

    [Header("🔊 Jumppad-Sound")]
    public AudioSource jumppadSound;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 🔊 Jump_Pad Sound abspielen
        if (jumppadSound != null)
            jumppadSound.Play();

        if (collision.gameObject.CompareTag("Player"))
        {
            // Spieler nach oben stoßen
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            }

            // Trampolin-Animation auslösen, falls der Animator zugewiesen wurde
            if (animator != null)
            {
                animator.SetTrigger("JumpPadHit");
            }
        }
    }
}
