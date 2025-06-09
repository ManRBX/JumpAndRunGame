using System.Collections;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D playerCollider;

    private GameObject currentOneWayPlatform;

    void Update()
    {
        if (KeyBindManager.Instance != null &&
            Input.GetKeyDown(KeyBindManager.Instance.GetKeyCodeForAction("DropPlatform")))
        {
            if (currentOneWayPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollision()
    {
        // WICHTIG: Richtigen Collider-Typ holen
        Collider2D platformCollider = currentOneWayPlatform.GetComponent<Collider2D>();

        if (platformCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, platformCollider);
            yield return new WaitForSeconds(0.5f);
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        }
    }
}
