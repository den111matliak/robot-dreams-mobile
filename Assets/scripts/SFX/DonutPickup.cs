using UnityEngine;
using JSAM;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class DonutPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    //[SerializeField] private int scoreValue = 1; // keep for future scoring

    [SerializeField] private Sounds donutSfx;
    [SerializeField] private bool spatialize = true;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (spatialize) AudioManager.PlaySound(donutSfx, transform);
        else AudioManager.PlaySound(donutSfx);

        // scoring can be added here later if needed
        Destroy(gameObject);
    }
}
