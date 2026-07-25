using UnityEngine;

public class InterviewerInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject pressEIndicator;
    public InterviewManager interviewManager;
    public InterviewQuests dialogueSystem;
    public GameObject player; // ← AGORA É GameObject, não TopDownMovement

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

        // Se tem intro, mostra diálogo primeiro
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

        // Se não tem intro, começa direto
        StartQuiz();
    }

    System.Collections.IEnumerator WaitForDialogueThenStartQuiz()
    {
        // Espera o diálogo terminar
        yield return new WaitWhile(() => dialogueSystem.IsRunning);

        // 🔥 COMEÇA O QUIZ E DESATIVA O PLAYER
        StartQuiz();
        if (player != null)
        {
            player.SetActive(false);
            Debug.Log("🚫 Player desativado!");
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
        // 🔓 ATIVA O PLAYER DE VOLTA
        if (player != null)
        {
            player.SetActive(true);
            Debug.Log("✅ Player ativado!");
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