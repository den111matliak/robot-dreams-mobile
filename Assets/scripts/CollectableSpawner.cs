using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    public GameObject collectablePrefab;

    void Start()
    {
        Transform spawnPointsParent = transform.Find("CollectableSpawnPoints");
        if (spawnPointsParent != null)
        {
            foreach (Transform point in spawnPointsParent)
            {
                Instantiate(collectablePrefab, point.position, point.rotation, transform);
            }
        }
    }
}