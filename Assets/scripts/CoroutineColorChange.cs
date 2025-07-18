using UnityEngine;
using System.Collections;

public class ColorCycler : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 2f;
    [SerializeField] private Renderer rend;

    private Material mat;

    private Color[] colors = new Color[]
    {
        Color.blue,
        Color.yellow,
        Color.green
    };

    private void Start()
    {
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
        float elapsed = 0f; // Тутка задаємо стартовий час — скільки вже пройшло часу з початку зміни кольору

        while (elapsed < duration) // допоки елапсед меньше дюрейшену який ми задаємо в інспекторі (2 по дефолту)
        {
            float t = elapsed / duration;
            /*
            Тутка рахуємо прогрес анімації
            якщо elapsed = 0   → t = 0 / 2   = 0     (початок)
            якщо elapsed = 1   → t = 1 / 2   = 0.5   (половина)
            якщо elapsed = 2   → t = 2 / 2   = 1     (кінець)
            */
            mat.color = Color.Lerp(from, to, t); // тутка плавна зміна кольору від і до за t час
            elapsed += Time.deltaTime; // додаємо до 0 кожен кадр якусь долю секнди в залежності від фпс
            yield return null;
        }

        mat.color = to; // коли elapsed >= duration ми показуємо повністю натупний колір
    }
}
