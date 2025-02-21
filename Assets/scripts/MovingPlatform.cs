using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] points; // Array of Transforms for the points
    public float moveSpeed = 3f; // Movement speed

    private int currentPointIndex = 0; // Index of the current target point
    private bool isActivated = false; // Flag for activation of the movement

    void Update()
    {
        if (points.Length < 2) return; // Ensure that there are at least 2 points
        MovePlatform();
    }

    void MovePlatform()
    {
        // Move the platform towards the current target point
        Transform targetPoint = points[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // If the platform reaches the target, switch to the next point
        if (transform.position == targetPoint.position)
        {
            currentPointIndex++;

            // If the last point is reached, go back to the first point
            if (currentPointIndex >= points.Length)
            {
                currentPointIndex = 0;
            }
        }
    }

    // The player is moved with the platform when standing on it
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Set the player as a child of the platform so they move with it
            other.transform.SetParent(transform);

            // Optional: Adjust the player's Rigidbody2D to make them move with the platform
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Prevents the platform from pulling the player through the air
            }
        }
    }

    // When the player leaves the platform, they are no longer moved with it
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null); // Detach the player from the platform

            // Optional: If you want to reactivate gravity, you can adjust the Rigidbody
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Reset the velocity if needed
            }
        }
    }

    // Optional: Visualize the points and draw a line between them for debugging purposes
    void OnDrawGizmos()
    {
        if (points.Length < 2) return;

        // Draw the lines between the points
        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.color = Color.green; // Choose a color for the line
            Gizmos.DrawLine(points[i].position, points[i + 1].position); // Draw a line between point A and B
        }

        // If you want to draw a line from the last to the first point (for a circular movement)
        Gizmos.color = Color.red; // Choose a different color to visualize the reverse direction
        Gizmos.DrawLine(points[points.Length - 1].position, points[0].position); // Connect the last point to the first
    }
}
