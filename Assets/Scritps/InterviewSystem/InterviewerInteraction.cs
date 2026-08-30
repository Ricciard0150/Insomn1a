using UnityEngine;
using System.Collections;

public class InterviewerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pressEIndicator;
    [SerializeField] private InterviewManager interviewManager;
    [SerializeField] private InterviewQuests dialogueSystem;
    [SerializeField] private GameObject player;

    [Header("Settings")]
    [SerializeField] private bool startWithIntro = true;

    private bool playerIsNear = false;
    private bool isInteracting = false;
    private int index = 0;

    void Start()
    {
        if (pressEIndicator != null)
            pressEIndicator.SetActive(false);
    }

    void Update()
    {
        if (interviewManager != null && interviewManager.IsQuestCompleted())
        {
            if (pressEIndicator != null && pressEIndicator.activeSelf)
                pressEIndicator.SetActive(false);
            return;
        }

        if (playerIsNear && Input.GetButtonDown("Interact") && !isInteracting)
        {
            StartInteraction();
        }

        if (Input.GetButtonDown("Interact"))
        {
            print("interact");
        }
    }

    private void StartInteraction()
    {
        if (interviewManager != null && interviewManager.IsQuestCompleted())
        {
            Debug.Log("Quest já completada! Não pode interagir.");
            return;
        }

        isInteracting = true;

        if (pressEIndicator != null)
            pressEIndicator.SetActive(false);

        if (interviewManager != null)
        {
            interviewManager.StartSystem();
            return;
        }

        if (startWithIntro && dialogueSystem != null)
        {
            var quest = GetQuestFromManager();
            if (quest != null && quest.introLines.Length > 0)
            {
                dialogueSystem.StartDialogue(quest.introLines);
                StartCoroutine(WaitForDialogueThenStartQuiz());
                index = 0;
                return;
            }
        }
        StartQuiz();
    }

    private IEnumerator WaitForDialogueThenStartQuiz()
    {
        yield return new WaitWhile(() => dialogueSystem.IsRunning);

        StartQuiz();
        if (player != null)
        {
            player.SetActive(false);
            Debug.Log("player off");
        }
    }

    private void StartQuiz()
    {
        if (interviewManager != null)
        {
            interviewManager.OnQuizFinished += OnQuizFinished;
            interviewManager.StartSystem();
        }
    }

    private void OnQuizFinished()
    {
        if (player != null)
        {
            player.SetActive(true);
            Debug.Log("player on");
        }

        isInteracting = false;

        if (interviewManager != null)
            interviewManager.OnQuizFinished -= OnQuizFinished;
    }

    private QuestData GetQuestFromManager()
    {
        if (interviewManager == null) return null;

        var database = FindAnyObjectByType<QuestDatabase>();
        if (database == null) return null;

        return database.GetQuest(interviewManager.questId);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerIsNear = true;

            if (!isInteracting && pressEIndicator != null)
            {
                if (interviewManager != null && !interviewManager.IsQuestCompleted())
                {
                    pressEIndicator.SetActive(true);
                }
                else
                {
                    pressEIndicator.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerIsNear = false;
            if (pressEIndicator != null)
                pressEIndicator.SetActive(false);
        }
    }
}