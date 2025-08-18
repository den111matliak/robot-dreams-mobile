using UnityEngine;
using JSAM;

[DisallowMultipleComponent]
public class JumpSFX : MonoBehaviour
{
    [SerializeField] private Sounds jumpSFX;
    [SerializeField] private bool spatialize = true;
    [SerializeField] private float minInterval = 0.2f;
    private float lastTime;

    public void OnJumpHappened()
    {
        if (Time.time - lastTime < minInterval) return;
        lastTime = Time.time;

        if (spatialize) AudioManager.PlaySound(jumpSFX, transform);
        else AudioManager.PlaySound(jumpSFX);
    }
}
