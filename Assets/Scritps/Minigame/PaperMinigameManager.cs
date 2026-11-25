using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaperMinigameManager : MonoBehaviour
{
    [Header("UI do Minigame")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Diálogos e Fades")]
    [SerializeField] private InterviewQuests dialogueSystem;
    [SerializeField] private FadeController fadeController;

    [Header("Game Over UI")]
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private DialogueLine[] defeatDialogueLines;

    [System.Serializable]
    public class DialogueLineData
    {
        public bool playHandAnimation;
        public DialogueLine lineData;
    }

    [System.Serializable]
    public class RoundDialogue
    {
        public DialogueLineData[] lines;
    }

    [Header("Diálogos Entre Rodadas")]
    [SerializeField] private RoundDialogue[] dialoguesBetweenRounds;

    [Header("1. Diálogo Final (Painel AINDA Aberto)")]
    [SerializeField] private DialogueLine[] finalVictoryLines;

    [Header("2. Diálogo Pós-Minigame (Painel Fechado)")]
    [SerializeField] private DialogueLine[] postMinigameLines;

    [Header("Spots e Papéis")]
    [SerializeField] private PaperSpot[] allSpots;
    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private Transform spawnPoint;

    [Tooltip("Arraste o Placeholder aqui. O papel vai nascer do tamanho dele.")]
    [SerializeField] private GameObject placeholderReferencia;

    [Header("Animação da Mão")]
    [SerializeField] private GameObject handPrefab;
    [SerializeField] private Transform handStartPosition;
    [SerializeField] private Transform handTargetPoint;
    [SerializeField] private float animationDuration = 1.5f;

    [Header("Consequences - Victory")]
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    [Header("Regras")]
    [SerializeField] private int totalRounds = 5;
    [SerializeField] private float timePerRound = 10f;
    [SerializeField] private int maxErrors = 3;
    [SerializeField] private int papersToOrganizePerRound = 3;

    private int currentRound = 0;
    private int currentErrors = 0;
    private int papersOrganizedThisRound = 0;
    private float currentTime;
    private bool isRoundActive = false;
    private bool isGameOver = false;

    private GameObject activePaper;
    private PaperSpot currentHighlightedSpot;

    // ✅ Variável para checar se o minigame tá rolando
    public bool IsMinigameRunning { get; private set; } = false;

    void Start()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
    }

    public void StartMinigame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PausarMusica();

        if (minigamePanel != null) minigamePanel.SetActive(true);
        if (timerText != null) timerText.text = "";

        IsMinigameRunning = true; // ✅ Ativa a flag
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (currentRound < totalRounds && !isGameOver)
        {
            currentRound++;
            papersOrganizedThisRound = 0;
            isRoundActive = true;

            AtualizarUI();

            foreach (var spot in allSpots)
            {
                if (spot != null) spot.SetHighlight(false);
            }

            if (dialogueSystem != null && dialoguesBetweenRounds.Length >= currentRound)
            {
                DialogueLineData[] currentRoundLines = dialoguesBetweenRounds[currentRound - 1].lines;

                foreach (DialogueLineData line in currentRoundLines)
                {
                    if (line.playHandAnimation)
                    {
                        yield return StartCoroutine(PlayHandTutorial());
                    }

                    dialogueSystem.StartDialogue(new DialogueLine[] { line.lineData });
                    yield return new WaitWhile(() => dialogueSystem.IsRunning);
                }
            }

            if (currentHighlightedSpot == null)
            {
                RandomizeSpot();
            }

            SpawnNextPaper(true);
            currentTime = timePerRound;

            while (currentTime > 0 && papersOrganizedThisRound < papersToOrganizePerRound && !isGameOver)
            {
                if (isRoundActive)
                {
                    currentTime -= Time.deltaTime;
                    if (timerText != null) timerText.text = $"TEMPO: {currentTime:F1}s";
                }
                yield return null;
            }

            isRoundActive = false;

            if (currentTime <= 0 && papersOrganizedThisRound < papersToOrganizePerRound && !isGameOver)
            {
                yield return StartCoroutine(TriggerGameOver());
                yield break;
            }

            if (isGameOver) yield break;
        }

        if (!isGameOver) yield return StartCoroutine(VictorySequence());
    }

    private IEnumerator PlayHandTutorial()
    {
        if (handPrefab == null) yield break;
        RandomizeSpot();
        Vector3 fakePaperPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
        GameObject fakePaper = Instantiate(paperPrefab, fakePaperPos, Quaternion.identity, null);
        Collider2D fakeCol = fakePaper.GetComponent<Collider2D>();
        if (fakeCol != null) fakeCol.enabled = false;

        Vector3 startPos = handStartPosition != null ? handStartPosition.position : spawnPoint.position;
        startPos.z = 0f;
        Vector3 targetPos = handTargetPoint != null
            ? handTargetPoint.position
            : (currentHighlightedSpot != null ? currentHighlightedSpot.transform.position : spawnPoint.position);
        targetPos.z = 0f;

        GameObject handObj = Instantiate(handPrefab, startPos, Quaternion.identity, null);
        PaperHandAnimation handScript = handObj.GetComponent<PaperHandAnimation>();
        PaperVisualDemo paperScript = fakePaper.GetComponent<PaperVisualDemo>();

        if (handScript != null && paperScript != null)
        {
            Coroutine handRoutine = StartCoroutine(handScript.AnimateHand(startPos, targetPos, animationDuration));
            Coroutine paperRoutine = StartCoroutine(paperScript.AnimatePaper(startPos, targetPos, animationDuration));
            yield return handRoutine;
            yield return paperRoutine;
        }

        if (handObj != null) Destroy(handObj);
        if (fakePaper != null) Destroy(fakePaper);
    }

    public void OnPaperOrganizedCorrectly()
    {
        if (!isRoundActive || isGameOver) return;
        papersOrganizedThisRound++;
        AtualizarUI();
        DestroyCurrentPaper();

        if (papersOrganizedThisRound >= papersToOrganizePerRound) isRoundActive = false;
        else { RandomizeSpot(); SpawnNextPaper(true); }
    }

    public void OnPaperOrganizedWrongly()
    {
        if (!isRoundActive || isGameOver) return;
        currentErrors++;
        AtualizarUI();
        DestroyCurrentPaper();

        if (currentErrors >= maxErrors)
        {
            isRoundActive = false;
            isGameOver = true;
            StartCoroutine(TriggerGameOver());
        }
        else
        {
            RandomizeSpot();
            SpawnNextPaper(true);
        }
    }

    private void RandomizeSpot()
    {
        foreach (var spot in allSpots)
        {
            if (spot != null) spot.SetHighlight(false);
        }
        if (allSpots.Length > 0)
        {
            int randomIndex = Random.Range(0, allSpots.Length);
            currentHighlightedSpot = allSpots[randomIndex];
            currentHighlightedSpot.SetHighlight(true);
        }
    }

    private void SpawnNextPaper(bool isInteractive)
    {
        DestroyCurrentPaper();
        if (paperPrefab != null && spawnPoint != null)
        {
            Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
            activePaper = Instantiate(paperPrefab, spawnPos, Quaternion.identity, null);

            if (placeholderReferencia != null)
            {
                activePaper.transform.localScale = placeholderReferencia.transform.lossyScale;
            }

            Collider2D paperCollider = activePaper.GetComponent<Collider2D>();
            if (paperCollider != null) paperCollider.enabled = isInteractive;
        }
    }

    private void DestroyCurrentPaper()
    {
        if (activePaper != null)
        {
            Destroy(activePaper);
            activePaper = null;
        }
    }

    private void AtualizarUI()
    {
        if (roundText != null) roundText.text = $"RODADA: {currentRound} / {totalRounds}";
        if (errorText != null) errorText.text = $"ERROS: {currentErrors} / {maxErrors}";
    }

    private IEnumerator TriggerGameOver()
    {
        isRoundActive = false;
        IsMinigameRunning = false; // ✅ Desativa a flag

        if (fadeController != null) yield return StartCoroutine(fadeController.FadeToBlack(1f));
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.RetomarMusica();
    }

    private IEnumerator VictorySequence()
    {
        if (dialogueSystem != null && finalVictoryLines != null && finalVictoryLines.Length > 0)
        {
            dialogueSystem.StartDialogue(finalVictoryLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        ActivateVictoryObjects();

        if (minigamePanel != null) minigamePanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        if (dialogueSystem != null && postMinigameLines != null && postMinigameLines.Length > 0)
        {
            dialogueSystem.StartDialogue(postMinigameLines);
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.RetomarMusica();

        IsMinigameRunning = false; // ✅ Desativa a flag
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

    // ✅ FUNÇÕES ADICIONADAS PARA RESOLVER OS ERROS

    public bool IsMinigameRunningCheck() => IsMinigameRunning;

    public void RestartMinigame()
    {
        // Fecha o painel de Game Over se estiver aberto
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Reseta as variáveis do minigame
        currentRound = 0;
        currentErrors = 0;
        papersOrganizedThisRound = 0;
        isGameOver = false;
        isRoundActive = false;

        // Para a música se estiver tocando (opcional)
        if (AudioManager.Instance != null)
            AudioManager.Instance.PausarMusica();

        // Inicia o minigame de novo
        StartMinigame();
    }
}