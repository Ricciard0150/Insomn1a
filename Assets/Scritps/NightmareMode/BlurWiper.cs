using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlurWiper : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private BlurController blur;
    [SerializeField] private RectTransform wipeSprite;
    [SerializeField] private CanvasGroup wipeCanvasGroup;

    [Header("Configuração")]
    [Tooltip("Quantas esfregadas (passadas) até limpar todo o blur.")]
    [SerializeField] private int wipeCount = 3;

    [Tooltip("Duração de UMA esfregada completa (círculo inteiro).")]
    [SerializeField] private float wipeDuration = 0.35f;

    [Tooltip("Raio do círculo que o pano vai fazer na tela.")]
    [SerializeField] private float circleRadius = 250f;

    [Header("Easing")]
    [SerializeField] private AnimationCurve wipeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Input")]
    [SerializeField] private string advanceButton = "Blink";

    private int wipesDone = 0;
    private bool wiping = false;
    private Action onFinished;
    private bool active = false;
    private Vector2 initialCenter;

    public bool IsWiping => wiping;
    public int WipesDone => wipesDone;

    void Awake()
    {
        if (wipeSprite != null)
        {
            // Salva a posição central onde o pano deve fazer o círculo
            initialCenter = wipeSprite.anchoredPosition;
        }

        if (wipeCanvasGroup != null)
            wipeCanvasGroup.alpha = 0f;

        if (wipeSprite != null)
            wipeSprite.gameObject.SetActive(false);
    }

    public void Begin(Action onComplete)
    {
        onFinished = onComplete;
        wipesDone = 0;
        wiping = false;
        active = true;

        if (blur != null)
            blur.SetBlurIntensity(1f); // começa com blur total

        if (wipeSprite != null)
            wipeSprite.gameObject.SetActive(false);
        if (wipeCanvasGroup != null)
            wipeCanvasGroup.alpha = 0f;
    }

    public void Cancel()
    {
        StopAllCoroutines();
        active = false;
        wiping = false;
        if (wipeSprite != null) wipeSprite.gameObject.SetActive(false);
        if (wipeCanvasGroup != null) wipeCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (!active || wiping) return;

        // Detecta o clique do ESPAÇO (não precisa segurar)
        if (Input.GetButtonDown(advanceButton))
        {
            StartCoroutine(DoWipe());
        }
    }

    private IEnumerator DoWipe()
    {
        wiping = true;

        if (wipeSprite != null)
        {
            wipeSprite.gameObject.SetActive(true);
            // O pano começa no centro, mas um pouco afastado para o fade in
            wipeSprite.anchoredPosition = initialCenter + new Vector2(circleRadius, 0);
        }

        // Fade in suave do pano
        if (wipeCanvasGroup != null)
        {
            float t = 0f;
            while (t < 0.1f)
            {
                t += Time.deltaTime;
                wipeCanvasGroup.alpha = Mathf.Clamp01(t / 0.1f);
                yield return null;
            }
            wipeCanvasGroup.alpha = 1f;
        }

        // Movimento Circular (Desenha o círculo)
        float elapsed = 0f;
        while (elapsed < wipeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / wipeDuration);

            // O ângulo vai de 0 a 360 (uma volta completa)
            float angle = progress * Mathf.PI * 2f;

            // Calcula a posição do círculo usando Seno e Cosseno
            // Multiplicamos por 1.5f para dar uma "volta e meia" (mais natural que só 1 volta)
            float currentAngle = angle * 1.5f;

            float x = Mathf.Cos(currentAngle) * circleRadius;
            float y = Mathf.Sin(currentAngle) * circleRadius;

            if (wipeSprite != null)
                wipeSprite.anchoredPosition = initialCenter + new Vector2(x, y);

            yield return null;
        }

        // Fade out rápido do pano
        if (wipeCanvasGroup != null)
        {
            float t = 0f;
            while (t < 0.1f)
            {
                t += Time.deltaTime;
                wipeCanvasGroup.alpha = 1f - Mathf.Clamp01(t / 0.1f);
                yield return null;
            }
            wipeCanvasGroup.alpha = 0f;
        }

        if (wipeSprite != null)
            wipeSprite.gameObject.SetActive(false);

        // Contabiliza o progresso
        wipesDone++;

        // Reduz o blur gradativamente
        if (blur != null)
        {
            float remaining = 1f - (float)wipesDone / wipeCount;
            blur.SetBlurIntensity(remaining);
        }

        wiping = false;

        // Se limpou tudo, finaliza
        if (wipesDone >= wipeCount)
        {
            active = false;
            if (blur != null) blur.SetBlurIntensity(0f);
            onFinished?.Invoke();
        }
    }
}