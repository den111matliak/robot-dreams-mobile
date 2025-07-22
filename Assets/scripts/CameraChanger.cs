using UnityEngine;
using Unity.Cinemachine;

public class CameraChanger : MonoBehaviour
{
    public CinemachineCamera cam1;
    public CinemachineCamera cam2;
    public CinemachineCamera cam3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        ActivateCamera(cam1);

        if (Input.GetKeyDown(KeyCode.Alpha2))
        ActivateCamera(cam2);

        if (Input.GetKeyDown(KeyCode.Alpha3))
        ActivateCamera(cam3);
    }

    void ActivateCamera(CinemachineCamera activeCam)
    {
        if (cam1 == activeCam)
            cam1.Priority = 10;
        else
            cam1.Priority = 0;

        if (cam2 == activeCam)
            cam2.Priority = 10;
        else
            cam2.Priority = 0;

        if (cam3 == activeCam)
            cam3.Priority = 10;
        else
            cam3.Priority = 0;
    }
}