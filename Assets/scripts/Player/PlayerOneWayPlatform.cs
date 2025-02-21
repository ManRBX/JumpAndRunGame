using System.Collections;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    [SerializeField] private BoxCollider2D playerCollider;

    private GameObject currentOneWayPlatform;

    void Update()
    {
        // Instead of checking (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)), 
        // now you only check ONE action: "DropPlatform".
        // Default is S or DownArrow (as set in the key bindings).
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

    /// <summary>
    /// Temporarily disables collision with the one-way platform, 
    /// allowing the player to drop through it.
    /// </summary>
    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(playerCollider, platformCollider);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
