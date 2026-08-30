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

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Defeat Settings")]
    [SerializeField] private string defeatTitle = "❌ VOCÊ PERDEU!";
    [TextArea(2, 4)]
    [SerializeField] private string defeatMessage = "Suas respostas não foram suficientes.";

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.5f;

    [Header("References")]
    [SerializeField] private InterviewManager interviewManager;

    void Start()
    {
        gameObject.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);
    }

    void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClicked);

        if (menuButton != null)
            menuButton.onClick.RemoveListener(OnMenuClicked);
    }

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

    private void OnRestartClicked()
    {
        if (interviewManager != null)
            interviewManager.RestartQuiz();
    }

    private void OnMenuClicked()
    {
        if (interviewManager != null)
            interviewManager.GoToMenu();
    }

    public void SetInterviewManager(InterviewManager manager)
    {
        interviewManager = manager;
    }
}