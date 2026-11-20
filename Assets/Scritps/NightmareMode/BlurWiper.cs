using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlurWiper : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private BlurController blur;
    [SerializeField] private RectTransform wipeSprite; // o pano (Image ou SpriteRenderer em Canvas)
    [SerializeField] private CanvasGroup wipeCanvasGroup; // opcional, para fade do pano

    [Header("Configuração")]
    [Tooltip("Quantas passadas do pano até limpar todo o blur.")]
    [SerializeField] private int wipeCount = 3;

    [Tooltip("Duração de UMA passada do pano.")]
    [SerializeField] private float wipeDuration = 0.45f;

    [Tooltip("Tempo mínimo que o jogador precisa segurar o espaço para uma passada valer.")]
    [SerializeField] private float minHoldTime = 0.15f;

    [Tooltip("Direção da passada. X = esquerda->direita, Y = baixo->cima.")]
    [SerializeField] private Vector2 wipeDirection = new Vector2(1f, 0f);

    [Tooltip("Quanto o pano vai além da tela (em pixels) para sair totalmente.")]
    [SerializeField] private float overshoot = 300f;

    [Header("Easing")]
    [SerializeField] private AnimationCurve wipeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Estado")]
    [SerializeField] private string holdButton = "Blink";

    private int wipesDone = 0;
    private bool wiping = false;
    private bool holding = false;
    private float holdStartTime = 0f;
    private Vector2 spriteStart;
    private Vector2 spriteEnd;
    private Action onFinished;
    private bool active = false;

    public bool IsWiping => wiping;
    public int WipesDone => wipesDone;

    void Awake()
    {
        if (wipeSprite != null)
        {
            spriteStart = wipeSprite.anchoredPosition;
        }
        if (wipeCanvasGroup != null)
            wipeCanvasGroup.alpha = 0f;

        if (wipeSprite != null)
            wipeSprite.gameObject.SetActive(false);
    }

    /// <summary>
    /// Inicia o processo. Chame quando o jumpscare pedir para o jogador limpar a tela.
    /// </summary>
    public void Begin(Action onComplete)
    {
        onFinished = onComplete;
        wipesDone = 0;
        wiping = false;
        holding = false;
        active = true;

        if (blur != null)
            blur.SetBlurIntensity(1f); // começa com blur total

        if (wipeSprite != null)
            wipeSprite.gameObject.SetActive(false);
        if (wipeCanvasGroup != null)
            wipeCanvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Força parar (ex: se o jumpscare for cancelado).
    /// </summary>
    public void Cancel()
    {
        StopAllCoroutines();
        active = false;
        wiping = false;
        holding = false;
        if (wipeSprite != null) wipeSprite.gameObject.SetActive(false);
        if (wipeCanvasGroup != null) wipeCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (!active || wiping) return;

        // Segurou o botão
        if (Input.GetButtonDown(holdButton))
        {
            holding = true;
            holdStartTime = Time.time;
        }

        // Soltou
        if (Input.GetButtonUp(holdButton))
        {
            holding = false;
        }

        // Se está segurando há tempo suficiente, dispara uma passada
        if (holding && Time.time - holdStartTime >= minHoldTime && wipesDone < wipeCount)
        {
            holding = false;
            StartCoroutine(DoWipe());
        }
    }

    private IEnumerator DoWipe()
    {
        wiping = true;

        // Ativa o pano
        if (wipeSprite != null)
        {
            wipeSprite.gameObject.SetActive(true);
            wipeSprite.anchoredPosition = GetOffscreenPosition(-1f); // começa fora da tela
        }

        // Fade in rápido
        if (wipeCanvasGroup != null)
        {
            float t = 0f;
            while (t < 0.08f)
            {
                t += Time.deltaTime;
                wipeCanvasGroup.alpha = Mathf.Clamp01(t / 0.08f);
                yield return null;
            }
            wipeCanvasGroup.alpha = 1f;
        }

        // Move o pano
        float elapsed = 0f;
        Vector2 startPos = GetOffscreenPosition(-1f);
        Vector2 endPos = GetOffscreenPosition(1f);

        while (elapsed < wipeDuration)
        {
            elapsed += Time.deltaTime;
            float p = wipeCurve.Evaluate(Mathf.Clamp01(elapsed / wipeDuration));
            if (wipeSprite != null)
                wipeSprite.anchoredPosition = Vector2.Lerp(startPos, endPos, p);
            yield return null;
        }

        if (wipeSprite != null)
            wipeSprite.anchoredPosition = endPos;

        // Fade out rápido
        if (wipeCanvasGroup != null)
        {
            float t = 0f;
            while (t < 0.08f)
            {
                t += Time.deltaTime;
                wipeCanvasGroup.alpha = 1f - Mathf.Clamp01(t / 0.08f);
                yield return null;
            }
            wipeCanvasGroup.alpha = 0f;
        }

        if (wipeSprite != null)
            wipeSprite.gameObject.SetActive(false);

        // Aumenta o progresso
        wipesDone++;

        // Reduz o blur proporcionalmente
        if (blur != null)
        {
            float remaining = 1f - (float)wipesDone / wipeCount;
            blur.SetBlurIntensity(remaining);
        }

        wiping = false;

        // Terminou?
        if (wipesDone >= wipeCount)
        {
            active = false;
            if (blur != null) blur.SetBlurIntensity(0f);
            onFinished?.Invoke();
        }
    }

    /// <summary>
    /// Retorna a posição "fora da tela" para começar/terminar a passada.
    /// factor = -1 (antes de entrar), +1 (depois de sair).
    /// </summary>
    private Vector2 GetOffscreenPosition(float factor)
    {
        if (wipeSprite == null) return Vector2.zero;

        RectTransform parentRect = wipeSprite.parent as RectTransform;
        float halfWidth = parentRect != null ? parentRect.rect.width * 0.5f : 960f;
        float halfHeight = parentRect != null ? parentRect.rect.height * 0.5f : 540f;

        // Direção normalizada
        Vector2 dir = wipeDirection.normalized;
        if (dir == Vector2.zero) dir = Vector2.right;

        // Quantidade em cada eixo para sair da tela + overshoot
        float offsetX = (halfWidth + overshoot) * Mathf.Sign(dir.x == 0 ? 1 : dir.x);
        float offsetY = (halfHeight + overshoot) * Mathf.Sign(dir.y == 0 ? 1 : dir.y);

        // Se a direção tem componente X dominante, usa X, senão Y
        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return new Vector2(offsetX * factor, spriteStart.y);
        else
            return new Vector2(spriteStart.x, offsetY * factor);
    }
}