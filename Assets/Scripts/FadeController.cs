using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;
    private Image fadeImage;
    private float fadeDuration = 0.5f;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            fadeImage = GetComponentInChildren<Image>();
        } else {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeOut() {
        yield return StartCoroutine(Fade(0f, 1f));
    }

    public IEnumerator FadeIn() {
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to) {
        float timer = 0f;
        Color color = fadeImage.color;
        while (timer < fadeDuration) {
            float alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            timer += Time.deltaTime;
            yield return null;
        }
        color.a = to;
        fadeImage.color = color;
    }
}
