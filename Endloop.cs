using UnityEngine;

public class Endloop : MonoBehaviour
{
    public Transform pointB;      // Point B
    private Transform target;     // Current target (in this case, Point B)
    private bool isMoving = false; // Flag to check if ObjectX is moving
    public float speed = 2.0f;    // Movement speed of ObjectX

    void Update()
    {
        // Check if ObjectB is destroyed
        if (GameObject.Find("ObjectB") == null && !isMoving)
        {
            // If ObjectB is destroyed, start moving towards PointB
            isMoving = true;
            target = pointB;  // Set target to PointB
        }

        // If ObjectX is moving, move towards the target
        if (isMoving && target != null)
        {
            MoveTowardsTarget();

            // Check if ObjectX has reached the target
            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                // Once ObjectX reaches PointB, stop moving
                isMoving = false;
                Debug.Log("ObjectX has reached PointB after ObjectB was destroyed.");
            }
        }
    }

    // Move Object X towards the target position
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }
}
