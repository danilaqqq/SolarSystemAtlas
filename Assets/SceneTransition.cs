using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    // ����� ����������
    public float fadeDuration = 1f;
    private bool isFading = false;

    void Start()
    {
        // ���������� ��� ������
        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeOutAndLoad(sceneName));
        }
    }

    // ���������
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

    // ���������� � ����� �����
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

    // ��������� ������������ Image
    void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = Mathf.Clamp01(alpha);
        fadeImage.color = color;
    }
}
