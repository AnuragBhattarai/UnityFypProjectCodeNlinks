using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The object the camera will follow
    public Vector3 offset = new Vector3(0, 2, -5); // Offset relative to the target

    [Header("Camera Settings")]
    public float followSpeed = 10f; // Speed at which the camera follows the target
    public float rotationSpeed = 5f; // Speed at which the camera rotates

    private float pitch = 0f; // Vertical angle
    private float yaw = 0f; // Horizontal angle

    [Header("Mouse Control Settings")]
    public bool allowMouseRotation = true; // Enable mouse rotation
    public float mouseSensitivity = 100f; // Mouse sensitivity for rotating the camera

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to the camera.");
            return;
        }

        HandleMouseRotation();
        FollowTarget();
    }

    private void HandleMouseRotation()
    {
        if (!allowMouseRotation) return;

        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust yaw (horizontal) and pitch (vertical)
        yaw += mouseX;
        pitch -= mouseY;

        // Clamp pitch to prevent flipping
        pitch = Mathf.Clamp(pitch, -30f, 60f); // Adjust angles to match desired camera behavior
    }

    private void FollowTarget()
    {
        // Calculate desired position
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + targetRotation * offset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate the camera to face the target's forward direction
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}
