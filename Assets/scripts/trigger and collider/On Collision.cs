using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public GameObject car1;
    public GameObject car2;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Cube")
        { 
            Debug.Log("I have collision with" + other.gameObject.name);

            if (car1 != null)
            {
                car1.SetActive(true); // 1 Машинка падає
            }
            if (car2 != null)
            {
                car2.SetActive(true); // 2 Машинка падає
            }
        }
    }


    private void OnCollisionExit(Collision other) 
    {
        Debug.Log("Collision exited with: " + other.gameObject.name);
    }

}