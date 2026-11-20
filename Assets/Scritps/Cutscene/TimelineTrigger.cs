using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimelineTrigger : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O PlayableDirector que contém a sua Timeline")]
    [SerializeField] private PlayableDirector timelineDirector;

    [Header("Configurações")]
    [Tooltip("Tempo que a tela leva para ficar preta")]
    [SerializeField] private float fadeOutDuration = 1f;

    [Tooltip("Tempo que a tela fica preta antes da Timeline começar")]
    [SerializeField] private float tempoTelaPreta = 0.5f;

    [Tooltip("Tempo que a tela leva para clarear depois da Timeline")]
    [SerializeField] private float fadeInDuration = 1f;

    [Tooltip("Se marcado, só acontece uma vez")]
    [SerializeField] private bool triggerOnlyOnce = true;

    [Tooltip("Bloquear o movimento do player durante a cutscene?")]
    [SerializeField] private bool freezePlayer = true;

    [Header("Player (Opcional)")]
    [SerializeField] private TopDownMovement playerMovement;

    private bool playerNear = false;
    private bool hasTriggered = false;
    private bool isPlaying = false;

    // Variáveis internas para o Fade
    private Image fadeImage;
    private Canvas fadeCanvas;

    void Awake()
    {
        // Garante que a Timeline comece parada
        if (timelineDirector != null)
            timelineDirector.Stop();

        CriarCanvasDeFade();
    }

    private void CriarCanvasDeFade()
    {
        // 1. Cria o Canvas que vai ficar por cima de TUDO (Order 999)
        GameObject canvasGO = new GameObject("FadeCanvas_Cutscene");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // 2. Cria a Imagem preta
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0); // Começa transparente
        fadeImage.raycastTarget = false;

        // 3. Faz a imagem cobrir a tela inteira
        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void Update()
    {
        if (playerNear && !hasTriggered && !isPlaying)
        {
            // Se a Timeline já estiver tocando, não faz nada
            if (timelineDirector != null && timelineDirector.state == PlayState.Playing)
                return;

            StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene()
    {
        isPlaying = true;

        if (triggerOnlyOnce)
            hasTriggered = true;

        // Bloqueia o player
        if (freezePlayer && playerMovement != null)
            playerMovement.SetCanMove(false);

        // 1. Fade Out (Escurecer a tela)
        yield return StartCoroutine(Fade(0f, 1f, fadeOutDuration));

        // 2. Espera um pouquinho no escuro
        yield return new WaitForSeconds(tempoTelaPreta);

        // 3. Toca a Timeline
        if (timelineDirector != null)
        {
            timelineDirector.Play();

            // 4. Espera a Timeline terminar sozinha
            yield return new WaitWhile(() => timelineDirector.state == PlayState.Playing);
        }
        else
        {
            Debug.LogWarning("[TimelineTrigger] Nenhuma Timeline atribuída!");
            yield return new WaitForSeconds(2f);
        }

        // 5. Fade In (Clarear a tela)
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        // Libera o player
        if (freezePlayer && playerMovement != null)
            playerMovement.SetCanMove(true);

        isPlaying = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // unscaled para funcionar mesmo se o jogo pausar
            float t = elapsed / duration;

            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;

            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = false;
        }
    }

    // Limpa o Canvas criado por código quando o objeto for destruído
    private void OnDestroy()
    {
        if (fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);
    }
}