using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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

    [Header("References (Opcionais)")]
    [Tooltip("Arraste o InterviewManager se for usar o Game Over na entrevista.")]
    [SerializeField] private InterviewManager interviewManager;

    [Tooltip("Arraste o PaperMinigameManager se for usar o Game Over no minigame.")]
    [SerializeField] private PaperMinigameManager paperMinigameManager;

    private Vector3 escalaOriginal;
    private bool escalaSalva = false;

    // ✅ Sobrecarga 1: Usada pela Entrevista (com pontuação)
    public void ShowDefeat(int points, int maxPoints)
    {
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
        {
            scoreText.gameObject.SetActive(true);
            scoreText.text = $"Pontuação: {points}/{maxPoints}";
        }

        StartCoroutine(AnimatePanel());
    }

    // ✅ Sobrecarga 2: Usada pelo Minigame (com mensagem customizada)
    public void ShowDefeat(string customMessage)
    {
        if (!escalaSalva)
        {
            escalaOriginal = transform.localScale;
            escalaSalva = true;
        }

        gameObject.SetActive(true);

        if (titleText != null)
            titleText.text = defeatTitle;

        if (messageText != null)
            messageText.text = customMessage;

        if (scoreText != null)
            scoreText.gameObject.SetActive(false); // Esconde a pontuação no minigame

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

            transform.localScale = Vector3.Lerp(Vector3.zero, escalaOriginal, progress);
            canvasGroup.alpha = progress;

            yield return null;
        }

        transform.localScale = escalaOriginal;
        canvasGroup.alpha = 1f;
    }

    // ✅ Botão Reiniciar
    public void OnRestartClicked()
    {
        if (interviewManager != null)
        {
            interviewManager.RestartQuiz();
            return;
        }

        if (paperMinigameManager != null)
        {
            paperMinigameManager.RestartMinigame();
            return;
        }

        // Fallback: Se não tiver nenhum dos dois, recarrega a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ✅ Botão Menu
    public void OnMenuClicked()
    {
        if (interviewManager != null)
        {
            interviewManager.GoToMenu();
            return;
        }

        // Fallback: Se for do minigame (ou qualquer outro), carrega o Menu Principal
        SceneManager.LoadScene("MainMenu");
    }

    public void SetInterviewManager(InterviewManager manager)
    {
        interviewManager = manager;
    }
}