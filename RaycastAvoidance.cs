using UnityEngine;

public class RaycastAvoidance : MonoBehaviour
{
    public float detectionRange = 5f; // Range for raycast detection
    public float sidestepDistance = 3f; // Distance to push the object away from detected agent
    public int numRays = 12; // Number of rays to cast in a circular pattern around the object
    public float raycastAngleStep = 30f; // Angle step for each ray in the circular pattern
    public float moveSpeed = 5f; // Speed at which the object moves
    public float rotationSpeed = 5f; // Rotation speed when adjusting to avoid an agent

    private Vector3 currentDirection;  // Stores current movement direction (only for X-axis movement)

    private void Update()
    {
        // Perform raycasting to check for nearby agents and push the object
        PerformRaycastsAndPush();
    }

    private void PerformRaycastsAndPush()
    {
        Vector3 avoidanceForce = Vector3.zero;

        // Raycast directions in a circular pattern around the object (on X-Z plane)
        for (float angle = 0f; angle < 360f; angle += raycastAngleStep)
        {
            Vector3 direction = GetDirectionFromAngle(angle);
            RaycastHit hit;

            // Cast ray in the current direction (X-Z plane)
            if (Physics.Raycast(transform.position, direction, out hit, detectionRange))
            {
                // If we detect an agent, push away from it
                if (hit.collider.CompareTag("Agent"))
                {
                    avoidanceForce += AvoidanceBehavior(hit.collider.transform.position, direction);
                }
            }

            // Additionally cast rays upwards and downwards along the Y-axis
            Vector3 upDirection = direction + Vector3.up;  // Tilted upwards
            Vector3 downDirection = direction + Vector3.down;  // Tilted downwards

            // Cast ray upwards
            if (Physics.Raycast(transform.position, upDirection, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Agent"))
                {
                    avoidanceForce += AvoidanceBehavior(hit.collider.transform.position, upDirection);
                }
            }

            // Cast ray downwards
            if (Physics.Raycast(transform.position, downDirection, out hit, detectionRange))
            {
                if (hit.collider.CompareTag("Agent"))
                {
                    avoidanceForce += AvoidanceBehavior(hit.collider.transform.position, downDirection);
                }
            }
        }

        // Apply avoidance force to move the object (only affects X-axis movement)
        if (avoidanceForce != Vector3.zero)
        {
            transform.position += avoidanceForce * Time.deltaTime;
        }

        // Move the object in the current direction (only X-axis movement)
        MoveObject();
    }

    private Vector3 AvoidanceBehavior(Vector3 agentPosition, Vector3 rayDirection)
    {
        // Push the agent away in the opposite direction of the raycast, but only along the X-axis
        Vector3 awayFromAgent = transform.position - agentPosition;
        
        // Keep only the X component of the direction for movement
        awayFromAgent.y = 0f;
        awayFromAgent.z = 0f;

        // Return the avoidance force normalized, affecting only the X-axis
        return awayFromAgent.normalized * sidestepDistance;
    }

    private void MoveObject()
    {
        // Only move the object along the X-axis (currentDirection.y and currentDirection.z will remain 0)
        if (currentDirection != Vector3.zero)
        {
            Vector3 movementDirection = new Vector3(currentDirection.x, 0f, 0f); // Only use X for movement

            // Move the object along the X-axis
            transform.position += movementDirection * moveSpeed * Time.deltaTime;

            // Rotate smoothly towards the direction it's moving (rotation on the Y-axis to face left/right)
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetDirectionFromAngle(float angle)
    {
        // Convert the angle to radians and calculate the direction
        float radian = angle * Mathf.Deg2Rad;
        float x = Mathf.Cos(radian);
        float z = Mathf.Sin(radian);

        // Return a vector only for movement in the X-Z plane (ignoring Y)
        return new Vector3(x, 0f, z).normalized; // 2D plane movement (X-Z plane)
    }

    private void OnDrawGizmos()
    {
        // Draw the raycast lines for debugging (green or red depending on hit)
        RaycastHit hit;

        // Cast rays in a circular pattern (including up/down detection)
        for (float angle = 0f; angle < 360f; angle += raycastAngleStep)
        {
            Vector3 direction = GetDirectionFromAngle(angle);

            // Raycast check for horizontal movement (X-Z plane)
            bool isHit = Physics.Raycast(transform.position, direction, out hit, detectionRange);
            Color rayColor = isHit ? Color.red : Color.green;
            Gizmos.color = rayColor;
            Gizmos.DrawLine(transform.position, transform.position + direction * detectionRange);

            // Raycast for up and down detection
            Vector3 upDirection = direction + Vector3.up;
            bool isUpHit = Physics.Raycast(transform.position, upDirection, out hit, detectionRange);
            Gizmos.color = isUpHit ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, transform.position + upDirection * detectionRange);

            Vector3 downDirection = direction + Vector3.down;
            bool isDownHit = Physics.Raycast(transform.position, downDirection, out hit, detectionRange);
            Gizmos.color = isDownHit ? Color.red : Color.green;
            Gizmos.DrawLine(transform.position, transform.position + downDirection * detectionRange);
        }
    }
}
