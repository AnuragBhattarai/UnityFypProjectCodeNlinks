using UnityEngine;

public class SpeedReducer : MonoBehaviour
{
    public int maxPickupAmount = 10; // Maximum number of pickups
    public float reductionPercentage = 10f; // Percentage to reduce per pickup
    public float agentDetectionRange = 5f; // Range to detect the agent in front
    public float minSpeed = 1f; // Minimum speed that the agent should maintain

    private PickupSystem pickupSystem; // Reference to the PickupSystem script
    private Agent agent; // Reference to the Agent script
    private float originalSpeed; // To store the original speed of the agent

    private void Start()
    {
        // Automatically find the PickupSystem and Agent scripts attached to the same GameObject
        pickupSystem = GetComponent<PickupSystem>();
        agent = GetComponent<Agent>();

        // Ensure the PickupSystem and Agent scripts are found
        if (pickupSystem == null)
        {
            Debug.LogWarning("PickupSystem script not found on this GameObject.");
        }
        if (agent == null)
        {
            Debug.LogWarning("Agent script not found on this GameObject.");
        }

        // Store the original speed to reference later
        if (agent != null)
        {
            originalSpeed = agent.moveSpeed;
        }
    }

    private void Update()
    {
        // Check if there is an agent in front and reduce the speed accordingly
        AdjustSpeedBasedOnAgentDetection();

        // Call the method to update speed based on pickups
        UpdateSpeed();
    }

    // Check if there is an Agent in front and adjust speed based on the distance
    private void AdjustSpeedBasedOnAgentDetection()
    {
        if (agent != null)
        {
            // Cast a ray in front of the object to check if there is an agent within range
            RaycastHit hit;
            Vector3 forwardDirection = transform.forward;
            if (Physics.Raycast(transform.position, forwardDirection, out hit, agentDetectionRange))
            {
                // If the hit object has the "Agent" tag, adjust speed
                if (hit.collider.CompareTag("Agent"))
                {
                    float distanceToAgent = hit.distance;
                    float agentSpeed = hit.collider.GetComponent<Agent>().moveSpeed;

                    // If the agent in front is faster than this object, allow the front agent to move
                    if (agentSpeed > agent.moveSpeed)
                    {
                        // Don't reduce the faster agent's speed to zero, allow it to slow down but not stop
                        agent.moveSpeed = Mathf.Max(agent.moveSpeed, minSpeed); // Ensure speed never goes below minSpeed

                        // Log the action
                        Debug.Log($"Agent detected at {distanceToAgent:F2} meters with speed {agentSpeed}. Speed adjusted to minimum: {minSpeed}.");
                    }
                    else
                    {
                        // If the agent in front is slower, it can slow down to avoid collision
                        agent.moveSpeed = Mathf.Max(agent.moveSpeed - (agentSpeed * reductionPercentage / 100f), minSpeed);

                        // Log that the slower agent is adjusting speed
                        Debug.Log($"Agent detected at {distanceToAgent:F2} meters with speed {agentSpeed}. Slowing down to avoid collision.");
                    }

                    return;
                }
            }
        }

        // If no agent is detected, restore the agent's speed to the original speed
        if (agent != null)
        {
            agent.moveSpeed = Mathf.Max(originalSpeed, minSpeed);
        }
    }

    // Call this method every time the PickupSystem updates the pickup count
    public void UpdateSpeed()
    {
        if (pickupSystem != null && agent != null)
        {
            int currentPickupCount = pickupSystem.CurrentPickupCount;

            // Calculate the speed reduction based on pickups
            float speedReduction = agent.moveSpeed * (reductionPercentage / 100f) * currentPickupCount;

            // Calculate the new speed after reduction
            float newSpeed = agent.moveSpeed - speedReduction;

            // Ensure speed doesn't drop below 0 or the minimum speed
            newSpeed = Mathf.Max(newSpeed, minSpeed);

            // Display the calculated speed in the console
            Debug.Log($"Current Speed: {agent.moveSpeed:F2} m/s");
            Debug.Log($"Pickup Count: {currentPickupCount}/{maxPickupAmount}");
            Debug.Log($"Calculated Speed Reduction: {speedReduction:F2} m/s");
            Debug.Log($"New Speed after Reduction: {newSpeed:F2} m/s");

            // Update the agent's speed
            agent.moveSpeed = newSpeed;
        }
    }

    // Visualize the agent detection range for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * agentDetectionRange, 0.5f);
    }
}
