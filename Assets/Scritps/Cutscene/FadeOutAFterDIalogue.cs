using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class FadeOutAfterDialogue : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Arraste o GameObject que tem o DialogueSystem")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Tooltip("Arraste o PlayableDirector da Timeline que deve tocar durante a piscada")]
    [SerializeField] private PlayableDirector timelineToPlay;

    [Header("Configurações da Piscada")]
    [Tooltip("Tempo que a tela leva para escurecer e clarear (ex: 0.2)")]
    [SerializeField] private float tempoPiscada = 0.2f;

    [Tooltip("Tempo que a tela fica totalmente preta antes de clarear (ex: 0.1)")]
    [SerializeField] private float tempoTelaPreta = 0.1f;

    [Header("Teleporte (Opcional)")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform pontoDeTeleporte;

    private bool playerNear = false;
    private bool jaFezFade = false;
    private Image fadeImage;
    private Canvas fadeCanvas;

    void Awake()
    {
        CriarCanvasDeFade();

        // Garante que a Timeline comece parada
        if (timelineToPlay != null)
            timelineToPlay.Stop();
    }

    private void CriarCanvasDeFade()
    {
        GameObject canvasGO = new GameObject("FadeCanvas_DialogueEnd");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = true;

            if (!jaFezFade)
            {
                StartCoroutine(EsperarDialogoAcabar());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = false;
        }
    }

    private IEnumerator EsperarDialogoAcabar()
    {
        if (dialogueSystem == null)
        {
            jaFezFade = true;
            yield return StartCoroutine(ExecutarPiscada());
            yield break;
        }

        yield return new WaitForSeconds(0.1f);

        float tempoEsperando = 0f;
        while (!dialogueSystem.DialogoAtivo && tempoEsperando < 2f)
        {
            tempoEsperando += Time.deltaTime;
            yield return null;
        }

        yield return new WaitWhile(() => dialogueSystem.DialogoAtivo);

        jaFezFade = true;
        yield return StartCoroutine(ExecutarPiscada());
    }

    private IEnumerator ExecutarPiscada()
    {
        Color color = fadeImage.color;

        // 1. Fade In (Escurecer rápido)
        float elapsed = 0f;
        while (elapsed < tempoPiscada)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / tempoPiscada;
            color.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;

        // --- TELA TOTALMENTE PRETA AQUI ---

        // 2. Teleporta o player (se configurado)
        if (player != null && pontoDeTeleporte != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = pontoDeTeleporte.position;
            }
            player.transform.position = pontoDeTeleporte.position;
        }

        // 3. ✅ TOCA A TIMELINE AQUI (com a tela preta, antes de clarear)
        if (timelineToPlay != null)
        {
            timelineToPlay.Play();
        }

        // 4. Espera um pouquinho com a tela preta para a Timeline começar
        yield return new WaitForSecondsRealtime(tempoTelaPreta);

        // 5. Fade Out (Clarear rápido) - Revela a Timeline já rodando
        elapsed = 0f;
        while (elapsed < tempoPiscada)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / tempoPiscada;
            color.a = Mathf.Lerp(1f, 0f, t);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;

        // A Timeline continua rodando sozinha após a tela clarear
    }

    private void OnDestroy()
    {
        if (fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);
    }
}