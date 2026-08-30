using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterviewManager : MonoBehaviour
{
    public event System.Action OnQuizFinished;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private UInterviewManager uiManager;
    [SerializeField] private InterviewQuests dialogueSystem;

    [Header("Quest")]
    [SerializeField] public string questId;

    [Header("Consequences")]
    [SerializeField] private GameObject objectToActivate;
    [SerializeField] private GameObject objectToDeactivate;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform respawnPoint;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Quest Database")]
    [SerializeField] private QuestDatabase questDatabase;

    private QuestData currentQuest;
    private InterviewSystem system;
    private bool isFinishing;
    private bool isCompleted = false; // ← NOVO: Controla se a quest foi completada

    void Awake()
    {
        if (questDatabase != null)
        {
            currentQuest = questDatabase.GetQuest(questId);
        }

        if (currentQuest == null)
        {
            Debug.LogError($"Quest '{questId}' not found!");
            return;
        }

        system = new InterviewSystem(
            currentQuest.questions,
            currentQuest.maxLives,
            currentQuest.minCorrect
        );

        system.OnRoundChanged += ShowRound;
        system.OnFinished += OnSystemFinished;
        system.OnFeedback += ShowFeedback;
    }

    void OnDestroy()
    {
        if (system == null)
            return;

        system.OnRoundChanged -= ShowRound;
        system.OnFinished -= OnSystemFinished;
        system.OnFeedback -= ShowFeedback;
    }

    public void StartSystem()
    {
        // ← NOVO: Impede de iniciar se já foi completada
        if (isCompleted)
        {
            Debug.Log("Esta quest já foi completada! Não pode ser reiniciada.");
            return;
        }

        if (system == null || system.IsActive)
            return;

        isFinishing = false;

        if (panel != null)
            panel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        system.Start();
    }

    void ShowRound(QuestionRPG question, int totalOptions)
    {
        uiManager.ShowQuestion(
            question,
            totalOptions,
            (choice) =>
            {
                StartCoroutine(ProcessChoiceWithDialogue(choice));
            }
        );
    }

    IEnumerator ProcessChoiceWithDialogue(int choice)
    {
        QuestionRPG question = currentQuest.questions[system.GetCurrentRound()];
        DialogueLine[] dialogueLines = null;

        if (question.optionDialogues != null && choice < question.optionDialogues.Length)
        {
            dialogueLines = question.optionDialogues[choice].dialogue;
        }

        if (dialogueSystem != null && dialogueLines != null && dialogueLines.Length > 0)
        {
            DialogueLine[] lines = new DialogueLine[dialogueLines.Length];

            for (int i = 0; i < dialogueLines.Length; i++)
            {
                lines[i] = new DialogueLine();
                lines[i].characterSprite = dialogueLines[i].characterSprite;
                lines[i].text = dialogueLines[i].text.Replace("{answer}", question.options[choice]);
            }

            dialogueSystem.StartDialogue(lines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        system.ProcessChoice(choice);
    }

    void ShowFeedback(string message)
    {
        uiManager.ShowFeedback(message);
    }

    void OnSystemFinished(bool victory, int points, int lives)
    {
        if (isFinishing)
            return;

        isFinishing = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveQuestResult(questId, victory, points);
        }

        if (panel != null)
            panel.SetActive(false);

        if (!victory)
        {
            StartCoroutine(ShowDefeatSequence(points));
            return;
        }

        // ========== VITÓRIA ==========
        // ← NOVO: Marca como completada
        isCompleted = true;

        // Ativa/desativa objetos
        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        if (objectToDeactivate != null)
            objectToDeactivate.SetActive(false);

        // Mostra diálogo de vitória
        if (dialogueSystem != null)
        {
            dialogueSystem.ShowVictoryDialogue();
        }

        OnQuizFinished?.Invoke();
    }

    IEnumerator ShowDefeatSequence(int points)
    {
        Debug.Log("=== SEQUÊNCIA DE DERROTA ===");

        if (dialogueSystem != null)
        {
            Debug.Log("Mostrando diálogo de derrota...");
            dialogueSystem.ShowDefeatDialogue();
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
            Debug.Log("Diálogo de derrota terminou!");
        }

        if (gameOverPanel != null)
        {
            Debug.Log("Ativando GameOverPanel...");
            gameOverPanel.SetActive(true);

            GameOverUI panelScript = gameOverPanel.GetComponent<GameOverUI>();
            if (panelScript != null)
            {
                panelScript.ShowDefeat(points, currentQuest.questions.Length);
            }
        }
        else
        {
            Debug.LogError("GameOverPanel é NULL!");
        }

        Time.timeScale = 0f;
        OnQuizFinished?.Invoke();
    }

    public void RestartQuiz()
    {
        // ← NOVO: Impede restart se já foi completada
        if (isCompleted)
        {
            Debug.Log("Quest já completada! Não pode reiniciar.");
            return;
        }

        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        system = new InterviewSystem(
            currentQuest.questions,
            currentQuest.maxLives,
            currentQuest.minCorrect
        );

        system.OnRoundChanged += ShowRound;
        system.OnFinished += OnSystemFinished;
        system.OnFeedback += ShowFeedback;

        isFinishing = false;
        StartSystem();
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    // ← NOVO: Método para verificar se a quest já foi completada
    public bool IsQuestCompleted()
    {
        return isCompleted;
    }

    // ← NOVO: Método para resetar o estado (se necessário)
    public void ResetQuestState()
    {
        isCompleted = false;
        isFinishing = false;
    }
}