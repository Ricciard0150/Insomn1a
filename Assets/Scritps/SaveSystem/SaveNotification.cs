using UnityEngine;
using TMPro;
using System.Collections;

public class SaveNotification : MonoBehaviour
{
    public TextMeshProUGUI notificationText;
    public float fadeInDuration = 0.5f;
    public float displayDuration = 1.5f;
    public float fadeOutDuration = 0.5f;

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
    }

    public void Show(string message)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        notificationText.text = message;
        gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayDuration);

        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        gameObject.SetActive(false);
        currentCoroutine = null;
    }
}