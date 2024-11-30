using UnityEngine;
using UnityEngine.UI;  // Import UI namespace for Slider

public class TruckMovement : MonoBehaviour
{
    public Transform[] waypoints;  // Waypoints for the truck to follow
    public Transform hq;  // The HQ to deliver resources to
    public float truckSpeed = 3f;  // Speed of the truck
    public float deliveryRadius = 5f;  // Radius to deliver to the HQ
    public float waitTime = 3f;  // Time to wait after completing a loop (in seconds)
    public int maxCarryAmount = 16;  // Maximum carry amount (resources)
    public Slider resourceSlider;  // Slider to track resource load

    private int currentCarryAmount = 0;  // Current number of resources on the truck
    private int currentWaypointIndex = 0;
    private bool isDelivering = false;
    private bool isReversing = false;  // Flag to check if the truck is reversing the path
    private bool isWaiting = false;  // Flag to check if the truck is waiting
    private bool isReadyToMove = false;  // Flag to indicate whether the truck is ready to start moving

    private float waitTimer = 0f;  // Timer for waiting

    void Start()
    {
        if (resourceSlider != null)
        {
            // Set the slider's maximum value to the truck's max carry amount
            resourceSlider.maxValue = maxCarryAmount;
            // Optionally set the slider's current value to the current carry amount
            resourceSlider.value = currentCarryAmount;
        }
        else
        {
            Debug.LogError("Slider is not assigned in the TruckMovement script!");
        }
    }

    void Update()
    {
        // If the truck is not ready to move, stop here
        if (!isReadyToMove)
        {
            return;
        }

        // Check if the slider value has reached max capacity (16)
        if (resourceSlider != null && resourceSlider.value >= maxCarryAmount)
        {
            // Print "Max value reached!" to the console
            Debug.Log("Max value reached!");

            // Start moving after displaying the message
            if (!isReadyToMove)
            {
                isReadyToMove = true;
                Debug.Log("Truck is full and ready to start moving.");
                // Start the truck's movement here
                StartMoving();
            }
        }

        if (isWaiting)
        {
            // If the truck is waiting, decrement the wait timer
            waitTimer -= Time.deltaTime;

            // Once the wait time is over, start moving again
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                StartMoving();
            }
        }
        else
        {
            // Move along the path
            MoveAlongPath();

            // If the truck is delivering and it's in range of the HQ, deliver the resource
            if (isDelivering && Vector3.Distance(transform.position, hq.position) <= deliveryRadius)
            {
                DeliverToHQ();
            }
        }
    }

    void MoveAlongPath()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, truckSpeed * Time.deltaTime);

        // If the truck reaches the waypoint, go to the next one
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            if (!isReversing)  // If the truck is moving forward
            {
                if (currentWaypointIndex == waypoints.Length - 1)  // If it's at the last waypoint
                {
                    // Start reversing the direction
                    isReversing = true;
                    currentWaypointIndex--;  // Move towards the second last waypoint
                }
                else
                {
                    currentWaypointIndex++;  // Move to the next waypoint
                }
            }
            else  // If the truck is reversing
            {
                if (currentWaypointIndex == 0)  // If it's at the first waypoint
                {
                    // Start waiting after completing the loop
                    isReversing = false;
                    isWaiting = true;
                    waitTimer = waitTime;  // Set the wait timer
                    currentWaypointIndex++;  // Move to the second waypoint to start the next loop
                }
                else
                {
                    currentWaypointIndex--;  // Move to the previous waypoint
                }
            }
        }
    }

    void DeliverToHQ()
    {
        Debug.Log("Delivering to HQ...");

        // Delivery logic can be added here (e.g., unload resources)
        // After delivery, reset the delivering flag
        isDelivering = false;

        // Reset current carry amount to zero after delivery
        currentCarryAmount = 0;
        isReadyToMove = false;  // Reset movement
    }

    void StartMoving()
    {
        // Start moving after the wait time
        MoveAlongPath();
    }

    // Call this method when resources are loaded onto the truck
    public void LoadResources(int amount)
    {
        if (resourceSlider != null)
        {
            // Add resources to the slider's current value
            resourceSlider.value += amount;

            // Make sure the slider does not exceed the max capacity
            if (resourceSlider.value > maxCarryAmount)
            {
                resourceSlider.value = maxCarryAmount;
            }

            Debug.Log("Loaded " + amount + " resources. Current load: " + resourceSlider.value + "/" + maxCarryAmount);
        }
        else
        {
            Debug.LogError("Slider is not assigned in the TruckMovement script!");
        }
    }

    // Optional: Method to unload resources
    public void UnloadResources(int amount)
    {
        if (resourceSlider != null)
        {
            resourceSlider.value -= amount;
            if (resourceSlider.value < 0)
            {
                resourceSlider.value = 0;
            }

            Debug.Log("Unloaded " + amount + " resources. Current load: " + resourceSlider.value + "/" + maxCarryAmount);
        }
        else
        {
            Debug.LogError("Slider is not assigned in the TruckMovement script!");
        }
    }
}
