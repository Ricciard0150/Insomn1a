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

    [Header("Intro Panel")]
    [SerializeField] private IntroPanelController introPanel;

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
    private bool isCompleted = false;

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
        if (isCompleted)
        {
            Debug.Log("Esta quest já foi completada!");
            return;
        }

        if (system == null || system.IsActive)
            return;

        isFinishing = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (dialogueSystem != null && currentQuest != null && currentQuest.introLines.Length > 0)
        {
            dialogueSystem.StartDialogue(currentQuest.introLines);
            StartCoroutine(WaitForDialogueThenIntroPanel());
        }
        else
        {
            StartCoroutine(ShowIntroPanelThenQuiz());
        }
    }

    private IEnumerator WaitForDialogueThenIntroPanel()
    {
        yield return new WaitWhile(() => dialogueSystem.IsRunning);

        StartCoroutine(ShowIntroPanelThenQuiz());
    }

    private IEnumerator ShowIntroPanelThenQuiz()
    {
        if (introPanel != null)
        {
            introPanel.ShowPanel();

            yield return new WaitForSeconds(0.5f);
        }

        if (panel != null)
            panel.SetActive(true);

        yield return null;

        if (introPanel != null)
        {
            introPanel.HidePanel();
        }

        if (system != null && !system.IsActive)
        {
            system.Start();
        }
    }

    private void ShowRound(QuestionRPG question, int totalOptions)
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

    private IEnumerator ProcessChoiceWithDialogue(int choice)
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

    private void ShowFeedback(string message)
    {
        uiManager.ShowFeedback(message);
    }

    private void OnSystemFinished(bool victory, int points, int lives)
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

        isCompleted = true;

        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        if (objectToDeactivate != null)
            objectToDeactivate.SetActive(false);

        if (dialogueSystem != null)
        {
            dialogueSystem.ShowVictoryDialogue();
        }

        OnQuizFinished?.Invoke();
    }

    private IEnumerator ShowDefeatSequence(int points)
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.ShowDefeatDialogue();
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            GameOverUI panelScript = gameOverPanel.GetComponent<GameOverUI>();
            if (panelScript != null)    
            {
                panelScript.ShowDefeat(points, currentQuest.questions.Length);
            }
        }

        Time.timeScale = 0f;
        OnQuizFinished?.Invoke();
    }

    public void RestartQuiz()
    {
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

    public bool IsQuestCompleted()
    {
        return isCompleted;
    }

    public void ResetQuestState()
    {
        isCompleted = false;
        isFinishing = false;
    }
}