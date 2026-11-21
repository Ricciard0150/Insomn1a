using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class SceneFadeIn : MonoBehaviour
{
    [Header("Configurações de Fade")]
    [Tooltip("Tempo que a tela fica parada no preto antes de começar a clarear")]
    [SerializeField] private float tempoHold = 0.5f;

    [Tooltip("Tempo que a tela leva para clarear")]
    [SerializeField] private float tempoFade = 1.5f;

    [Header("Timeline")]
    [Tooltip("Arraste o PlayableDirector da Timeline que deve tocar após o clareamento")]
    [SerializeField] private PlayableDirector timelineToPlay;

    private Image fadeImage;
    private Canvas fadeCanvas;

    void Awake()
    {
        CriarCanvasDeFade();

        if (timelineToPlay != null)
            timelineToPlay.Stop();
    }

    void Start()
    {
        StartCoroutine(FazerFadeETocarTimeline());
    }

    private void CriarCanvasDeFade()
    {
        GameObject canvasGO = new GameObject("FadeCanvas_CenaStart");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.raycastTarget = false;

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Começa PRETO
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FazerFadeETocarTimeline()
    {
        if (fadeImage == null) yield break;

        // ✅ NOVO: Espera o tempo de Hold com a tela totalmente preta
        yield return new WaitForSeconds(tempoHold);

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < tempoFade)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / tempoFade);

            // Correção de Gamma para o fade ficar suave
            float alphaCorrigido = 1f - (t * t);

            color.a = alphaCorrigido;
            fadeImage.color = color;

            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;

        if (fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);

        if (timelineToPlay != null)
        {
            timelineToPlay.Play();
        }
    }

    private void OnDestroy()
    {
        if (fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);
    }
}