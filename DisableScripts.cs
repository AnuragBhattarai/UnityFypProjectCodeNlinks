using UnityEngine;
using System.Collections; // To use coroutines

public class DisableScripts : MonoBehaviour
{
    public MainStorage mainStorage;  // Reference to the MainStorage script
    public MonoBehaviour[] scriptsToDisable;  // List of scripts to disable
    public int depositThreshold = 10;  // Threshold for the deposit count
    public float checkInterval = 1f;  // Interval to check the condition
    public float delayBeforeEnabling = 5f;  // Delay in seconds before enabling scripts

    private bool scriptsEnabled = false;  // Flag to check if scripts are enabled

    private void Start()
    {
        if (mainStorage == null)
        {
            mainStorage = FindObjectOfType<MainStorage>(); // Try to find MainStorage if not assigned
        }

        if (mainStorage != null)
        {
            // Initially check the condition and enable/disable scripts accordingly
            CheckAndUpdateScripts();
        }
        else
        {
            Debug.LogError("MainStorage reference is missing!");
        }
    }

    private void Update()
    {
        // Continuously check the condition at specified intervals
        if (Time.time % checkInterval < 0.1f)
        {
            CheckAndUpdateScripts();
        }
    }

    // Method to check if the deposit count meets the threshold and enable/disable scripts
    private void CheckAndUpdateScripts()
    {
        if (mainStorage != null && mainStorage.IsCounterReached(depositThreshold) && !scriptsEnabled)
        {
            // If the deposit count meets the threshold and scripts are not yet enabled, wait before enabling scripts
            StartCoroutine(EnableScriptsAfterDelay());
        }
        else if (!scriptsEnabled)
        {
            DisableScriptsOnObject();
        }
    }

    // Coroutine to wait before enabling scripts
    private IEnumerator EnableScriptsAfterDelay()
    {
        // Wait for the specified delay time before enabling the scripts
        yield return new WaitForSeconds(delayBeforeEnabling);

        // Enable the scripts after the delay
        EnableScriptsOnObject();
        scriptsEnabled = true;  // Mark that scripts have been enabled
    }

    // Disable all specified scripts
    private void DisableScriptsOnObject()
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }

    // Enable all specified scripts
    private void EnableScriptsOnObject()
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }
    }
}
