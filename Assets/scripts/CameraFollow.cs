using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Ziel der Kamera, normalerweise der Spieler
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) // Prüfen, ob das Ziel noch existiert
        {
            Debug.LogWarning("Das Ziel für die Kamera existiert nicht mehr.");
            return; // Verlasse die Methode, um Fehler zu vermeiden
        }

        // Kamera folgt dem Ziel
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
