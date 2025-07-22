using UnityEngine;

public class Trigger : MonoBehaviour
{
    public GameObject car;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("I am triggered by" + other.name);

        if (car != null)
        {
            car.SetActive(true); // Машинка падає
        }
    }


    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exited by: " + other.name);
    }

}
