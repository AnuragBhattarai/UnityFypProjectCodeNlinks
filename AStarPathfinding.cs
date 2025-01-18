using UnityEngine;
using System.Collections.Generic;

public class AStarPathfinding : MonoBehaviour
{
    public List<PathWaypoint> waypoints;  // List of waypoints to navigate
    public float moveSpeed = 3f;          // Speed of the agent

    private int currentWaypointIndex = 0;  // Index of the current waypoint

    private void Start()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogError("Waypoints list is empty!");
        }
    }

    private void Update()
    {
        if (waypoints.Count > 0)
        {
            // Move towards the current waypoint
            MoveTowardsWaypoint(waypoints[currentWaypointIndex]);

            // Check if we've reached the current waypoint
            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].transform.position) < 0.1f)
            {
                // Go to the next waypoint
                currentWaypointIndex++;

                // If we have reached the last waypoint, start over
                if (currentWaypointIndex >= waypoints.Count)
                {
                    currentWaypointIndex = 0;
                }
            }
        }
    }

    // Move towards the next waypoint in the path
    private void MoveTowardsWaypoint(PathWaypoint waypoint)
    {
        // Move towards the waypoint
        transform.position = Vector3.MoveTowards(transform.position, waypoint.transform.position, moveSpeed * Time.deltaTime);

        // Rotate to face the next waypoint
        Vector3 direction = waypoint.transform.position - transform.position;
        if (direction != Vector3.zero)  // Prevent division by zero in case of no movement
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * moveSpeed);
        }
    }
}

[System.Serializable]
public class PathWaypoint
{
    public Transform transform;  // Reference to the waypoint's position
}
