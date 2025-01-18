using UnityEngine;

public class WaypointVisibilityToggle : MonoBehaviour
{
    public Transform[] waypoints;  // Array of waypoints
    public GameObject waypointPrefab;  // Prefab for the waypoint (e.g., a sphere)
    private GameObject[] waypointObjects;  // Hold references to instantiated waypoint objects
    private bool showWaypoints = true;  // Flag to toggle waypoint visibility

    void Start()
    {
        // Instantiate the waypoints in the game world using the prefab
        waypointObjects = new GameObject[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                waypointObjects[i] = Instantiate(waypointPrefab, waypoints[i].position, Quaternion.identity);
                waypointObjects[i].SetActive(showWaypoints);  // Set visibility based on the flag
            }
        }
    }

    void Update()
    {
        // Toggle the waypoint visibility when the 'V' key is pressed
        if (Input.GetKeyDown(KeyCode.V))
        {
            showWaypoints = !showWaypoints;
            ToggleWaypointVisibility();
        }
    }

    private void ToggleWaypointVisibility()
    {
        // Loop through each waypoint object and toggle its visibility
        foreach (var waypoint in waypointObjects)
        {
            if (waypoint != null)
            {
                waypoint.SetActive(showWaypoints);  // Show or hide the waypoint
            }
        }
    }
}
