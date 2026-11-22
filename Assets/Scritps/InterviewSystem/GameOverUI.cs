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

    private Vector3 escalaOriginal;
    private bool escalaSalva = false;

    public void ShowDefeat(int points, int maxPoints)
    {
        // ✅ Salva o tamanho verdadeiro do painel ANTES de mexer nele
        if (!escalaSalva)
        {
            escalaOriginal = transform.localScale;
            escalaSalva = true;
        }

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

        // ✅ Começa na escala zero para dar o efeito de "pop"
        transform.localScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / animationDuration;

            // ✅ Cresce até a escala original real, não para o Vector3.one
            transform.localScale = Vector3.Lerp(Vector3.zero, escalaOriginal, progress);
            canvasGroup.alpha = progress;

            yield return null;
        }

        // ✅ Garante que no final fique exatamente no tamanho que você deixou no Editor
        transform.localScale = escalaOriginal;
        canvasGroup.alpha = 1f;
    }

    public void OnRestartClicked()
    {
        if (interviewManager != null)
            interviewManager.RestartQuiz();
    }

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