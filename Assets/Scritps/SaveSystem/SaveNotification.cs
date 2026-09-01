using UnityEngine;
using TMPro;
using System.Collections;

public class SaveNotification : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float displayDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private string saveMessage = "💾 Jogo Salvo!";

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    void Start()
    {
        if (SaveNotificationManager.Instance != null)
        {
            SaveNotificationManager.Instance.RegisterNotification(this);
            Debug.Log("✅ SaveNotification registrada!");
        }
    }

    public void ShowNotification(string message = null)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (!string.IsNullOrEmpty(message))
            notificationText.text = message;
        else
            notificationText.text = saveMessage;

        gameObject.SetActive(true);
        currentCoroutine = StartCoroutine(AnimateNotification());
    }

    IEnumerator AnimateNotification()
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