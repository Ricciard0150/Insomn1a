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

    [Header("Controle do Jogador")]
    [SerializeField] private TopDownMovement playerMovement;

    [Header("Regras")]
    [SerializeField] private int totalRounds = 5;
    [SerializeField] private float timePerRound = 10f; // ✅ Tempo por rodada
    [SerializeField] private int maxErrors = 3;
    [SerializeField] private int papersToOrganizePerRound = 3;

    private int currentRound = 0;
    private int currentErrors = 0;
    private int papersOrganizedThisRound = 0;
    private float currentTime;
    private bool isRoundActive = false;
    private bool isGameOver = false;
    private bool minigameConcluido = false;

    private GameObject activePaper;
    private PaperSpot currentHighlightedSpot;

    private Coroutine currentHandCoroutine;
    private GameObject currentHandObject;

    public bool IsMinigameRunning { get; private set; } = false;

    void Start()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (placeholderNaCena != null) placeholderNaCena.SetActive(false);
    }

    public void StartMinigame()
    {
        if (minigameConcluido) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PausarMusica();

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        if (minigamePanel != null) minigamePanel.SetActive(true);

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

            // --- DIÁLOGO DA RODADA E ANIMAÇÃO DA MÃO ---
            if (dialogueSystem != null && dialoguesBetweenRounds.Length >= currentRound)
            {
                DialogueLineData[] currentRoundLines = dialoguesBetweenRounds[currentRound - 1].lines;

                foreach (DialogueLineData line in currentRoundLines)
                {
                    if (line.playHandAnimation)
                    {
                        Vector3 targetPos = handTargetPoint != null
                            ? handTargetPoint.position
                            : spawnPoint.position;

                        currentHandCoroutine = StartCoroutine(PlayHandTutorial(targetPos, null));
                    }

                    dialogueSystem.StartDialogue(new DialogueLine[] { line.lineData });
                    yield return new WaitWhile(() => dialogueSystem.IsRunning);
                }
            }

            if (currentHandObject != null)
            {
                Destroy(currentHandObject);
                currentHandObject = null;
                currentHandCoroutine = null;
            }

            if (placeholderNaCena != null) placeholderNaCena.SetActive(false);

            Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
            SpawnNextPaper(true, spawnPos);

            // ✅ Reseta o timer para essa rodada
            currentTime = timePerRound;
            isRoundActive = true;

            // --- LOOP DA RODADA ---
            while (isRoundActive && !isGameOver)
            {
                currentTime -= Time.deltaTime;

                if (timerText != null)
                    timerText.text = $"TEMPO: {Mathf.CeilToInt(currentTime)}s";

                if (currentTime <= 0f)
                {
                    currentTime = 0f;
                    // ✅ Tempo esgotado: conta como erro e interrompe o loop
                    OnPaperOrganizedWrongly();
                    break;
                }

                yield return null;
            }

            // ✅ TRAVA DE SEGURANÇA: Se o erro estourou o limite, o Game Over JÁ foi disparado
            // no OnPaperOrganizedWrongly. Garantimos que o loop pare aqui.
            if (isGameOver)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.3f);
        }

        // ✅ Se passou por todas as rodadas sem perder, é vitória
        if (!isGameOver)
        {
            minigameConcluido = true;
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

        currentHandObject = Instantiate(handPrefab, startPos, Quaternion.identity, null);
        PaperHandAnimation handScript = currentHandObject.GetComponent<PaperHandAnimation>();

        if (handScript != null)
        {
            yield return StartCoroutine(handScript.AnimateHand(
                startPos,
                targetPos,
                animationDuration,
                onPaperReleased: () =>
                {
                    if (isRoundActive || activePaper != null) return;

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

        if (currentHandObject != null) Destroy(currentHandObject);
        currentHandObject = null;
        currentHandCoroutine = null;

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

        // ✅ Se errou 3 vezes, dispara Game Over (via DefeatSequence)
        if (currentErrors >= maxErrors)
        {
            isRoundActive = false;
            isGameOver = true;
            StartCoroutine(DefeatSequence());
        }
        else
        {
            // Ainda tem vidas, apenas encerra a rodada atual
            isRoundActive = false;
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

    // ✅ SEQUÊNCIA DE DERROTA: Diálogo -> Fecha painel -> Ativa painel de derrota
    private IEnumerator DefeatSequence()
    {
        IsMinigameRunning = false;

        // 1. Para o timer imediatamente
        if (timerText != null) timerText.text = "";

        // 2. Diálogo de derrota (Painel do minigame AINDA aberto)
        if (dialogueSystem != null && defeatDialogueLines != null && defeatDialogueLines.Length > 0)
        {
            dialogueSystem.StartDialogue(defeatDialogueLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        // 3. Fecha o painel do minigame e limpa o cenário
        DestroyCurrentPaper();
        if (placeholderNaCena != null) placeholderNaCena.SetActive(false);
        if (minigamePanel != null) minigamePanel.SetActive(false);

        // 4. Fade para preto (opcional)
        if (fadeController != null) yield return StartCoroutine(fadeController.FadeToBlack(1f));

        // 5. Ativa o painel de derrota (GameOverUI)
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverUI != null) gameOverUI.gameObject.SetActive(true);

        // 6. Retoma a música
        if (AudioManager.Instance != null)
            AudioManager.Instance.RetomarMusica();

        // 7. Destrava o jogador (se você quiser que ele já possa andar ao sair do game over)
        if (playerMovement != null)
            playerMovement.SetCanMove(true);
    }

    private IEnumerator VictorySequence()
    {
        IsMinigameRunning = false;

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

        if (playerMovement != null)
            playerMovement.SetCanMove(true);
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
        if (gameOverUI != null) gameOverUI.gameObject.SetActive(false);

        currentRound = 0;
        currentErrors = 0;
        papersOrganizedThisRound = 0;
        isGameOver = false;
        isRoundActive = false;
        minigameConcluido = false;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PausarMusica();

        StartMinigame();
    }
}