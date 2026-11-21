using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Configurações de Cena")]
    [Tooltip("Nome EXATO da cena que será carregada")]
    [SerializeField] private string nomeDaCena = "NomeDaCena";

    [Tooltip("Tempo que a tela leva para ficar preta")]
    [SerializeField] private float tempoFade = 1.5f;

    [Tooltip("Atraso após a tela ficar preta antes de carregar a cena")]
    [SerializeField] private float atrasoAntesDeCarregar = 0.2f;

    [Header("Input")]
    [SerializeField] private string interactButton = "Interact";

    [Header("Indicador Visual (Opcional)")]
    [SerializeField] private GameObject pressEIndicator;

    [Header("Teleporte e Movimento")]
    [Tooltip("Arraste o GameObject do Player aqui")]
    [SerializeField] private GameObject player;

    [Tooltip("Arraste o Transform do ponto onde o player deve aparecer")]
    [SerializeField] private Transform pontoDeTeleporte;

    [Tooltip("Arraste o TopDownMovement do player aqui")]
    [SerializeField] private TopDownMovement playerMovement;

    [Tooltip("Se marcado, DESTRÓI o TopDownMovement. Se desmarcado, apenas DESATIVA.")]
    [SerializeField] private bool destruirScriptDeMovimento = true;

    private bool playerNear = false;
    private bool jaAtivou = false;
    private Image fadeImage;
    private Canvas fadeCanvas;

    void Awake()
    {
        CriarCanvasDeFade();
    }

    void Start()
    {
        if (pressEIndicator != null)
            pressEIndicator.SetActive(false);
    }

    void Update()
    {
        if (playerNear && !jaAtivou && Input.GetButtonDown(interactButton))
        {
            jaAtivou = true;

            if (pressEIndicator != null)
                pressEIndicator.SetActive(false);

            StartCoroutine(TeleportarFadeECarregar());
        }
    }

    private void CriarCanvasDeFade()
    {
        GameObject canvasGO = new GameObject("FadeCanvas_Transicao");
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

    private IEnumerator TeleportarFadeECarregar()
    {
        // --- 1. TELEPORTA O PLAYER PRIMEIRO (Antes de tudo) ---
        if (player != null && pontoDeTeleporte != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                // Força a posição no Rigidbody e no Transform
                rb.position = pontoDeTeleporte.position;
                player.transform.position = pontoDeTeleporte.position;
            }
            else
            {
                player.transform.position = pontoDeTeleporte.position;
            }
        }

        // --- 2. TRAVA O PLAYER E DESTRÓI O MOVIMENTO ---
        if (playerMovement != null)
        {
            if (destruirScriptDeMovimento)
                Destroy(playerMovement);
            else
                playerMovement.enabled = false;
        }

        // --- 3. FADE IN (ESCURECER A TELA) ---
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < tempoFade)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / tempoFade;
            color.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        // --- 4. ESPERA E CARREGA A CENA ---
        yield return new WaitForSecondsRealtime(atrasoAntesDeCarregar);

        if (!string.IsNullOrEmpty(nomeDaCena))
            SceneManager.LoadScene(nomeDaCena);
        else
            Debug.LogError("[SceneTransitionTrigger] O nome da cena não foi definido no Inspector!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = true;

            if (pressEIndicator != null && !jaAtivou)
                pressEIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = false;

            if (pressEIndicator != null)
                pressEIndicator.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);
    }
}