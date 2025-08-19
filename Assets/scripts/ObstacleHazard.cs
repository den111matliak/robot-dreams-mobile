using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObstacleHazard : MonoBehaviour
{
    private void Reset()
    {
        // Recommended for endless runners:
        // Make the collider a trigger and add a kinematic rigidbody on the obstacle root.
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        if (!TryGetComponent<Rigidbody>(out var rb))
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // No tags required. If the other object (or its parents) has PlayerDamage, apply hit.
        var damage = other.GetComponentInParent<PlayerDamage>();
        if (damage != null) damage.ApplyHit();
    }

    // If you decide to use non-trigger collisions instead (Is Trigger OFF), keep this too:
    private void OnCollisionEnter(Collision other)
    {
        var damage = other.collider.GetComponentInParent<PlayerDamage>();
        if (damage != null) damage.ApplyHit();
    }
}
