using System.Collections;
using UnityEngine;

public class InterviewManager : MonoBehaviour
{
    public event System.Action OnQuizFinished;

    [Header("UI")]
    public GameObject panel;
    public UInterviewManager uiManager;
    public InterviewQuests dialogueSystem;

    [Header("Quest")]
    public string questId;

    [Header("Consequences")]
    public GameObject objectToActivate;
    public GameObject objectToDeactivate;
    public GameObject player;
    public Transform respawnPoint;

    [Header("Feedback Dialogue")]
    public string[] correctDialogue;
    public string[] wrongDialogue;   

    private QuestData currentQuest;
    private InterviewSystem system;
    private bool isFinishing;
    private bool waitingForDialogue;

    void Awake()
    {
        var database = FindAnyObjectByType<QuestDatabase>();
        if (database != null)
        {
            currentQuest = database.GetQuest(questId);
        }

        if (currentQuest == null)
        {
            Debug.LogError($"Quest '{questId}' não encontrada!");
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
        if (system != null)
        {
            system.OnRoundChanged -= ShowRound;
            system.OnFinished -= OnSystemFinished;
            system.OnFeedback -= ShowFeedback;
        }
    }

    public void StartSystem()
    {
        if (system == null || system.IsActive) return;
        isFinishing = false;

        if (panel != null)
            panel.SetActive(true);

        system.Start();
    }

    void ShowRound(QuestionRPG question, int totalOptions)
    {
        uiManager.ShowQuestion(question, totalOptions, (choice) => {
            StartCoroutine(ProcessChoiceWithDialogue(choice));
        });
    }

    IEnumerator ProcessChoiceWithDialogue(int choice)
    {
        var question = currentQuest.questions[system.GetCurrentRound()];
        bool isCorrect = choice == question.correctAnswer;

        string[] dialogueLines = isCorrect ? correctDialogue : wrongDialogue;

        if (dialogueSystem != null && dialogueLines.Length > 0)
        {
            string answerText = question.options[choice];
            for (int i = 0; i < dialogueLines.Length; i++)
            {
                dialogueLines[i] = dialogueLines[i].Replace("{answer}", answerText);
            }

            dialogueSystem.StartDialogue(dialogueLines);

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
        if (isFinishing) return;
        isFinishing = true;

        if (panel != null)
            panel.SetActive(false);

        if (objectToActivate != null)
            objectToActivate.SetActive(victory);

        if (objectToDeactivate != null)
            objectToDeactivate.SetActive(!victory);

        string[] lines = victory ? currentQuest.victoryLines : currentQuest.defeatLines;
        StartCoroutine(DelayThenDialogue(lines, 1f, victory));
    }

    IEnumerator DelayThenDialogue(string[] lines, float delay, bool victory)
    {
        yield return new WaitForSeconds(delay);

        if (dialogueSystem != null && lines != null && lines.Length > 0)
        {
            dialogueSystem.StartDialogue(lines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        if (!victory && player != null && respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }

        OnQuizFinished?.Invoke();
    }
}