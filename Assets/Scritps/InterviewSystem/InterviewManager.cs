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

    private QuestData currentQuest;
    private InterviewSystem system;
    private bool isFinishing;

    void Awake()
    {
        QuestDatabase database = FindAnyObjectByType<QuestDatabase>();

        if (database != null)
        {
            currentQuest = database.GetQuest(questId);
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
        if (system == null || system.IsActive)
            return;

        isFinishing = false;

        if (panel != null)
            panel.SetActive(true);

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
        bool isCorrect = choice == question.correctAnswer;
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
            DialogueLine[] dialogue = new DialogueLine[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                dialogue[i] = new DialogueLine();
                dialogue[i].text = lines[i];
                dialogue[i].characterSprite = null;
            }

            dialogueSystem.StartDialogue(dialogue);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        if (!victory && player != null && respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }

        OnQuizFinished?.Invoke();
    }

    public bool IsQuestCompleted()
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.IsQuestCompleted(questId);
        return false;
    }

    public bool GetQuestResult()
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.GetQuestResult(questId);
        return false;
    }
}