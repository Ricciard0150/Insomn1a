using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Defeat Settings")]
    [SerializeField] private string defeatTitle;
    [TextArea(2, 4)]
    [SerializeField] private string defeatMessage;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;

    [Header("References")]
    [SerializeField] private InterviewManager interviewManager;

    // ❌ REMOVIDO: Toda a lógica do Awake() e OnDestroy() que adicionava os listeners.

    public void ShowDefeat(int points, int maxPoints)
    {
        gameObject.SetActive(true);

        if (titleText != null)
            titleText.text = defeatTitle;

        if (messageText != null)
            messageText.text = defeatMessage;

        if (scoreText != null)
            scoreText.text = $"Pontuação: {points}/{maxPoints}";

        StartCoroutine(AnimatePanel());
    }

    private IEnumerator AnimatePanel()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / animationDuration;

            transform.localScale = Vector3.one * progress;
            canvasGroup.alpha = progress;

            yield return null;
        }

        transform.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
    }

    // ✅ AGORA É PÚBLICO: Vai aparecer na lista do OnClick() do botão
    public void OnRestartClicked()
    {
        if (interviewManager != null)
            interviewManager.RestartQuiz();
    }

    // ✅ AGORA É PÚBLICO: Vai aparecer na lista do OnClick() do botão
    public void OnMenuClicked()
    {
        if (interviewManager != null)
            interviewManager.GoToMenu();
    }

    public void SetInterviewManager(InterviewManager manager)
    {
        interviewManager = manager;
    }
}