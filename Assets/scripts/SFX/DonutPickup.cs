using UnityEngine;
using JSAM;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class DonutPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private int scoreValue = 1;

    [SerializeField] private Sounds donutSfx;
    [SerializeField] private bool spatialize = true;

    public static event Action<int> OnDonutCollected;

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

        OnDonutCollected?.Invoke(scoreValue);
        Destroy(gameObject);
    }
}
