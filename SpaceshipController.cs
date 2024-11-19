using UnityEngine;

public class AdvancedVehicleController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f; // Base forward speed
    public float boostMultiplier = 2f; // Boost multiplier for speed
    public float pitchSpeed = 50f; // Speed for pitch rotation (W/S)
    public float rollSpeed = 50f; // Speed for roll rotation (A/D)

    [Header("Boost Settings")]
    public float boostDuration = 3f; // How long the boost lasts
    public float boostCooldown = 5f; // How long before boost recharges

    private bool isBoosting = false; // Is boost currently active?
    private bool boostOnCooldown = false; // Is boost recharging?
    private float boostTimer = 0f; // Timer for boost duration
    private float cooldownTimer = 0f; // Timer for boost cooldown

    private void Update()
    {
        HandleRotation();
        HandleMovement();
        HandleBoost();
    }

    private void HandleRotation()
    {
        float pitch = 0f; // For pitch (W/S)
        float roll = 0f; // For roll (A/D)

        // Get input for pitch (W/S) and roll (A/D)
        if (Input.GetKey(KeyCode.W))
            pitch = 1f;
        else if (Input.GetKey(KeyCode.S))
            pitch = -1f;

        if (Input.GetKey(KeyCode.D))
            roll = -1f;
        else if (Input.GetKey(KeyCode.A))
            roll = 1f;

        // Apply pitch and roll rotation
        transform.Rotate(Vector3.right * pitch * pitchSpeed * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.forward * roll * rollSpeed * Time.deltaTime, Space.Self);
    }

    private void HandleMovement()
    {
        // Determine speed based on whether boost is active
        float currentSpeed = isBoosting ? speed * boostMultiplier : speed;

        // Move the object forward in the direction it is pointing
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);
    }

    private void HandleBoost()
    {
        if (isBoosting)
        {
            // Boost timer countdown
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosting = false;
                boostOnCooldown = true;
                cooldownTimer = boostCooldown; // Start cooldown
            }
        }
        else if (boostOnCooldown)
        {
            // Cooldown timer countdown
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                boostOnCooldown = false; // Boost is ready again
            }
        }

        // Activate boost if Shift is pressed and boost is ready
        if (Input.GetKey(KeyCode.LeftShift) && !boostOnCooldown && !isBoosting)
        {
            isBoosting = true;
            boostTimer = boostDuration; // Start boost duration
        }
    }
}
