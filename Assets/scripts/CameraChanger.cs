using UnityEngine;
using Unity.Cinemachine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera[] cameras;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateCamera(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateCamera(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivateCamera(2);
    }

    void ActivateCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == index)
            {
                cameras[i].Priority = 10;
            }
            else
            {
                cameras[i].Priority = 0;
            }
        }
    }
}