using UnityEngine;

public class CameraFollowTree : MonoBehaviour
{
    public Transform target; // The current tree to follow
    public Vector3 offset = new Vector3(0, 5f, -10f); // Adjust for your scene
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
