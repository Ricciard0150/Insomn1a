using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Tempo que a tela leva para clarear (Fade In)")]
    [SerializeField] private float tempoFadeIn = 1.5f;

    [Tooltip("Tempo que a tela fica totalmente visível antes de sumir (opcional)")]
    [SerializeField] private float tempoEspera = 0.5f;

    [Tooltip("Se marcado, faz o Fade In (começa preto e clareia). Se desmarcado, faz o Fade Out (começa visível e escurece)")]
    [SerializeField] private bool fazerFadeIn = true;

    private Image fadeImage;
    private Canvas fadeCanvas;

    void Awake()
    {
        CriarCanvasDeFade();
    }

    void Start()
    {
        StartCoroutine(AnimarFade());
    }

    private void CriarCanvasDeFade()
    {
        // Cria o Canvas por cima de tudo
        GameObject canvasGO = new GameObject("FadeCanvas_CenaStart");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999; // Fica por cima de toda a UI

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Cria a Imagem preta
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.raycastTarget = false;

        // Estica a imagem para cobrir a tela inteira
        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // Define a cor inicial baseada na configuração
        if (fazerFadeIn)
            fadeImage.color = new Color(0, 0, 0, 1); // Começa PRETO
        else
            fadeImage.color = new Color(0, 0, 0, 0); // Começa TRANSPARENTE
    }

    private IEnumerator AnimarFade()
    {
        if (fadeImage == null) yield break;

        // Se for Fade Out, espera um pouquinho antes de começar a escurecer
        if (!fazerFadeIn)
            yield return new WaitForSeconds(tempoEspera);

        float startAlpha = fazerFadeIn ? 1f : 0f;
        float endAlpha = fazerFadeIn ? 0f : 1f;

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < tempoFadeIn)
        {
            elapsed += Time.unscaledDeltaTime; // unscaled para funcionar mesmo se o jogo pausar
            float t = elapsed / tempoFadeIn;

            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;

            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;

        // Se for Fade In, destrói o Canvas no final para liberar memória
        if (fazerFadeIn && fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);
    }

    private void OnDestroy()
    {
        if (fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);
    }
}