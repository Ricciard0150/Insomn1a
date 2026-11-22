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

    [System.Serializable]
    public class DialogueLineData
    {
        [Tooltip("Se marcado, a mão vai animar E sortear um novo Spot enquanto essa fala está na tela.")]
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

    [Header("Animação da Mão (Tutorial)")]
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

    void Start()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void StartMinigame()
    {
        if (minigamePanel != null) minigamePanel.SetActive(true);
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
                yield return StartCoroutine(TriggerGameOver("Muito lento. O patrão te demitiu."));
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

        // ✅ Força Z = 0 na posição do papel falso
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

        if (papersOrganizedThisRound >= papersToOrganizePerRound)
        {
            isRoundActive = false;
        }
        else
        {
            RandomizeSpot();
            SpawnNextPaper(false);
        }
    }

    public void OnPaperOrganizedWrongly()
    {
        if (!isRoundActive || isGameOver) return;

        currentErrors++;
        AtualizarUI();
        DestroyCurrentPaper();

        if (currentErrors >= maxErrors)
        {
            StartCoroutine(TriggerGameOver("Muitos erros. O patrão te demitiu."));
            return;
        }

        if (papersOrganizedThisRound < papersToOrganizePerRound)
        {
            SpawnNextPaper(false);
        }
    }

    private void AtualizarUI()
    {
        if (roundText != null) roundText.text = $"RODADA: {currentRound}/{totalRounds}";
        if (errorText != null) errorText.text = $"ERROS: {currentErrors}/{maxErrors}";
    }

    private void RandomizeSpot()
    {
        if (allSpots.Length == 0) return;

        foreach (var spot in allSpots)
            if (spot != null) spot.SetHighlight(false);

        int randomIndex = Random.Range(0, allSpots.Length);
        currentHighlightedSpot = allSpots[randomIndex];
        currentHighlightedSpot.SetHighlight(true);
    }

    // ✅ AQUI ESTÁ A CORREÇÃO DO Z
    private void SpawnNextPaper(bool isFirstPaperOfRound)
    {
        if (paperPrefab != null && spawnPoint != null)
        {
            // Força a posição de spawn a ter Z = 0
            Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);

            activePaper = Instantiate(paperPrefab, spawnPos, Quaternion.identity);
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

    private IEnumerator TriggerGameOver(string reason)
    {
        isGameOver = true;
        isRoundActive = false;

        DestroyCurrentPaper();

        if (minigamePanel != null) minigamePanel.SetActive(false);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator VictorySequence()
    {
        isRoundActive = false;

        if (finalVictoryLines != null && finalVictoryLines.Length > 0 && dialogueSystem != null)
        {
            dialogueSystem.StartDialogue(finalVictoryLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        if (minigamePanel != null) minigamePanel.SetActive(false);

        if (postMinigameLines != null && postMinigameLines.Length > 0 && dialogueSystem != null)
        {
            dialogueSystem.StartDialogue(postMinigameLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        if (objectsToActivate != null)
        {
            foreach (GameObject obj in objectsToActivate)
                if (obj != null) obj.SetActive(true);
        }

        if (objectsToDeactivate != null)
        {
            foreach (GameObject obj in objectsToDeactivate)
                if (obj != null) obj.SetActive(false);
        }

        if (fadeController != null)
        {
            yield return StartCoroutine(fadeController.FadeToBlack(1f));
        }
    }
}