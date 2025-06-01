using System;
using System.Collections;
using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    private bool isActive = false;
    private float range;
    private float speed;

    public void ActivateMagnet(float duration, float r, float s)
    {
        range = r;
        speed = s;
        StartCoroutine(MagnetCoroutine(duration));
    }

    private IEnumerator MagnetCoroutine(float duration)
    {
        isActive = true;
        yield return new WaitForSeconds(duration);
        isActive = false;
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        Collider2D[] coins = Physics2D.OverlapCircleAll(transform.position, range);
        foreach (Collider2D col in coins)
        {
            if (col.CompareTag("Coin"))
            {
                col.transform.position = Vector3.MoveTowards(col.transform.position, transform.position, speed * Time.fixedDeltaTime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (isActive)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }

    internal void ActivateMagnet(float duration)
    {
        throw new NotImplementedException();
    }
}
