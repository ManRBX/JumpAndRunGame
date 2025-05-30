using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] points;
    public float moveSpeed = 3f;

    private int currentPointIndex = 0;
    private Vector3 lastPosition;
    [HideInInspector] public Vector3 platformVelocity;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (points.Length < 2) return;
        MovePlatform();
        platformVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    void MovePlatform()
    {
        Transform targetPoint = points[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        if (transform.position == targetPoint.position)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Length;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kein Parenting, der Player holt sich die Velocity selber
    }

    void OnDrawGizmos()
    {
        if (points.Length < 2) return;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(points[i].position, points[i + 1].position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(points[points.Length - 1].position, points[0].position);
    }
}