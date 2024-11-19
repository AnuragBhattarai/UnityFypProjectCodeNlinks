using UnityEngine;

public class ObjectX : MonoBehaviour
{
    public Transform pointA;          // Point A
    public Transform pointB;          // Point B
    public GameObject objectZPrefab;  // Object Z to be duplicated and carried

    private Transform target;         // Current target (Point A or Point B)
    private GameObject objectZ;       // Duplicated Object Z
    private int reachCount = 0;       // Counter for how many times ObjectX reaches Point A
    private bool isCarryingZ = false; // Flag to check if Object X is carrying Object Z
    private bool isIdle = false;      // Flag to check if ObjectX is idle (stopped at PointB)

    public float speed = 2.0f;        // Movement speed of Object X

    void Start()
    {
        // Check if references are assigned in the Inspector
        if (pointA == null || pointB == null)
        {
            Debug.LogError("Point A or Point B is not assigned in the Inspector!");
            return;
        }

        if (objectZPrefab == null)
        {
            Debug.LogError("Object Z Prefab is not assigned in the Inspector!");
            return;
        }

        target = pointA;  // Start by moving towards Point A
    }

    void Update()
    {
        // If ObjectB exists, continue moving back and forth between PointA and PointB
        if (GameObject.Find("ObjectB") != null)
        {
            if (isIdle)
            {
                isIdle = false; // Reactivate movement if ObjectB exists
            }

            // Ensure that ObjectX is moving towards its target
            if (target != null)
            {
                MoveTowardsTarget();

                // Check if Object X has reached its target
                if (Vector3.Distance(transform.position, target.position) < 0.1f)
                {
                    if (target == pointA)
                    {
                        reachCount++;
                        Debug.Log("ObjectX has reached Point A " + reachCount + " times.");

                        if (reachCount >= 3)
                        {
                            // When ObjectX reaches Point A 3 times, destroy ObjectB
                            DestroyObjectB();  // This will handle the destruction of ObjectB
                            return;
                        }

                        // Duplicate Object Z if it's not already carried
                        if (!isCarryingZ)
                        {
                            objectZ = Instantiate(objectZPrefab, transform.position, Quaternion.identity);
                            objectZ.transform.SetParent(transform);
                            objectZ.transform.localPosition = Vector3.zero;
                            isCarryingZ = true;
                        }

                        target = pointB;  // Set target to Point B for return trip
                    }
                    else if (target == pointB)
                    {
                        // After reaching Point B, set target to Point A
                        target = pointA;  // Set target to Point A for the next cycle
                        Debug.Log("ObjectX has reached Point B.");
                    }
                }
            }
        }
        else
        {
            // If ObjectB is destroyed, make ObjectX move to PointB first before going idle
            if (!isIdle)
            {
                if (target != pointB)
                {
                    // Move ObjectX to PointB before entering idle state
                    target = pointB;
                }
                else
                {
                    isIdle = true;  // Enter idle state after reaching PointB
                    target = null;  // Stop movement
                    Debug.Log("ObjectX is idle because ObjectB is destroyed.");
                }
            }
        }
    }

    // Move Object X towards the target position
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    // This function destroys ObjectB when ObjectX reaches Point A 3 times
    private void DestroyObjectB()
    {
        // If ObjectB exists, destroy it
        GameObject objectB = GameObject.Find("ObjectB");
        if (objectB != null)
        {
            Debug.Log("ObjectB has been destroyed.");
            Destroy(objectB);  // Destroy the actual ObjectB in the scene
        }
        else
        {
            Debug.LogWarning("ObjectB not found. Can't destroy it.");
        }

        // Set ObjectX to idle state once ObjectB is destroyed
        isIdle = true;
        target = pointB; // Make sure it moves to PointB first before stopping
    }
}
