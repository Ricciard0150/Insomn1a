using System.Collections;
using UnityEngine;

public class IntroPanelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float waitTime = 1.5f;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Position")]
    [SerializeField] private Vector2 offScreenLeft = new Vector2(-1500, 0);
    [SerializeField] private Vector2 centerPosition = new Vector2(0, 0);

    [Header("Events")]
    [SerializeField] private UnityEngine.Events.UnityEvent onPanelShown;
    [SerializeField] private UnityEngine.Events.UnityEvent onPanelHidden;

    private bool isAnimating = false;

    void Awake()
    {
        if (panelRect == null)
            panelRect = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        panelRect.anchoredPosition = offScreenLeft;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        if (isAnimating) return;
        gameObject.SetActive(true);
        StartCoroutine(SlideIn());
    }

    public void HidePanel()
    {
        if (isAnimating) return;
        StartCoroutine(SlideOut());
    }

    private IEnumerator SlideIn()
    {
        isAnimating = true;
        float elapsed = 0f;

        panelRect.anchoredPosition = offScreenLeft;
        canvasGroup.alpha = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsed / slideDuration);

            panelRect.anchoredPosition = Vector2.Lerp(offScreenLeft, centerPosition, t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        panelRect.anchoredPosition = centerPosition;
        canvasGroup.alpha = 1f;
        isAnimating = false;

        onPanelShown?.Invoke();

        yield return new WaitForSeconds(waitTime);

        HidePanel();
    }

    private IEnumerator SlideOut()
    {
        isAnimating = true;
        float elapsed = 0f;

        Vector2 startPos = panelRect.anchoredPosition;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsed / slideDuration);

            panelRect.anchoredPosition = Vector2.Lerp(startPos, offScreenLeft, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        panelRect.anchoredPosition = offScreenLeft;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        isAnimating = false;

        onPanelHidden?.Invoke();
    }

    public void ForceHide()
    {
        StopAllCoroutines();
        panelRect.anchoredPosition = offScreenLeft;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        isAnimating = false;
    }
}