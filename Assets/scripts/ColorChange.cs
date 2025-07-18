using UnityEngine;

public class ColorSwitcher : MonoBehaviour
{
    [SerializeField] private float duration = 1f;

    private Material mat;
    private Color colorA = Color.yellow;
    private Color colorB = Color.red;
    private float timer = 0f;
    private bool goingToB = true;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        timer += Time.deltaTime / duration;

        if (goingToB)
        {
            mat.color = Color.Lerp(colorA, colorB, timer);
        }
        else
        {
            mat.color = Color.Lerp(colorB, colorA, timer);
        }

        if (timer >= 1f)
        {
            timer = 0f;
            goingToB = !goingToB;
        }
    }
}