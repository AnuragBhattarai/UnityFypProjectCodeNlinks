using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class PickupSystem : MonoBehaviour
{
    public float pickupRange = 2f; // Range within which the agent can pick up objects
    public int maxPickupAmount = 10; // Maximum number of objects the agent can pick up
    private int currentPickupCount = 0; // Current number of items picked up
    private int totalItemsDeposited = 0; // Tracks the total number of items deposited

    public TextMeshProUGUI pickupCountTMP; // TMP UI Text to display pickup count
    public TextMeshProUGUI totalDepositCountTMP; // TMP UI Text to display total deposited count

    // Expose the current pickup count so other scripts can access it
    public int CurrentPickupCount => currentPickupCount;

    private void Start()
    {
        UpdatePickupCountUI();
        UpdateTotalDepositCountUI();
    }

    private void Update()
    {
        if (currentPickupCount < maxPickupAmount)
        {
            CheckForPickup();
        }

        // Check for deposit
        CheckForDeposit();
    }

    private void CheckForPickup()
    {
        // Find all objects tagged as "Pickupable" within the pickup range
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider obj in objectsInRange)
        {
            if (obj.CompareTag("Pickupable"))
            {
                PickupObject(obj.gameObject);
                break; // Only pick up one object per frame
            }
        }
    }

    private void PickupObject(GameObject pickup)
    {
        // Increment the pickup count
        currentPickupCount++;

        // Handle the pickup logic (e.g., destroy the object, play sound, etc.)
        Destroy(pickup);

        // Update the TMP UI to show the new pickup count
        UpdatePickupCountUI();
        Debug.Log($"Picked up {currentPickupCount}/{maxPickupAmount} objects.");

        // Call UpdateSpeed in SpeedReducer to adjust the speed based on the current pickup count
        SpeedReducer speedReducer = GetComponent<SpeedReducer>();
        if (speedReducer != null)
        {
            speedReducer.UpdateSpeed(); // Update the speed in SpeedReducer
        }
    }

    private void CheckForDeposit()
    {
        // Find all objects tagged as "Deposit" within the deposit range
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, 2f); // Assuming the deposit range is 2

        foreach (Collider obj in objectsInRange)
        {
            if (obj.CompareTag("Deposit") && currentPickupCount > 0) // Only deposit if there's something to deposit
            {
                DepositObject();
                break; // Only deposit once per frame
            }
        }
    }

    private void DepositObject()
    {
        // Track the total number of items deposited
        totalItemsDeposited += currentPickupCount;

        // Reset the pickup count to 0 when depositing
        currentPickupCount = 0;

        // Update the TMP UI to show the reset pickup count and total deposited count
        UpdatePickupCountUI();
        UpdateTotalDepositCountUI();
        Debug.Log($"Deposited {totalItemsDeposited} items. Current pickup count reset to 0.");

        // Notify MainStorage script that items were deposited
        MainStorage mainStorage = FindObjectOfType<MainStorage>();
        if (mainStorage != null)
        {
            mainStorage.TrackDeposit(totalItemsDeposited); // Track the total deposited items
        }
    }

    private void UpdatePickupCountUI()
    {
        if (pickupCountTMP != null)
        {
            pickupCountTMP.text = $"Picked Up: {currentPickupCount}/{maxPickupAmount}";
        }
    }

    private void UpdateTotalDepositCountUI()
    {
        if (totalDepositCountTMP != null)
        {
            totalDepositCountTMP.text = $"Total Deposited: {totalItemsDeposited}";
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a gizmo to visualize the pickup range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
