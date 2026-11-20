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

    // ✅ NOVO: Arraste o botão blink aqui
    [SerializeField] private GameObject spaceKeyIndicator;

    [Header("Configuração")]
    [SerializeField] private int wipeCount = 3;
    [SerializeField] private float wipeDuration = 0.35f;
    [SerializeField] private float circleRadius = 250f;
    [SerializeField] private AnimationCurve wipeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
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
        if (wipeSprite != null) initialCenter = wipeSprite.anchoredPosition;
        if (wipeCanvasGroup != null) wipeCanvasGroup.alpha = 0f;
        if (wipeSprite != null) wipeSprite.gameObject.SetActive(false);
    }

    public void Begin(Action onComplete)
    {
        onFinished = onComplete;
        wipesDone = 0;
        wiping = false;
        active = true;

        if (blur != null) blur.SetBlurIntensity(1f);
        if (wipeSprite != null) wipeSprite.gameObject.SetActive(false);
        if (wipeCanvasGroup != null) wipeCanvasGroup.alpha = 0f;
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

        if (Input.GetButtonDown(advanceButton))
        {
            // ✅ NOVO: Esconde o botão blink no primeiro clique
            if (wipesDone == 0 && spaceKeyIndicator != null)
            {
                spaceKeyIndicator.SetActive(false);
            }

            StartCoroutine(DoWipe());
        }
    }

    private IEnumerator DoWipe()
    {
        wiping = true;

        if (wipeSprite != null)
        {
            wipeSprite.gameObject.SetActive(true);
            wipeSprite.anchoredPosition = initialCenter + new Vector2(circleRadius, 0);
        }

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

        float elapsed = 0f;
        while (elapsed < wipeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / wipeDuration);
            float angle = progress * Mathf.PI * 2f;
            float currentAngle = angle * 1.5f;

            float x = Mathf.Cos(currentAngle) * circleRadius;
            float y = Mathf.Sin(currentAngle) * circleRadius;

            if (wipeSprite != null)
                wipeSprite.anchoredPosition = initialCenter + new Vector2(x, y);

            yield return null;
        }

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

        if (wipeSprite != null) wipeSprite.gameObject.SetActive(false);

        wipesDone++;

        if (blur != null)
        {
            float remaining = 1f - (float)wipesDone / wipeCount;
            blur.SetBlurIntensity(remaining);
        }

        wiping = false;

        if (wipesDone >= wipeCount)
        {
            active = false;
            if (blur != null) blur.SetBlurIntensity(0f);
            onFinished?.Invoke();
        }
    }
}