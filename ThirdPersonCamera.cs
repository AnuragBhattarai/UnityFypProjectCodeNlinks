using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The object the camera will follow
    public Vector3 offset = new Vector3(0, 2, -5); // Offset relative to the target

    [Header("Camera Settings")]
    public float followSpeed = 10f; // Speed at which the camera follows the target
    public float rotationSpeed = 5f; // Speed at which the camera rotates

    [Header("Mouse Control Settings")]
    public bool allowMouseRotation = true; // Enable mouse rotation
    public float mouseSensitivity = 100f; // Mouse sensitivity for rotating the camera

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f; // Speed at which the camera zooms in/out
    public float minZoom = 3f; // Minimum zoom distance
    // Remove maxZoom so that there's no limit on zoom out

    [Header("Camera Distance Settings")]
    public float cameraDistance = 5f; // Camera distance from the target
    public float cameraDistanceSpeed = 2f; // Speed at which the camera distance changes
    public float minCameraDistance = 3f; // Minimum distance from the target
    // Remove maxCameraDistance for no limit on the camera's distance

    [Header("Collision Settings")]
    public float cameraCollisionRadius = 0.5f; // Radius for the camera to check for collisions
    public LayerMask collisionLayer; // Layer mask to define what the camera can collide with

    private float pitch = 0f; // Vertical angle
    private float yaw = 0f; // Horizontal angle

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No target assigned to the camera.");
            return;
        }

        HandleMouseRotation();
        HandleZoom();
        HandleCameraDistance();
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

    private void HandleZoom()
    {
        // Get scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Zoom in/out based on scroll wheel
        offset.z += scrollInput * zoomSpeed;

        // Clamp the zoom to the specified minimum zoom distance only
        offset.z = Mathf.Clamp(offset.z, -minZoom, -minZoom); // Remove maxZoom restriction
    }

    private void HandleCameraDistance()
    {
        // Use arrow keys or other input to change camera distance
        float distanceInput = Input.GetAxis("Vertical") * cameraDistanceSpeed * Time.deltaTime;

        // Adjust camera distance based on user input (could be mapped to other controls)
        cameraDistance = Mathf.Clamp(cameraDistance + distanceInput, minCameraDistance, cameraDistance); // Removed max distance limit

        // Update the offset based on the camera distance
        offset.z = -cameraDistance; // Keep the camera distance along the negative Z-axis
    }

    private void FollowTarget()
    {
        // Adjust the camera's offset dynamically to ensure it is not inside the mesh
        AdjustCameraPosition();

        // Calculate desired position
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + targetRotation * offset;

        // Smoothly move the camera to the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate the camera to face the target's forward direction
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void AdjustCameraPosition()
    {
        // Get the target's collider bounds for size estimation
        Collider targetCollider = target.GetComponent<Collider>();
        if (targetCollider == null)
        {
            Debug.LogWarning("Target has no collider. Camera position adjustments may not work properly.");
            return;
        }

        // Get the distance from the target to the camera
        Vector3 cameraPosition = transform.position;
        Vector3 targetPosition = target.position;

        // Check if the camera is inside the target's bounds
        Vector3 directionToTarget = cameraPosition - targetPosition;
        float distanceToTarget = directionToTarget.magnitude;

        // If the camera is too close to the target, move it further away
        if (distanceToTarget < targetCollider.bounds.extents.magnitude)
        {
            // Move the camera back to ensure it doesn't clip into the target
            offset.z = -targetCollider.bounds.extents.magnitude * 1.5f; // Adjust based on the size of the target
        }

        // Ensure the camera is not too close from the target (based on minCameraDistance)
        offset.z = Mathf.Clamp(offset.z, -cameraDistance, -minCameraDistance);
    }
}
