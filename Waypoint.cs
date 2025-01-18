using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public float PheromoneStrength = 0f;

    public void DecayPheromone(float decayRate)
    {
        // Decay pheromone over time
        PheromoneStrength = Mathf.Max(0f, PheromoneStrength - decayRate * Time.deltaTime);
    }
}
