using UnityEngine;

public class Agent : MonoBehaviour
{
    public Transform[] waypoints; // Waypoints the agent navigates to
    public GameObject resourcePrefab; // Prefab of the resource to carry
    public Transform transport; // Transport location
    public float agentSpeed = 2f; // Speed of the agent
    public float harvestRange = 2f; // Range to start harvesting
    public float transportRange = 2f; // Range to start loading resource
    public float harvestTime = 3f; // Time it takes to harvest a resource

    private int currentWaypointIndex = 0;
    private GameObject currentResource; // The resource the agent is carrying
    private bool isHarvesting = false; // Indicates if the agent is harvesting
    private bool isReturning = false; // Indicates if the agent is returning to transport
    private bool isWaitingAtWaypoint1 = false; // To track if the agent is waiting at waypoint 1
    private float harvestTimer = 0f; // Tracks harvest time

    void Update()
    {
        // Check if the agent is carrying a resource
        if (currentResource != null)
        {
            if (isWaitingAtWaypoint1)
            {
                StayAtWaypoint1();
            }
            else
            {
                NavigateToWaypoint();
            }
        }
        else if (isHarvesting)
        {
            Harvesting();
        }
        else
        {
            NavigateToWaypoint();
        }

        // Check for harvestable nodes and transport in range
        CheckHarvestableRange();
        CheckTransportRange();
    }

    void NavigateToWaypoint()
    {
        if (waypoints.Length == 0) return;

        // The agent moves towards the current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, agentSpeed * Time.deltaTime);

        // If the agent reaches the waypoint, go to the next one
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            if (isReturning)
            {
                // If the agent is reversing back to waypoint 1, prevent going out of bounds
                if (currentWaypointIndex > 0)
                {
                    currentWaypointIndex--;
                }
                else
                {
                    // Once the agent reaches waypoint 1, stop moving
                    isReturning = false;
                    isWaitingAtWaypoint1 = true; // Stay idle at waypoint 1
                }
            }
            else
            {
                // If the agent is not returning, continue to the next waypoint
                if (currentWaypointIndex < waypoints.Length - 1)
                {
                    currentWaypointIndex++;
                }
                else
                {
                    // Start returning after reaching the last waypoint
                    isReturning = true;
                }
            }
        }
    }

    void StayAtWaypoint1()
    {
        // If the agent is at waypoint 1 and the transport is still not in range, stay idle
        if (Vector3.Distance(transform.position, waypoints[0].position) < 0.1f)
        {
            // Agent waits at waypoint 1 until transport is in range
            isWaitingAtWaypoint1 = true;
        }
    }

    void CheckHarvestableRange()
    {
        // Don't check for harvestables if carrying a resource
        if (currentResource != null) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, harvestRange);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Harvestable") && !isHarvesting)
            {
                HarvestNode harvestNode = collider.GetComponent<HarvestNode>();
                if (harvestNode && harvestNode.Harvest())
                {
                    // Start harvesting
                    isHarvesting = true;
                    harvestTimer = 0f;
                }
            }
        }
    }

    void Harvesting()
    {
        harvestTimer += Time.deltaTime;
        if (harvestTimer >= harvestTime)
        {
            // Finish harvesting and pick up the resource
            currentResource = Instantiate(resourcePrefab, transform.position + Vector3.up, Quaternion.identity);
            currentResource.transform.parent = transform;
            isHarvesting = false;

            // Start returning to transport
            isReturning = true;
        }
    }

    void CheckTransportRange()
    {
        // If carrying a resource and the transport is in range
        if (currentResource != null && Vector3.Distance(transform.position, transport.position) < transportRange)
        {
            StartLoadingSequence();
        }

        // If carrying a resource but no transport is nearby, wait at waypoint 1
        if (currentResource != null && Vector3.Distance(transform.position, transport.position) > transportRange)
        {
            // Only wait at waypoint 1 if it's not already idle
            if (Vector3.Distance(transform.position, waypoints[0].position) < 0.1f)
            {
                isWaitingAtWaypoint1 = true;
            }
        }
    }

    void StartLoadingSequence()
    {
        // Once in range, the agent delivers the resource to the transport
        Transport transportScript = transport.GetComponent<Transport>();
        if (transportScript)
        {
            transportScript.ReceiveResource(currentResource);
            currentResource.transform.parent = null; // Detach resource from agent
            currentResource = null; // Resource is dropped off

            // Allow movement again after delivering the resource
            isReturning = false;

            // After delivering, proceed to next waypoint
            isWaitingAtWaypoint1 = false;
        }
    }
}
