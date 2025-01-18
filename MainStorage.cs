using UnityEngine;
using System.Collections;  // Required for using coroutines
using TMPro;  // Required for using TextMeshPro

public class MainStorage : MonoBehaviour
{
    public int totalDeposited = 0;  // Tracks the total number of items deposited
    public int escortsSpawned = 0;  // Tracks how many Escorts have been spawned
    public GameObject prefabToSpawn; // Reference to the prefab to be spawned
    public float fadeDuration = 2f; // Duration for the fade-in effect

    public TMP_Text depositText; // UI TextMeshProUGUI field to display the deposited count
    public TMP_Text escortsText; // UI TextMeshProUGUI field to display the number of Escorts spawned

    // Method to call when items are deposited
    public void TrackDeposit(int itemsDeposited)
    {
        totalDeposited += itemsDeposited;  // Add the deposited items to the total
        Debug.Log("Total Deposited: " + totalDeposited);

        // Check if the totalDeposited has reached or surpassed 10
        if (totalDeposited >= 10)
        {
            // Spawn the prefab
            SpawnPrefab();

            // Display the totalDeposited and escorts spawned on the UI
            UpdateUIText();

            // Do not deduct 10, as per the new requirement
        }
    }

    // Method to check if the deposit count has reached or surpassed the threshold
    public bool IsCounterReached(int threshold)
    {
        return totalDeposited >= threshold; // Check if totalDeposited has reached the threshold
    }

    // Method to spawn the prefab
    private void SpawnPrefab()
    {
        if (prefabToSpawn != null)
        {
            // Instantiate the prefab at the position of the MainStorage object
            GameObject spawnedObject = Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            Debug.Log("Prefab spawned at position: " + transform.position);

            // Start the fade-in effect on the spawned object
            StartCoroutine(FadeIn(spawnedObject));

            // Increment the number of Escorts spawned
            escortsSpawned++;
        }
        else
        {
            Debug.LogError("Prefab to spawn is not assigned!");
        }
    }

    // Coroutine to fade in the object over time
    private IEnumerator FadeIn(GameObject spawnedObject)
    {
        // Get the Renderer component of the spawned object
        Renderer renderer = spawnedObject.GetComponent<Renderer>();
        
        if (renderer != null)
        {
            Material material = renderer.material;  // Get the material of the object

            // Ensure the material has transparency enabled
            if (material.HasProperty("_Color"))
            {
                Color color = material.color;
                color.a = 0;  // Start with fully transparent
                material.color = color;

                float timeElapsed = 0f;

                // Gradually increase the alpha value to make the object visible
                while (timeElapsed < fadeDuration)
                {
                    timeElapsed += Time.deltaTime;
                    color.a = Mathf.Lerp(0, 1, timeElapsed / fadeDuration);  // Lerp from 0 to 1 over the fade duration
                    material.color = color;
                    yield return null;
                }

                // Ensure the final alpha is fully visible
                color.a = 1;
                material.color = color;
            }
            else
            {
                Debug.LogWarning("Material does not have a _Color property for transparency!");
            }
        }
        else
        {
            Debug.LogWarning("Spawned object does not have a Renderer component!");
        }
    }

    // Method to update the UI text fields
    private void UpdateUIText()
    {
        // Display the total deposited amount and number of escorts spawned
        if (depositText != null)
        {
            depositText.text = "Total Deposited: " + totalDeposited;
        }

        if (escortsText != null)
        {
            escortsText.text = "Escorts Spawned: " + escortsSpawned;
        }
    }
}
