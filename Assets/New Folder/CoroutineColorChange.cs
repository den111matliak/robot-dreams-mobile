using UnityEngine;
using System.Collections;

public class CoroutineColorChange : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 2f;

    private Renderer rend;
    private Material mat;

    private Color[] colors = new Color[]
    {
        Color.blue,
        Color.yellow,
        Color.green
    };

    private void Start()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;
        StartCoroutine(CycleColors());
    }

    private IEnumerator CycleColors()
    {
        int currentIndex = 0;

        while (true)
        {
            Color fromColor = colors[currentIndex];
            Color toColor = colors[(currentIndex + 1) % colors.Length];

            yield return StartCoroutine(LerpColor(fromColor, toColor, transitionDuration));

            currentIndex = (currentIndex + 1) % colors.Length;
        }
    }

    private IEnumerator LerpColor(Color from, Color to, float duration)
    {
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime / duration;
            mat.color = Color.Lerp(from, to, timer);
            yield return null;
        }

        mat.color = to;
    }
}
