using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    // Время затемнения
    public float fadeDuration = 1f;
    private bool isFading = false;

    void Start()
    {
        // Затемнение при старте
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeOutAndLoad(sceneName));
        }
    }

    // Появление
    IEnumerator FadeIn()
    {
        isFading = true;
        float timer = fadeDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float alpha = timer / fadeDuration;
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0);
        isFading = false;
    }

    // Затемнение и смена сцены
    IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(1);

        SceneManager.LoadScene(sceneName);
    }

    // Установка прозрачности Image
    void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = Mathf.Clamp01(alpha);
        fadeImage.color = color;
    }
}
