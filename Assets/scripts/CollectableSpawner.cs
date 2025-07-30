using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField] private GameObject collectablePrefab;
    [SerializeField] private Transform[] spawnPoints;

    void Start()
    {
        foreach (Transform point in spawnPoints)
        {
            Instantiate(collectablePrefab, point.position, point.rotation, transform);
        }
    }
}
