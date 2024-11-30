using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    public float pickupRadius = 5f; // The radius within which to detect resources
    public string resourceTag = "Resource"; // Tag to identify resources in the scene
    public Transform hq; // Reference to the HQ transform
    public float hqRadius = 3f; // The radius within which to deliver resources

    // Flag to check if delivery is complete
    private bool isDelivering = false;

    void Update()
    {
        // If we are not currently delivering, continue picking up resources
        if (!isDelivering)
        {
            PickupResourceInRange();
        }

        // Check if the parent object has collided with the HQ and is ready to deliver resources
        if (isDelivering && Vector3.Distance(transform.position, hq.position) <= hqRadius)
        {
            DeliverResources();
        }
    }

    // Detect when the object enters the HQ collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HQ")) // Make sure the HQ has the "HQ" tag
        {
            isDelivering = true; // Begin delivery process
            Debug.Log("Collided with HQ - Ready to deliver resources");
        }
    }

    // Detect when the object exits the HQ collider
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HQ"))
        {
            isDelivering = false; // Stop delivery process if exiting the HQ
            Debug.Log("Exited the HQ collider");
        }
    }

    // Pickup resources within the defined radius
    void PickupResourceInRange()
    {
        // Detect all colliders within the pickup radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRadius);

        foreach (Collider col in colliders)
        {
            // Check if the object is a resource based on its tag
            if (col.CompareTag(resourceTag))
            {
                GameObject resource = col.gameObject;

                // Parent the resource to this object
                resource.transform.SetParent(transform);

                // Optionally, print a debug message to verify the resource was picked up
                Debug.Log($"Resource {resource.name} has been picked up and parented to {gameObject.name}");
            }
        }
    }

    // Deliver all resources to the HQ (destroy the child resources when delivered)
    void DeliverResources()
    {
        // Loop through all child objects (resources) and deliver them
        foreach (Transform child in transform)
        {
            // Here we simulate the delivery by destroying the resource
            // You can replace this with more complex logic (e.g., updating HQ UI, scoring, etc.)
            Destroy(child.gameObject);

            // Optionally, print a debug message to verify the resource was delivered
            Debug.Log($"Resource {child.name} delivered to HQ.");
        }

        // After delivering resources, stop the pickup process
        Debug.Log("All resources delivered.");
    }
}
