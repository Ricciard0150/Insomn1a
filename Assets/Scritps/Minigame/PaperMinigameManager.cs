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

    [Header("Diálogos e Fades")]
    [SerializeField] private InterviewQuests dialogueSystem;
    [SerializeField] private FadeController fadeController;

    [Header("Game Over UI")]
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Diálogo de Derrota")]
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
    private bool isGameOverSequenceRunning = false; // evita TriggerGameOver duplicado

    private GameObject activePaper;
    private PaperSpot currentHighlightedSpot;

    // Flag pública para o NPC evitar start duplicado
    public bool IsMinigameRunning => isRoundActive || (currentRound > 0 && !isGameOver);

    void Start()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
    }

    public void StartMinigame()
    {
        // Reset de estado caso o minigame seja reiniciado
        currentRound = 0;
        currentErrors = 0;
        papersOrganizedThisRound = 0;
        isGameOver = false;
        isGameOverSequenceRunning = false;
        isRoundActive = false;

        if (minigamePanel != null) minigamePanel.SetActive(true); // dispara OnEnable do PainelAudioController
        if (timerText != null) timerText.text = "";
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
        else { RandomizeSpot(); SpawnNextPaper(false); }
    }

    public void OnPaperOrganizedWrongly()
    {
        if (!isRoundActive || isGameOver) return;
        currentErrors++;
        AtualizarUI();
        DestroyCurrentPaper();

        if (currentErrors >= maxErrors)
        {
            StartCoroutine(TriggerGameOver());
            return;
        }

        if (papersOrganizedThisRound < papersToOrganizePerRound) SpawnNextPaper(false);
    }

    private void AtualizarUI()
    {
        if (roundText != null) roundText.text = $"RODADA: {currentRound}/{totalRounds}";
        if (errorText != null) errorText.text = $"ERROS: {currentErrors}/{maxErrors}";
    }

    private void RandomizeSpot()
    {
        if (allSpots.Length == 0) return;
        foreach (var spot in allSpots) if (spot != null) spot.SetHighlight(false);
        int randomIndex = Random.Range(0, allSpots.Length);
        currentHighlightedSpot = allSpots[randomIndex];
        currentHighlightedSpot.SetHighlight(true);
    }

    private void SpawnNextPaper(bool isFirstPaperOfRound)
    {
        if (paperPrefab != null && spawnPoint != null)
        {
            Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
            activePaper = Instantiate(paperPrefab, spawnPos, Quaternion.identity);
        }
    }

    private void DestroyCurrentPaper()
    {
        if (activePaper != null) { Destroy(activePaper); activePaper = null; }
    }

    private IEnumerator TriggerGameOver()
    {
        // Guarda contra dupla execução (timer + erros podem disparar ao mesmo tempo)
        if (isGameOverSequenceRunning) yield break;
        isGameOverSequenceRunning = true;

        isGameOver = true;
        isRoundActive = false;

        DestroyCurrentPaper();

        // 1. Fecha o painel → PainelAudioController.OnDisable dispara:
        //    para a música do minigame e retoma a música geral.
        if (minigamePanel != null) minigamePanel.SetActive(false);

        // 2. Diálogo de derrota
        if (dialogueSystem != null && defeatDialogueLines != null && defeatDialogueLines.Length > 0)
        {
            dialogueSystem.StartDialogue(defeatDialogueLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        // 3. UI de Game Over
        if (gameOverUI != null)
        {
            gameOverUI.ShowDefeat("Muito lento. O patrão te demitiu.");
        }
    }

    private IEnumerator VictorySequence()
    {
        isRoundActive = false;

        // 1. Diálogo final com o painel AINDA aberto (música do minigame ainda tocando)
        if (finalVictoryLines != null && finalVictoryLines.Length > 0 && dialogueSystem != null)
        {
            dialogueSystem.StartDialogue(finalVictoryLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        // 2. Fecha o painel → PainelAudioController.OnDisable:
        //    para a música do minigame e retoma a música geral.
        if (minigamePanel != null) minigamePanel.SetActive(false);

        // 3. Diálogo pós-minigame (painel fechado, música geral já tocando)
        if (postMinigameLines != null && postMinigameLines.Length > 0 && dialogueSystem != null)
        {
            dialogueSystem.StartDialogue(postMinigameLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        // 4. Consequências da vitória
        if (objectsToActivate != null)
            foreach (GameObject obj in objectsToActivate) if (obj != null) obj.SetActive(true);

        if (objectsToDeactivate != null)
            foreach (GameObject obj in objectsToDeactivate) if (obj != null) obj.SetActive(false);

        if (fadeController != null) yield return StartCoroutine(fadeController.FadeToBlack(1f));
    }

    public void RestartMinigame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}