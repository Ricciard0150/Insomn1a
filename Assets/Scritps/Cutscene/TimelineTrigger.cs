using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimelineTrigger : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O PlayableDirector que contém a sua Timeline")]
    [SerializeField] private PlayableDirector timelineDirector;

    [Header("Teleporte")]
    [Tooltip("O Transform para onde o player vai ser teleportado")]
    [SerializeField] private Transform pontoDeTeleporte;

    [Tooltip("Arraste o GameObject do Player aqui")]
    [SerializeField] private GameObject player;

    [Header("Configurações de Fade")]
    [Tooltip("Tempo que a tela leva para ficar preta (Fade In)")]
    [SerializeField] private float tempoEscurecer = 1f;

    [Tooltip("Tempo que a tela fica preta antes de teleportar")]
    [SerializeField] private float tempoTelaPreta = 0.5f;

    [Tooltip("Tempo que a tela leva para clarear (Fade Out)")]
    [SerializeField] private float tempoClarear = 1f;

    [Header("Configurações Gerais")]
    [Tooltip("Se marcado, só acontece uma vez")]
    [SerializeField] private bool triggerOnlyOnce = true;

    [Tooltip("Bloquear o movimento do player durante a cutscene?")]
    [SerializeField] private bool freezePlayer = true;

    [SerializeField] private TopDownMovement playerMovement;

    private bool playerNear = false;
    private bool hasTriggered = false;
    private bool isPlaying = false;

    private Image fadeImage;
    private Canvas fadeCanvas;

    void Awake()
    {
        if (timelineDirector != null)
            timelineDirector.Stop();

        CriarCanvasDeFade();
    }

    private void CriarCanvasDeFade()
    {
        GameObject canvasGO = new GameObject("FadeCanvas_Cutscene");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.raycastTarget = false;

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

        // 1. Fade In (Escurecer a tela)
        yield return StartCoroutine(Fade(0f, 1f, tempoEscurecer));

        // 2. Espera um pouquinho com a tela preta
        yield return new WaitForSeconds(tempoTelaPreta);

        // 3. Teleporta o player (invisível para o jogador, pois a tela está preta)
        if (player != null && pontoDeTeleporte != null)
        {
            player.transform.position = pontoDeTeleporte.position;
        }

        // 4. Fade Out (Clarear a tela mostrando o player no novo lugar)
        yield return StartCoroutine(Fade(1f, 0f, tempoClarear));

        // 5. Toca a Timeline
        if (timelineDirector != null)
        {
            timelineDirector.Play();

            // Espera a Timeline terminar sozinha
            yield return new WaitWhile(() => timelineDirector.state == PlayState.Playing);
        }
        else
        {
            Debug.LogWarning("[TimelineTrigger] Nenhuma Timeline atribuída!");
            yield return new WaitForSeconds(2f);
        }

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
            elapsed += Time.unscaledDeltaTime;
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

    private void OnDestroy()
    {
        if (fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);
    }
}