using UnityEngine;
using System.Collections;

public class InterviewerInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject pressEIndicator;
    public InterviewManager interviewManager;
    public InterviewQuests dialogueSystem;
    public GameObject player; 

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public bool startWithIntro = true;

    private bool playerIsNear = false;
    private bool isInteracting = false;

    void Start()
    {
        if (pressEIndicator != null)
            pressEIndicator.SetActive(false);
    }

    void Update()
    {
        if (playerIsNear && Input.GetKeyDown(interactKey) && !isInteracting)
        {
            StartInteraction();
        }
    }

    void StartInteraction()
    {
        isInteracting = true;

        if (pressEIndicator != null)
            pressEIndicator.SetActive(false);

        if (startWithIntro && dialogueSystem != null)
        {
            var quest = GetQuestFromManager();
            if (quest != null && quest.introLines.Length > 0)
            {
                dialogueSystem.StartDialogue(quest.introLines);
                StartCoroutine(WaitForDialogueThenStartQuiz());
                return;
            }
        }
        StartQuiz();
    }

    IEnumerator WaitForDialogueThenStartQuiz()
    {
        yield return new WaitWhile(() => dialogueSystem.IsRunning);

        StartQuiz();
        if (player != null)
        {
            player.SetActive(false);
            Debug.Log("player off");
        }
    }

    void StartQuiz()
    {

        if (interviewManager != null)
        {
            interviewManager.OnQuizFinished += OnQuizFinished;
            interviewManager.StartSystem();
        }
    }

    void OnQuizFinished()
    {
        if (player != null)
        {
            player.SetActive(true);
            Debug.Log("player on ");
        }

        isInteracting = false;

        if (interviewManager != null)
            interviewManager.OnQuizFinished -= OnQuizFinished;
    }

    QuestData GetQuestFromManager()
    {
        if (interviewManager == null) return null;

        var database = FindAnyObjectByType<QuestDatabase>();
        if (database == null) return null;

        return database.GetQuest(interviewManager.questId);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerIsNear = true;
            if (!isInteracting && pressEIndicator != null)
                pressEIndicator.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerIsNear = false;
            if (pressEIndicator != null)
                pressEIndicator.SetActive(false);
        }
    }
}