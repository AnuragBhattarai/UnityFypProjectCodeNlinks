using UnityEngine;

public class AIEscort : MonoBehaviour
{
    public string targetName = "Agent1";  // Name of the object the AI will follow
    public float followDistance = 5f;  // Distance the AI keeps from the target
    public float heightOffset = 3f;  // Height the AI maintains above the target
    public float moveSpeed = 5f;  // Movement speed of the AI
    public float rotationSpeed = 5f;  // Rotation speed of the AI
    public float arrivalThreshold = 0.2f;  // Threshold to check when the AI has reached the destination

    private Transform target;  // The object the AI will follow
    private Vector3 destination;  // The target position for the AI to move to
    private bool isAtDestination = false;  // Flag to check if AI has reached the destination

    void Start()
    {
        // Automatically find the target object by name
        GameObject targetObject = GameObject.Find(targetName);
        if (targetObject != null)
        {
            target = targetObject.transform;
        }
        else
        {
            Debug.LogError("Target object with name " + targetName + " not found!");
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Calculate the desired destination based on the target position and height offset
            Vector3 targetPosition = target.position + Vector3.up * heightOffset;

            // Determine where the AI should go (keeping the follow distance)
            destination = targetPosition - (targetPosition - transform.position).normalized * followDistance;

            // Rotate and move the AI towards the destination
            MoveTowardsDestination();

            // Once at the destination, look at the target's forward direction
            if (isAtDestination)
            {
                LookAtTarget();
            }
        }
    }

    private void MoveTowardsDestination()
    {
        // Move towards the destination
        Vector3 direction = (destination - transform.position).normalized;
        Vector3 move = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        // Rotate towards the target position while moving
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move the AI using simple translation
        transform.position = move;

        // Check if AI has reached the destination (within a threshold)
        if (Vector3.Distance(transform.position, destination) <= arrivalThreshold)
        {
            isAtDestination = true;
        }
        else
        {
            isAtDestination = false;
        }
    }

    private void LookAtTarget()
    {
        // Smoothly rotate to match the direction the target is facing
        Vector3 targetDirection = target.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        // Use Slerp to smoothly interpolate between current rotation and the target's rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
