using UnityEngine;
using UnityEngine.UI; // Include this for UI components

public class Transport : MonoBehaviour
{
    public Transform resourcePlacement; // The transform where resources are placed
    public Slider capacitySlider; // UI Slider to indicate capacity
    public float resourceSpacing = 0.5f; // Spacing between resources
    public int maxCapacity = 16; // Maximum number of resources the transport can hold
    private int resourceCount = 0; // Number of resources currently on the transport

    private int maxResourcesPerRow = 4; // 4 resources per row (horizontal)
    private int maxRows = 4; // 4 rows (vertical)

    void Start()
    {
        if (!resourcePlacement)
        {
            Debug.LogError("Resource Placement Transform is not assigned!");
        }

        if (!capacitySlider)
        {
            Debug.LogError("Capacity Slider is not assigned!");
        }
        else
        {
            capacitySlider.maxValue = maxCapacity; // Set the max value for the slider
            UpdateSlider(); // Initialize the slider's value
        }
    }

    public bool ReceiveResource(GameObject resource)
    {
        if (resourceCount >= maxCapacity)
        {
            Debug.Log("Transport is at full capacity!");
            return false;
        }

        // Calculate the position for the new resource in the 4x4 grid
        Vector3 newPosition = CalculateResourcePosition(resourceCount);
        resource.transform.position = resourcePlacement.position + newPosition;

        // Parent the resource to the transport for organization
        resource.transform.parent = resourcePlacement;

        resourceCount++;
        UpdateSlider(); // Update the slider to reflect the new capacity
        return true;
    }

    private Vector3 CalculateResourcePosition(int index)
    {
        // Make sure we stay within the 4x4 grid: 16 resources
        int row = index / maxResourcesPerRow; // Row index (0 to 3)
        int column = index % maxResourcesPerRow; // Column index (0 to 3)

        // Calculate the offset based on the row and column
        float rowOffset = row * resourceSpacing; // Adjust for the row
        float columnOffset = column * resourceSpacing; // Adjust for the column

        // Center the grid by subtracting half of the total grid size to place the resources in the middle
        float x = columnOffset - (resourceSpacing * (maxResourcesPerRow - 1) / 2f); // Adjust x to center horizontally
        float y = 0; // Keep y fixed for a flat grid
        float z = rowOffset - (resourceSpacing * (maxRows - 1) / 2f); // Adjust z to center vertically

        return new Vector3(x, y, z);
    }

    private void UpdateSlider()
    {
        if (capacitySlider)
        {
            capacitySlider.value = resourceCount; // Update the slider value
        }
    }
}
