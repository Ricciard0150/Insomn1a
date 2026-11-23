using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterviewManager : MonoBehaviour
{
    public event System.Action OnQuizFinished;

    [Header("Configurações da Quest")]
    [Tooltip("Se marcado, ao perder a entrevista, NÃO mostra o painel de Game Over (só toca o diálogo de derrota).")]
    [SerializeField] private bool derrotaSemGameOverPanel = false;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private UInterviewManager uiManager;
    [SerializeField] private InterviewQuests dialogueSystem;

    [Header("Sprites do Personagem (Quiz)")]
    [SerializeField] private UnityEngine.UI.Image characterImage;
    [SerializeField] private Sprite spritePergunta;
    [SerializeField] private Sprite spriteAcerto;
    [SerializeField] private Sprite spriteErro;

    [Header("Intro Panel")]
    [SerializeField] private IntroPanelController introPanel;

    [Header("Quest")]
    [SerializeField] public string questId;

    [Header("Consequences - Victory")]
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    [Header("Consequences - Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform respawnPoint;

    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameOverUI gameOverUIComponent;
    [SerializeField] private string menuSceneName = "MainMenu";

    [Header("Quest Database")]
    [SerializeField] private QuestDatabase questDatabase;

    // ✅ Áudio da Entrevista (RESTAURADO)
    [Header("Audio")]
    [Tooltip("Arraste um AudioSource aqui para tocar a música da entrevista")]
    [SerializeField] private AudioSource bgmAudioSource;
    [Tooltip("A música que toca enquanto a entrevista está rolando")]
    [SerializeField] private AudioClip musicaEntrevista;

    private QuestData currentQuest;
    private InterviewSystem system;
    private bool isFinishing;
    private bool isCompleted = false;

    void Start()
    {
        InitializeQuest();
    }

    private void InitializeQuest()
    {
        if (questDatabase == null)
            questDatabase = FindAnyObjectByType<QuestDatabase>();

        if (questDatabase == null || string.IsNullOrEmpty(questId))
        {
            Debug.LogError($"[InterviewManager] Erro ao inicializar quest '{questId}'. Banco ou ID nulo.");
            return;
        }

        currentQuest = questDatabase.GetQuest(questId);

        if (currentQuest == null)
        {
            Debug.LogError($"[InterviewManager] Quest '{questId}' não encontrada!");
            return;
        }

        if (SaveSystem.Instance != null && SaveSystem.Instance.IsQuestCompleted(questId))
        {
            isCompleted = true;
            ActivateVictoryObjects();
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
        if (system == null) return;

        system.OnRoundChanged -= ShowRound;
        system.OnFinished -= OnSystemFinished;
        system.OnFeedback -= ShowFeedback;
    }

    public void StartSystem()
    {
        if (isCompleted) return;
        if (system == null) return;
        if (system.IsActive) return;

        isFinishing = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // ✅ Pausa o AudioManager geral
        if (AudioManager.Instance != null)
            AudioManager.Instance.PausarMusica();

        if (dialogueSystem != null && currentQuest != null && currentQuest.introLines.Length > 0)
        {
            dialogueSystem.StartDialogue(currentQuest.introLines, currentQuest.introNames);
            StartCoroutine(WaitForDialogueThenIntroPanel());
        }
        else
        {
            StartCoroutine(ShowIntroPanelThenQuiz());
        }
    }

    // ✅ RESTAURADO
    private void PlayInterviewMusic()
    {
        if (bgmAudioSource != null && musicaEntrevista != null)
        {
            bgmAudioSource.clip = musicaEntrevista;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
            Debug.Log("[InterviewManager] Música da entrevista iniciada.");
        }
    }

    // ✅ RESTAURADO
    private void StopInterviewMusic()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            Debug.Log("[InterviewManager] Música da entrevista parada.");
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
        {
            panel.SetActive(true);

            // ✅ Toca a música específica da entrevista
            PlayInterviewMusic();
        }

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
        if (characterImage != null && spritePergunta != null)
            characterImage.sprite = spritePergunta;

        uiManager.ShowQuestion(
            question,
            totalOptions,
            (choice) =>
            {
                StartCoroutine(ProcessChoiceWithDialogue(choice, question));
            }
        );
    }

    private IEnumerator ProcessChoiceWithDialogue(int choice, QuestionRPG currentQuestionObj)
    {
        QuestionRPG question = currentQuest.questions[system.GetCurrentRound()];
        DialogueLine[] dialogueLines = null;
        bool isCorrect = (choice == question.correctAnswer);

        if (characterImage != null)
        {
            if (isCorrect && spriteAcerto != null)
                characterImage.sprite = spriteAcerto;
            else if (!isCorrect && spriteErro != null)
                characterImage.sprite = spriteErro;
        }

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
                lines[i].characterName = !string.IsNullOrEmpty(dialogueLines[i].characterName)
                    ? dialogueLines[i].characterName
                    : currentQuest.npcName;
                lines[i].text = dialogueLines[i].text.Replace("{answer}", question.options[choice]);
            }

            dialogueSystem.StartDialogue(lines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        system.ProcessChoice(choice);
    }

    private void ShowFeedback(string message)
    {
        if (uiManager != null)
            uiManager.ShowFeedback(message);
    }

    private void OnSystemFinished(bool victory, int points, int lives)
    {
        if (isFinishing) return;
        isFinishing = true;

        // ✅ Para a música específica da entrevista
        StopInterviewMusic();

        // ✅ Retoma a música de fundo geral
        if (AudioManager.Instance != null)
            AudioManager.Instance.RetomarMusica();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveQuestResult(questId, victory, points);
        }

        if (panel != null)
            panel.SetActive(false);

        if (!victory)
        {
            if (derrotaSemGameOverPanel)
            {
                if (dialogueSystem != null)
                {
                    dialogueSystem.ShowDefeatDialogue();
                }
                StartCoroutine(AvisarFimSemGameOver());
            }
            else
            {
                StartCoroutine(ShowDefeatSequence(points));
            }
            return;
        }

        isCompleted = true;

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.CompleteQuest(questId);
        }

        ActivateVictoryObjects();

        if (dialogueSystem != null)
        {
            dialogueSystem.ShowVictoryDialogue();
        }

        OnQuizFinished?.Invoke();
    }

    private IEnumerator AvisarFimSemGameOver()
    {
        if (dialogueSystem != null)
            yield return new WaitWhile(() => dialogueSystem.IsRunning);

        OnQuizFinished?.Invoke();
    }

    private void ActivateVictoryObjects()
    {
        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null) obj.SetActive(true);
            }
        }

        if (objectsToDeactivate != null)
        {
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
    }

    private IEnumerator ShowDefeatSequence(int points)
    {
        yield return new WaitForSeconds(1f);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (gameOverUIComponent == null)
                gameOverUIComponent = gameOverPanel.GetComponent<GameOverUI>();

            if (gameOverUIComponent == null)
                gameOverUIComponent = gameOverPanel.GetComponentInChildren<GameOverUI>(true);

            if (gameOverUIComponent != null)
            {
                int maxPoints = currentQuest != null ? currentQuest.questions.Length : 0;
                gameOverUIComponent.ShowDefeat(points, maxPoints);
            }
        }
    }

    public bool IsQuestCompleted() => isCompleted;

    public void RestartQuiz()
    {
        if (SceneManager.GetActiveScene().name != "")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}