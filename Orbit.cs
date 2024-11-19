using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public Transform parentObject;   // Reference to the parent object
    public float moveDistance = 10f; // Distance to move from the parent (orbit radius)
    public float revolutionSpeed = 50f; // Speed of revolution around the parent
    public float detectionRange = 30f; // Range to start spinning (revolving)
    public float attackRange = 20f; // Range to fly towards the enemy
    public float flightSpeed = 10f;  // Speed at which to fly towards the enemy
    private GameObject enemy;        // Reference to the detected enemy
    private bool isRevolving = false;
    private bool isFlying = false;
    private Vector3 revolutionAxis = Vector3.up; // Axis of revolution

    void Start()
    {
        // Move the object away from the parent by 10 meters (initial orbit position)
        transform.position = parentObject.position + new Vector3(moveDistance, 0, 0);
    }

    void Update()
    {
        // Find the nearest enemy with the "Enemy" tag
        enemy = FindClosestEnemyWithTag("Enemy");

        if (enemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // If the enemy is within 30 meters, start revolving around the parent
            if (distanceToEnemy <= detectionRange && distanceToEnemy > attackRange)
            {
                isRevolving = true;
                isFlying = false;
            }
            // If the enemy is within 20 meters, fly towards the enemy
            else if (distanceToEnemy <= attackRange)
            {
                isFlying = true;
                isRevolving = false;
            }
            else
            {
                isRevolving = false;
                isFlying = false;
            }

            // Handle object revolving around the parent
            if (isRevolving)
            {
                RevolveAroundParent();
            }

            // Handle flying towards the enemy
            if (isFlying)
            {
                FlyTowardsEnemy();
            }
        }
        else
        {
            isRevolving = false;
            isFlying = false;
        }
    }

    // Function to find the closest enemy with the "Enemy" tag
    GameObject FindClosestEnemyWithTag(string tag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
        GameObject closestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject potentialEnemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, potentialEnemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                closestEnemy = potentialEnemy;
            }
        }

        return closestEnemy;
    }

    void RevolveAroundParent()
    {
        // Revolve the object around the parent object without rotating it
        transform.RotateAround(parentObject.position, revolutionAxis, revolutionSpeed * Time.deltaTime);

        // Make sure the object's local rotation remains unchanged (no self-rotation)
        transform.rotation = Quaternion.identity;
    }

    void FlyTowardsEnemy()
    {
        // Move towards the enemy at flight speed
        transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position, flightSpeed * Time.deltaTime);
    }
}
