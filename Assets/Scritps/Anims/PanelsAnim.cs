using UnityEngine;
using System.Collections;

public class PanelAnim : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float slideDuration = 0.8f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Position")]
    [SerializeField] private Vector2 offScreenPosition = new Vector2(-1920, 0);
    [SerializeField] private Vector2 onScreenPosition = new Vector2(0, 0);

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private bool isCovering = false;
    private bool isAnimating = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // COMEÇA FORA DA TELA E INVISÍVEL
        rectTransform.anchoredPosition = offScreenPosition;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(true);

        Debug.Log("✅ BlackOverlay INICIADO!");
    }

    // 🔥 MÉTODO QUE O BOTÃO VAI CHAMAR
    public void CoverScreen()
    {
        if (isAnimating || isCovering) return;

        Debug.Log("⬛ Cobrindo tela com SLIDE!");
        isAnimating = true;

        rectTransform.anchoredPosition = offScreenPosition;
        canvasGroup.alpha = 1f;

        StartCoroutine(CoverCoroutine());
    }

    public void RevealScreen()
    {
        if (isAnimating || !isCovering) return;

        Debug.Log("⬜ Revelando tela!");
        isAnimating = true;

        StartCoroutine(RevealCoroutine());
    }

    public void ToggleCover()
    {
        if (isCovering)
            RevealScreen();
        else
            CoverScreen();
    }

    private IEnumerator CoverCoroutine()
    {
        float elapsed = 0f;
        Vector2 startPos = offScreenPosition;
        Vector2 endPos = onScreenPosition;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            float curveValue = slideCurve.Evaluate(t);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, curveValue);
            canvasGroup.alpha = 1f;

            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
        canvasGroup.alpha = 1f;
        isCovering = true;
        isAnimating = false;

        Debug.Log("✅ TELA PRETA COBRINDO TUDO!");
    }

    private IEnumerator RevealCoroutine()
    {
        float elapsed = 0f;
        Vector2 startPos = onScreenPosition;
        Vector2 endPos = offScreenPosition;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            float curveValue = slideCurve.Evaluate(t);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, curveValue);
            canvasGroup.alpha = 1f;

            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
        canvasGroup.alpha = 0f;
        isCovering = false;
        isAnimating = false;

        Debug.Log("✅ Tela REVELADA!");
    }
}