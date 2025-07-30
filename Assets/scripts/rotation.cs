using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 90f, 0);
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}