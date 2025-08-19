using UnityEngine;

[DisallowMultipleComponent]
public class PlayerDamage : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [Header("Hit logic")]
    [SerializeField] private float hitCooldown = 0.6f;      // prevents rapid re-hits
    [SerializeField] private bool lockMovementOnHit = false; // optional: pause movement while hit anim plays

    private const string HitTrigger = "isHit";
    private static readonly int HitTriggerHash = Animator.StringToHash(HitTrigger);

    private float _nextHitTime;
    private bool _movementLocked;

    void Reset()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    /// <summary>
    /// Called by hazards (cars) when they hit the player. Returns true if the hit was accepted.
    /// </summary>
    public bool ApplyHit()
    {
        if (Time.time < _nextHitTime) return false;

        _nextHitTime = Time.time + hitCooldown;

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator) animator.SetTrigger(HitTriggerHash);

        if (lockMovementOnHit)
        {
            _movementLocked = true;
            Invoke(nameof(UnlockMove), hitCooldown);
        }

        return true;
    }

    private void UnlockMove() => _movementLocked = false;

    /// <summary>
    /// Read this from PlayerController if you want to pause movement during hit.
    /// </summary>
    public bool MovementLocked => _movementLocked;
}
