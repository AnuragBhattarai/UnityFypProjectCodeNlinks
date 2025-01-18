using UnityEngine;

public class AutoVisibilityController : MonoBehaviour
{
    public GameObject[] pickupObjects; // Array to hold the 10 objects
    private PickupSystem pickupSystem; // Reference to the PickupSystem script

    private void Start()
    {
        // Get the PickupSystem script attached to the same GameObject
        pickupSystem = GetComponent<PickupSystem>();

        // Ensure the array contains exactly 10 objects
        if (pickupObjects.Length != 10)
        {
            Debug.LogWarning("There should be exactly 10 pickup objects assigned.");
        }

        // Initially, hide all the objects
        UpdateObjectVisibility(0);
    }

    private void Update()
    {
        // Get the current pickup count from PickupSystem
        int currentPickupCount = pickupSystem.CurrentPickupCount;

        // Make sure the current count doesn't exceed the available objects
        currentPickupCount = Mathf.Min(currentPickupCount, pickupObjects.Length);

        // Update the visibility of the objects
        UpdateObjectVisibility(currentPickupCount);
    }

    private void UpdateObjectVisibility(int count)
    {
        // Loop through all the objects and set visibility based on the count
        for (int i = 0; i < pickupObjects.Length; i++)
        {
            // Hide all objects initially
            pickupObjects[i].SetActive(false);
        }

        // Show objects up to the 'count' value
        for (int i = 0; i < count; i++)
        {
            pickupObjects[i].SetActive(true); // Show up to the 'count' number of objects
        }
    }
}
