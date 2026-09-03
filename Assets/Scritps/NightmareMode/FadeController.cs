using UnityEngine;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer fadeSprite; 
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Colors")]
    [SerializeField] private Color fadeColor = Color.black;

    private void Start()
    {
        if (fadeSprite == null)
        {
            return;
        }

        Color c = fadeColor;
        c.a = 0f;
        fadeSprite.color = c;

    }

    public IEnumerator FadeToBlack(float duration)
    {
        if (fadeSprite == null)
        {
            yield break;
        }


        float elapsed = 0f;
        Color startColor = fadeSprite.color;
        Color targetColor = fadeColor;
        targetColor.a = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            fadeSprite.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        fadeSprite.color = targetColor;
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        if (fadeSprite == null)
        {            yield break;
        }
        float elapsed = 0f;
        Color startColor = fadeSprite.color;
        Color targetColor = fadeColor;
        targetColor.a = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            fadeSprite.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        fadeSprite.color = targetColor;
    }

    public IEnumerator FlashScreen(int flashes)
    {
        if (fadeSprite == null)
        {
            yield break;
        }

        Color visibleColor = fadeColor;
        visibleColor.a = 1f;

        Color invisibleColor = fadeColor;
        invisibleColor.a = 0f;

        for (int i = 0; i < flashes; i++)
        {
            fadeSprite.color = visibleColor;
            yield return new WaitForSeconds(flashDuration);

            fadeSprite.color = invisibleColor;
            yield return new WaitForSeconds(flashDuration);
        }

        fadeSprite.color = invisibleColor;
    }

    public void SetFadeColor(Color color)
    {
        fadeColor = color;
    }

    public void SetAlpha(float alpha)
    {
        if (fadeSprite == null) return;

        Color c = fadeSprite.color;
        c.a = Mathf.Clamp01(alpha);
        fadeSprite.color = c;
    }


}