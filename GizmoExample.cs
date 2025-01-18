using UnityEngine;

public class GizmoExample : MonoBehaviour
{
    public Color gizmoColor = Color.green;  // Color of the gizmo
    public float gizmoSize = 1f;            // Size of the gizmo

    // This method will be called in the Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;  // Set the gizmo color
        Gizmos.DrawWireSphere(transform.position, gizmoSize);  // Draw a wireframe sphere at the object's position
    }
}
