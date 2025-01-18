using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras;  // Array to hold references to the cameras
    private int currentCameraIndex = 0;  // Index to keep track of the currently active camera

    private void Start()
    {
        // Ensure the cameras array is populated
        if (cameras.Length > 0)
        {
            // Disable all cameras initially
            foreach (Camera cam in cameras)
            {
                cam.gameObject.SetActive(false);
            }

            // Activate the first camera
            cameras[currentCameraIndex].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("No cameras assigned in the CameraSwitcher script.");
        }
    }

    private void Update()
    {
        // Check if the "C" key is pressed
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Swap to the next camera in the array
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        // Deactivate the current camera
        cameras[currentCameraIndex].gameObject.SetActive(false);

        // Update the camera index to the next one in the array
        currentCameraIndex = (currentCameraIndex + 1) % cameras.Length;

        // Activate the new camera
        cameras[currentCameraIndex].gameObject.SetActive(true);
    }
}
