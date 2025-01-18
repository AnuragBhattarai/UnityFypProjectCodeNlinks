using System.Collections.Generic;
using UnityEngine;

public class ACOController : MonoBehaviour
{
    public float pheromoneStrength = 1f; // Strength of pheromone trail
    public float pheromoneDecayRate = 0.05f; // Rate of pheromone decay between waypoints
    private Dictionary<Agent, List<Vector3>> agentPheromoneTrails = new Dictionary<Agent, List<Vector3>>(); // Separate trail for each agent

    void Update()
    {
        // Update pheromone decay for each agent
        foreach (var agentTrail in agentPheromoneTrails)
        {
            UpdatePheromoneDecay(agentTrail.Key);
        }
    }

    // Method to add pheromone trail positions from the agent
    public void DepositPheromone(Agent agent, Vector3 fromPosition, Vector3 toPosition)
    {
        // Ensure each agent has its own trail
        if (!agentPheromoneTrails.ContainsKey(agent))
        {
            agentPheromoneTrails[agent] = new List<Vector3>();
        }

        // Add the pheromone positions to the agent's trail
        agentPheromoneTrails[agent].Add(fromPosition);
        agentPheromoneTrails[agent].Add(toPosition);
    }

    // Method to update pheromone decay
    private void UpdatePheromoneDecay(Agent agent)
    {
        if (agentPheromoneTrails.ContainsKey(agent))
        {
            List<Vector3> pheromoneTrailPositions = agentPheromoneTrails[agent];

            // Decay pheromone along the trail positions
            for (int i = 0; i < pheromoneTrailPositions.Count; i++)
            {
                // Gradually reduce pheromone strength over time
                pheromoneStrength -= pheromoneDecayRate * Time.deltaTime;
                pheromoneStrength = Mathf.Max(pheromoneStrength, 0); // Ensure pheromone doesn't go negative
            }
        }
    }

    // Visualize pheromone trail using Gizmos (for debugging/visualization)
    private void OnDrawGizmos()
    {
        // Draw pheromone trails for each agent separately
        foreach (var agentTrail in agentPheromoneTrails)
        {
            List<Vector3> pheromoneTrailPositions = agentTrail.Value;
            if (pheromoneTrailPositions.Count > 1)
            {
                for (int i = 0; i < pheromoneTrailPositions.Count - 1; i++)
                {
                    float alpha = Mathf.Clamp01(1f - (i / (float)pheromoneTrailPositions.Count));
                    Color lineColor = new Color(1f, 0f, 0f, alpha); // Red color with fading alpha

                    Gizmos.color = lineColor;
                    Gizmos.DrawLine(pheromoneTrailPositions[i], pheromoneTrailPositions[i + 1]);
                }
            }
        }
    }
}
