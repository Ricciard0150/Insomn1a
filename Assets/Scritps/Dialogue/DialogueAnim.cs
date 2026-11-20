using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueAnim : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float slideOffset = 200f;
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade")]
    [Tooltip("Anima o alpha junto com o slide (recomendado).")]
    [SerializeField] private bool useFade = true;

    private RectTransform panelRect;
    private CanvasGroup canvasGroup;
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private Coroutine currentAnimation;

    void Awake()
    {
        panelRect = GetComponent<RectTransform>();

        if (panelRect != null)
        {
            shownPosition = panelRect.anchoredPosition;
            hiddenPosition = new Vector2(shownPosition.x, shownPosition.y - slideOffset);
            panelRect.anchoredPosition = hiddenPosition;
        }

        if (useFade)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 0f;
        }
    }

    public void ShowPanel(System.Action onComplete = null)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        gameObject.SetActive(true);

        // Se estava escondido, garante posição/alpha iniciais
        if (panelRect != null)
            panelRect.anchoredPosition = hiddenPosition;
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        currentAnimation = StartCoroutine(AnimatePanel(hiddenPosition, shownPosition, 0f, 1f, onComplete));
    }

    public void HidePanel(System.Action onComplete = null)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        float startAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;

        currentAnimation = StartCoroutine(AnimatePanel(shownPosition, hiddenPosition, startAlpha, 0f, () =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }

    private IEnumerator AnimatePanel(Vector2 startPos, Vector2 targetPos, float startAlpha, float targetAlpha, System.Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime; // funciona mesmo com timeScale = 0
            float t = animationCurve.Evaluate(Mathf.Clamp01(elapsed / animationDuration));

            if (panelRect != null)
                panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        if (panelRect != null) panelRect.anchoredPosition = targetPos;
        if (canvasGroup != null) canvasGroup.alpha = targetAlpha;

        currentAnimation = null;
        onComplete?.Invoke();
    }

    public bool IsAnimating()
    {
        return currentAnimation != null;
    }
}