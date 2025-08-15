using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DonutDestroyer : MonoBehaviour
{
    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // реагуємо або на тег Player, або на наявність PlayerController
        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
        {
            Destroy(gameObject);
        }
    }
}
