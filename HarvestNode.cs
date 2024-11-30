using UnityEngine;
using UnityEngine.Events;

public class HarvestNode : MonoBehaviour
{
    public int maxHarvests = 5; // Maximum times the node can be harvested
    private int harvestCount = 0; // Current harvest count

    // Create UnityEvents to broadcast the status
    public UnityEvent onHarvestable; // Broadcast when the node is harvestable
    public UnityEvent onDepleted;    // Broadcast when the node is depleted

    public bool Harvest()
    {
        if (harvestCount < maxHarvests)
        {
            harvestCount++;
            Debug.Log($"Harvested {harvestCount}/{maxHarvests}");

            // Broadcast the harvestable event
            if (onHarvestable != null)
                onHarvestable.Invoke();

            return true;
        }
        
        // Broadcast the depleted event when the node is depleted
        if (onDepleted != null)
            onDepleted.Invoke();

        Debug.Log("Node depleted.");
        return false;
    }
}
