using UnityEngine;

/// <summary>
/// Rotates this object so its local +Z axis points toward the target (player camera).
/// If no target is assigned in the Inspector, it automatically grabs Camera.main.
/// Add a small rotationOffset if the mesh’s “forward” isn’t the iris/pupil direction.
/// </summary>
public class EyeLookAtYou : MonoBehaviour
{
    [Tooltip("Transform to look at (leave empty to auto-use Main Camera).")]
    public Transform target;

    [Tooltip("Extra Euler rotation applied after LookAt (use if the pupil isn’t forward).")]
    public Vector3 rotationOffset;

    void Awake()
    {
        // If no explicit target was set, fall back to the main camera in the scene.
        if (target == null && Camera.main != null)
            target = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (target == null) return;                 // nothing to look at or do nothing

        // Point the pupils local +Z axis at the target.
        transform.LookAt(target);

        // Apply any tweak the artist needs so the iris lines up perfectly.
        transform.Rotate(rotationOffset, Space.Self);
    }
}