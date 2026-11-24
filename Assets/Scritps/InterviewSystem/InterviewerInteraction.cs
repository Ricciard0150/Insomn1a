using System.Collections;
using UnityEngine;

public class InterviewerInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pressEIndicator;
    [SerializeField] private InterviewManager interviewManager;
    [SerializeField] private InterviewQuests dialogueSystem;

    [Header("Settings")]
    [SerializeField] private bool startWithIntro = true;

    private bool playerIsNear = false;
    private bool isInteracting = false;

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
    }

    private void StartInteraction()
    {
        if (interviewManager != null && interviewManager.IsQuestCompleted()) return;

        isInteracting = true;

        if (pressEIndicator != null) pressEIndicator.SetActive(false);

        // O próprio InterviewManager já lida com a introDialogue.
        if (interviewManager != null)
        {
            interviewManager.StartSystem();
            interviewManager.OnQuizFinished += OnQuizFinished;
            return;
        }
    }

    private void OnQuizFinished()
    {
        isInteracting = false;
        if (interviewManager != null)
            interviewManager.OnQuizFinished -= OnQuizFinished;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = true;
            if (!isInteracting && pressEIndicator != null && interviewManager != null && !interviewManager.IsQuestCompleted())
                pressEIndicator.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = false;
            if (pressEIndicator != null) pressEIndicator.SetActive(false);
        }
    }
}