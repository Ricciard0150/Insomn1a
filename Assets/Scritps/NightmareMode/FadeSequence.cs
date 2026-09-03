using UnityEngine;
using System.Collections;

public class SimpleFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private GameObject panel; // O Panel que vai aparecer (fundo preto)
    [SerializeField] private float fadeDuration = 1f; // Duração do fade
    [SerializeField] private bool triggerOnce = true; // Só ativa uma vez
    [SerializeField] private float delayToStart = 0f; // Delay antes de começar

    private bool triggered = false;
    private bool isFading = false;
    private CanvasGroup canvasGroup;

    void Start()
    {
        if (panel != null)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>();
            }

            // Começa completamente transparente
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            panel.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IStatusPlayer player)) return;
        if (triggerOnce && triggered) return;
        if (panel == null) return;
        if (isFading) return;

        triggered = true;
        isFading = true;

        if (delayToStart > 0)
        {
            Invoke(nameof(StartFade), delayToStart);
        }
        else
        {
            StartFade();
        }
    }

    void StartFade()
    {
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        float elapsedTime = 0f;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            // Curva suave
            t = t * t * (3f - 2f * t);

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        // Fica PRETO TOTAL (Alpha = 1)
        canvasGroup.alpha = 1f;
        isFading = false;
    }

    public void ResetTrigger()
    {
        triggered = false;
        isFading = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }
}