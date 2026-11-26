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

    [Header("Placeholder")]
    [SerializeField] private GameObject placeholderNaCena;

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

    public bool IsMinigameRunning { get; private set; } = false;

    void Start()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (placeholderNaCena != null) placeholderNaCena.SetActive(false);
    }

    public void StartMinigame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PausarMusica();

        if (minigamePanel != null) minigamePanel.SetActive(true);
        if (timerText != null) timerText.text = "";

        IsMinigameRunning = true;
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (currentRound < totalRounds && !isGameOver)
        {
            currentRound++;
            papersOrganizedThisRound = 0;
            isRoundActive = false;

            AtualizarUI();

            foreach (var spot in allSpots)
            {
                if (spot != null) spot.SetHighlight(false);
            }

            RandomizeSpot();

            DestroyCurrentPaper();
            if (placeholderNaCena != null) placeholderNaCena.SetActive(false);

            // ►►► INTRO DA RODADA: diálogo + mão em paralelo ◄◄◄
            if (dialogueSystem != null && dialoguesBetweenRounds.Length >= currentRound)
            {
                DialogueLineData[] currentRoundLines = dialoguesBetweenRounds[currentRound - 1].lines;

                foreach (DialogueLineData line in currentRoundLines)
                {
                    bool handFinished = false;
                    Coroutine handAnimRoutine = null;

                    if (line.playHandAnimation)
                    {
                        Vector3 targetPos = handTargetPoint != null
                            ? handTargetPoint.position
                            : spawnPoint.position;

                        handAnimRoutine = StartCoroutine(PlayHandTutorial(targetPos, () =>
                        {
                            handFinished = true;
                        }));
                    }

                    dialogueSystem.StartDialogue(new DialogueLine[] { line.lineData });

                    while (dialogueSystem.IsRunning || (handAnimRoutine != null && !handFinished))
                    {
                        yield return null;
                    }
                }
            }

            // ►►► INTRO TERMINOU: troca placeholder por papel interativo ◄◄◄
            if (placeholderNaCena != null) placeholderNaCena.SetActive(false);

            Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
            SpawnNextPaper(true, spawnPos);

            // ►►► COMEÇA O TIMER DA RODADA ◄◄◄
            isRoundActive = true;
            currentTime = timePerRound;

            while (isRoundActive && !isGameOver)
            {
                currentTime -= Time.deltaTime;

                if (timerText != null)
                    timerText.text = $"TEMPO: {Mathf.CeilToInt(currentTime)}s";

                if (currentTime <= 0f)
                {
                    OnPaperOrganizedWrongly();
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(0.3f);
        }

        if (!isGameOver)
        {
            yield return StartCoroutine(VictorySequence());
        }
    }

    private IEnumerator PlayHandTutorial(Vector3 targetPos, System.Action onFinished)
    {
        if (handPrefab == null)
        {
            onFinished?.Invoke();
            yield break;
        }

        Vector3 startPos = handStartPosition != null
            ? handStartPosition.position
            : spawnPoint.position;

        startPos.z = 0f;
        targetPos.z = 0f;

        GameObject handObj = Instantiate(handPrefab, startPos, Quaternion.identity, null);
        PaperHandAnimation handScript = handObj.GetComponent<PaperHandAnimation>();

        if (handScript != null)
        {
            yield return StartCoroutine(handScript.AnimateHand(
                startPos,
                targetPos,
                animationDuration,
                onPaperReleased: () =>
                {
                    if (placeholderNaCena != null)
                    {
                        Vector3 pos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
                        placeholderNaCena.transform.position = pos;
                        placeholderNaCena.SetActive(true);

                        SpriteRenderer sr = placeholderNaCena.GetComponent<SpriteRenderer>();
                        if (sr != null) sr.enabled = true;
                    }
                }
            ));
        }

        if (handObj != null) Destroy(handObj);

        onFinished?.Invoke();
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
            Vector3 pos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
            SpawnNextPaper(true, pos);
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
            isRoundActive = false;
            isGameOver = true;
            StartCoroutine(TriggerGameOver());
        }
        else
        {
            RandomizeSpot();
            Vector3 pos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
            SpawnNextPaper(true, pos);
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

    private void SpawnNextPaper(bool isInteractive, Vector3 posicao)
    {
        DestroyCurrentPaper();

        if (paperPrefab != null)
        {
            Vector3 pos = new Vector3(posicao.x, posicao.y, 0f);
            activePaper = Instantiate(paperPrefab, pos, Quaternion.identity, null);

            SpriteRenderer sr = activePaper.GetComponent<SpriteRenderer>();
            Collider2D col = activePaper.GetComponent<Collider2D>();

            if (sr != null) sr.enabled = isInteractive;
            if (col != null) col.enabled = isInteractive;
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
        IsMinigameRunning = false;

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

        IsMinigameRunning = false;
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

    public bool IsMinigameRunningCheck() => IsMinigameRunning;

    public void RestartMinigame()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        currentRound = 0;
        currentErrors = 0;
        papersOrganizedThisRound = 0;
        isGameOver = false;
        isRoundActive = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PausarMusica();

        StartMinigame();
    }
}