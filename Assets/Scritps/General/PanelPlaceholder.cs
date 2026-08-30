using UnityEngine;
using System.Collections;

public class PanelFadeOut : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f; // Tempo para sumir

    private CanvasGroup canvasGroup;

    void Start()
    {
        // PEGA O CanvasGroup (ou adiciona se não tiver)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // COMEÇA O FADE OUT
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float startAlpha = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}