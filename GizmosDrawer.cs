using UnityEngine;

public class WaypointVisualizer : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public Transform[] waypoints; // Array of waypoints
    public Color waypointColor = Color.green; // Color for waypoints
    public Color lineColor = Color.blue;     // Color for lines
    public float waypointRadius = 0.3f;      // Radius for waypoint spheres

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        // Set the color for the waypoints
        Gizmos.color = waypointColor;

        // Draw spheres at each waypoint position
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawSphere(waypoint.position, waypointRadius);
            }
        }

        // Set the color for the lines
        Gizmos.color = lineColor;

        // Draw lines connecting the waypoints
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
