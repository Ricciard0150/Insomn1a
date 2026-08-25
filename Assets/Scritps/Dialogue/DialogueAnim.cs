using UnityEngine;
using System.Collections;

public class DialogueAnim : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float slideOffset = 200f; 
    [SerializeField] private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform panelRect;
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
    }

    public void ShowPanel(System.Action onComplete = null)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        gameObject.SetActive(true);
        currentAnimation = StartCoroutine(AnimatePanel(hiddenPosition, shownPosition, onComplete));
    }

    public void HidePanel(System.Action onComplete = null)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(AnimatePanel(shownPosition, hiddenPosition, () => {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }));
    }

    private IEnumerator AnimatePanel(Vector2 startPos, Vector2 targetPos, System.Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = animationCurve.Evaluate(elapsed / animationDuration);
            panelRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        panelRect.anchoredPosition = targetPos;
        currentAnimation = null;
        onComplete?.Invoke();
    }

    public bool IsAnimating()
    {
        return currentAnimation != null;
    }
}