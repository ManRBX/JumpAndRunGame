using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Target for the camera, usually the player
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null) // Check if the target still exists
        {
            Debug.LogWarning("The camera target no longer exists.");
            return; // Exit the method to avoid errors
        }

        // Camera follows the target
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
