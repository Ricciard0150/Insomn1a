using UnityEngine;

/// <summary>
/// Trigger automático de entrevista. NÃO substitui nem altera o InterviewerInteraction.
/// Coloque este componente em um GameObject separado (com seu próprio Collider2D trigger).
/// </summary>
public class AutoInterviewTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InterviewManager interviewManager;
    [SerializeField] private GameObject player;

    [Header("Settings")]
    [SerializeField] private bool hidePlayerDuringQuiz = true;
    [SerializeField] private bool triggerOnlyOnce = true;
    [SerializeField] private float delayBeforeStart = 0.2f;

    private bool alreadyTriggered = false;
    private bool quizRunning = false;

    void Awake()
    {
        if (interviewManager == null)
            interviewManager = FindAnyObjectByType<InterviewManager>();

        if (interviewManager != null)
            interviewManager.OnQuizFinished += HandleQuizFinished;
    }

    void OnDestroy()
    {
        if (interviewManager != null)
            interviewManager.OnQuizFinished -= HandleQuizFinished;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IStatusPlayer _))
            return;

        if (quizRunning) return;
        if (triggerOnlyOnce && alreadyTriggered) return;
        if (interviewManager == null) return;
        if (interviewManager.IsQuestCompleted()) return;

        alreadyTriggered = true;
        quizRunning = true;

        if (hidePlayerDuringQuiz && player != null)
            player.SetActive(false);

        Invoke(nameof(StartInterview), delayBeforeStart);
    }

    private void StartInterview()
    {
        if (interviewManager == null) return;

        if (!interviewManager.IsQuestCompleted())
            interviewManager.StartSystem();
    }

    private void HandleQuizFinished()
    {
        quizRunning = false;

        if (hidePlayerDuringQuiz && player != null)
            player.SetActive(true);
    }
}