using UnityEngine;
using TMPro;

public class SpeedDisplay : MonoBehaviour
{
    public TextMeshProUGUI speedText; // TMP text object to display the speed

    private Vector3 lastPosition; // Store the last position of the GameObject
    private float speed; // The calculated speed

    private string objectName; // Store the name of the object

    private void Start()
    {
        // Initialize the last position at the start
        lastPosition = transform.position;
        objectName = gameObject.name; // Store the name of the object
    }

    private void Update()
    {
        // Calculate the speed by the difference in position
        Vector3 currentPosition = transform.position;
        float distanceMoved = Vector3.Distance(currentPosition, lastPosition);
        float deltaTime = Time.deltaTime;

        // Avoid division by zero
        if (deltaTime > 0f)
        {
            speed = distanceMoved / deltaTime; // Speed = distance / time
        }

        // Update the UI text to display the name and the speed
        if (speedText != null)
        {
            speedText.text = $"{objectName} Speed: {speed:F2} m/s"; // Display the name and speed with 2 decimals
        }

        // Update last position for the next frame
        lastPosition = currentPosition;
    }

    // Public method to get the speed information
    public string GetSpeedInfo()
    {
        return $"{objectName} Speed: {speed:F2} m/s"; // Return the speed and name as a string
    }

    // Public method to get the speed as a float
    public float GetSpeed()
    {
        return speed; // Return the current speed
    }
}
