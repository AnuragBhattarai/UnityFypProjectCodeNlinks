using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Agent : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public bool loop = false; // Changed to false to handle round trip
    public float detectionRadius = 5f; // Radius to detect other objects
    public float sidestepDistance = 5f; // Distance to move left/right during overtaking
    public float sidestepStrength = 0.8f; // Multiplier for sidestep effect
    public float movementRadius = 5f; // Radius around the agent for free movement
    public float slowSpeed = 2f; // Speed when yielding to a faster agent (can be higher than 5)
    public float minSpeed = 5f; // Minimum speed that the agent can have
    public float waypointReachThreshold = 1f; // Range threshold for reaching the waypoint

    public TextMeshProUGUI timeDisplay; // Reference to TMP text object
    public MonoBehaviour scriptToEnable; // Reference to the script that needs to be enabled when triggered

    public GameObject objectToDetect; // The object to detect within the radius

    private int currentWaypointIndex = 0;
    private Vector3 currentDirection;
    private bool isReturning = false; // Track if the agent is on the return path
    private List<Vector3> pathTaken = new List<Vector3>(); // Path taken by the agent
    private float tripStartTime; // Start time of the trip

    private ACOController acoController; // Reference to the ACOController
    private Vector3 lastPheromonePosition; // Store the last position where pheromone was deposited

    private void Start()
    {
        if (waypoints.Count > 0)
        {
            transform.position = waypoints[0].transform.position;
            tripStartTime = Time.time; // Record the start time
            pathTaken.Add(transform.position); // Record the starting position
            lastPheromonePosition = transform.position; // Set the initial pheromone position
        }

        // Get the ACOController component
        acoController = FindObjectOfType<ACOController>();

        if (acoController == null)
        {
            Debug.LogError("ACOController not found in the scene!");
        }

        // Ensure the objectToDetect is assigned in the inspector
        if (objectToDetect == null)
        {
            Debug.LogError("Object to detect is not assigned!");
        }
    }

    private void Update()
    {
        if (waypoints.Count > 0 && acoController != null)
        {
            MoveAndRotateToNextWaypoint();
        }

        // Check if the objectToDetect is within detection radius
        if (objectToDetect != null && Vector3.Distance(transform.position, objectToDetect.transform.position) <= detectionRadius)
        {
            DisableAgentAndEnableScript();
        }
    }

    private void MoveAndRotateToNextWaypoint()
    {
        Vector3 targetPosition;

        if (isReturning)
        {
            // Follow the path back to the start
            targetPosition = pathTaken[currentWaypointIndex];
        }
        else
        {
            // Follow the waypoints
            Waypoint targetWaypoint = waypoints[currentWaypointIndex];
            targetPosition = targetWaypoint.transform.position;
        }

        // Calculate the direction towards the target position
        Vector3 waypointDirection = (targetPosition - transform.position).normalized;

        // Get the avoidance direction based on nearby agents
        Vector3 avoidanceDirection = AvoidOtherAgents();

        // Combine the waypoint direction and avoidance direction
        currentDirection = waypointDirection + avoidanceDirection;

        // Normalize to ensure consistent movement speed
        currentDirection = currentDirection.normalized;

        // Apply movement (with potentially reduced speed for the slower agent)
        Vector3 nextPosition = transform.position + currentDirection * moveSpeed * Time.deltaTime;

        // Ensure movement stays within a free radius around the agent's current trajectory
        Vector3 centerPosition = transform.position;
        if (Vector3.Distance(centerPosition, nextPosition) > movementRadius)
        {
            nextPosition = centerPosition + (nextPosition - centerPosition).normalized * movementRadius;
        }

        transform.position = nextPosition;

        // Rotate smoothly towards the direction it's moving
        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Check if the agent has reached the target position (within the threshold range)
        if (Vector3.Distance(transform.position, targetPosition) < waypointReachThreshold)
        {
            AdvanceToNextWaypoint();
        }
    }

    private void AdvanceToNextWaypoint()
    {
        if (isReturning)
        {
            currentWaypointIndex--;

            // Check if the agent has returned to the start
            if (currentWaypointIndex < 0)
            {
                float tripEndTime = Time.time;
                float totalTime = tripEndTime - tripStartTime;

                if (timeDisplay != null)
                {
                    timeDisplay.text = $"Round trip completed in {totalTime:F2} seconds.";
                }
                else
                {
                    Debug.Log($"Round trip completed in {totalTime:F2} seconds.");
                }

                // Automatically disable this agent's script and enable the assigned script
                DisableAgentAndEnableScript();
            }
        }
        else
        {
            currentWaypointIndex++;
            pathTaken.Add(transform.position); // Record the path

            if (currentWaypointIndex >= waypoints.Count)
            {
                // Reverse the path and start returning
                isReturning = true;
                currentWaypointIndex = pathTaken.Count - 1;
            }
        }

        // Deposit pheromone after moving to the next waypoint
        if (!isReturning)
        {
            DepositPheromone();
        }
    }

    // Method to enable another script and disable this agent's script
    private void DisableAgentAndEnableScript()
    {
        if (scriptToEnable != null)
        {
            scriptToEnable.enabled = true; // Enable the assigned script
        }

        // Disable the current agent script
        enabled = false;
    }

    // Deposit pheromone after the agent moves from one waypoint to another
    private void DepositPheromone()
    {
        // Check if the agent has moved significantly since the last pheromone deposit
        if (Vector3.Distance(transform.position, lastPheromonePosition) > waypointReachThreshold)
        {
            acoController.DepositPheromone(this, lastPheromonePosition, transform.position); // Send the current agent to the pheromone controller
            lastPheromonePosition = transform.position; // Update the last pheromone position
        }
    }

    private Vector3 AvoidOtherAgents()
    {
        Vector3 avoidanceForce = Vector3.zero;

        // Find nearby agents within the detection radius
        Collider[] nearbyAgents = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in nearbyAgents)
        {
            // If it's another agent, apply avoidance logic
            if (collider.CompareTag("Agent") && collider.gameObject != this.gameObject)
            {
                Agent otherAgent = collider.GetComponent<Agent>();
                if (otherAgent == null) continue;

                Vector3 directionToAgent = collider.transform.position - transform.position;
                float distanceToAgent = directionToAgent.magnitude;

                // Determine which agent is faster
                bool isOtherAgentFaster = otherAgent.moveSpeed > this.moveSpeed;

                if (isOtherAgentFaster)
                {
                    // Slow down the slower agent but ensure it doesn't go below minSpeed
                    moveSpeed = Mathf.Max(slowSpeed, minSpeed); // Ensure slowSpeed is at least minSpeed
                    // Move left (pull to the left)
                    Vector3 sidestepLeft = Vector3.Cross(Vector3.up, -directionToAgent.normalized).normalized;
                    avoidanceForce += sidestepLeft * sidestepDistance / distanceToAgent;
                }
                else
                {
                    // Faster agent moves to the right with a straight angle (not curved)
                    moveSpeed = Mathf.Max(moveSpeed, 5f); // Restore normal speed for faster agent
                    // Move right (faster agent takes the right path)
                    Vector3 sidestepRight = Vector3.Cross(Vector3.up, directionToAgent.normalized).normalized;
                    avoidanceForce += sidestepRight * sidestepDistance / distanceToAgent;
                }
            }
        }

        return avoidanceForce;
    }

    private void OnDrawGizmos()
    {
        // Draw a gizmo to visualize the detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw a gizmo to visualize the movement radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, movementRadius);
    }
}
