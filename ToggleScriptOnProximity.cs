using UnityEngine;

public class ToggleBetweenScriptsOnProximity : MonoBehaviour
{
    public GameObject targetObject;  // The object whose radius will trigger the action
    public float triggerRadius = 5f; // Radius to detect the proximity
    public MonoBehaviour script1;    // The first script to toggle
    public MonoBehaviour script2;    // The second script to toggle

    private void Start()
    {
        // Ensure script1 is enabled and script2 is disabled by default
        if (script1 != null)
            script1.enabled = true;

        if (script2 != null)
            script2.enabled = false;
    }

    private void Update()
    {
        // Check the distance between this object and the targetObject
        float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);

        if (distanceToTarget <= triggerRadius)
        {
            // If within radius, enable script1 and disable script2
            ToggleScripts(true);
        }
        else
        {
            // If outside radius, enable script2 and disable script1
            ToggleScripts(false);
        }
    }

    // Function to toggle between the two scripts
    private void ToggleScripts(bool isInRange)
    {
        if (isInRange)
        {
            if (script1 != null)
                script1.enabled = true;
            
            if (script2 != null)
                script2.enabled = false;
        }
        else
        {
            if (script1 != null)
                script1.enabled = false;

            if (script2 != null)
                script2.enabled = true;
        }
    }
}
