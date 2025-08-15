using UnityEngine;

[DisallowMultipleComponent]
public class PlayerDamage : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [Header("Detection (Layers only)")]
    [Tooltip("Шари, які вважаємо за урон (наприклад, Obstacle).")]
    [SerializeField] private LayerMask damageLayers;

    [Header("Hit logic")]
    [SerializeField] private float hitCooldown = 0.6f;
    [SerializeField] private bool lockMovementOnHit = false;

    private const string HitTrigger = "isHit"; // тригер в Animator
    private static readonly int HitTriggerHash = Animator.StringToHash(HitTrigger);

    private float _nextHitTime;
    private bool _movementLocked;

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    void OnTriggerEnter(Collider other) { TryHit(other); }
    void OnCollisionEnter(Collision other) { TryHit(other.collider); }

    private void TryHit(Collider other)
    {
        if (!IsInDamageLayers(other.gameObject.layer)) return;
        if (Time.time < _nextHitTime) return;

        _nextHitTime = Time.time + hitCooldown;

        if (animator) animator.SetTrigger(HitTriggerHash);

        if (lockMovementOnHit)
        {
            _movementLocked = true;
            Invoke(nameof(UnlockMove), hitCooldown);
        }
    }

    private bool IsInDamageLayers(int layer)
    {
        return (damageLayers.value & (1 << layer)) != 0;
    }

    private void UnlockMove() => _movementLocked = false;

    // Якщо хочеш блокувати керування у PlayerController:
    public bool MovementLocked => _movementLocked;
}
