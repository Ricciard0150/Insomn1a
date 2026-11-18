using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] private string nextSceneName;

    [System.Serializable]
    public class RoundDialogue
    {
        public string[] lines;
    }

    [Header("Diálogos Entre Rodadas")]
    [SerializeField] private RoundDialogue[] dialoguesBetweenRounds;

    [Header("Spots e Papéis")]
    [SerializeField] private PaperSpot[] allSpots;
    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Animação da Mão (Tutorial)")]
    [SerializeField] private GameObject handPrefab;
    [Tooltip("Onde a mão começa. Se vazio, usa o SpawnPoint.")]
    [SerializeField] private Transform handStartPosition; // ✅ MUDANÇA AQUI
    [Tooltip("Até onde a mão vai. Se vazio, usa o Spot sorteado.")]
    [SerializeField] private Transform handTargetPoint;
    [SerializeField] private bool showHandTutorial = true;
    [SerializeField] private float animationDuration = 1.5f;

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

            // 1. Spawn do papel e escolha do spot
            SpawnNextPaper(false);

            // 2. Animação da mão tutorial
            if (showHandTutorial && handPrefab != null && activePaper != null)
            {
                // Define as posições
                Vector3 startPos = handStartPosition != null ? handStartPosition.position : spawnPoint.position;
                Vector3 targetPos = handTargetPoint != null
                    ? handTargetPoint.position
                    : (currentHighlightedSpot != null ? currentHighlightedSpot.transform.position : spawnPoint.position);

                GameObject handObj = Instantiate(handPrefab, startPos, Quaternion.identity); // ✅ Nasce no startPos
                PaperHandAnimation handScript = handObj.GetComponent<PaperHandAnimation>();
                PaperVisualDemo paperScript = activePaper.GetComponent<PaperVisualDemo>();

                if (handScript != null && paperScript != null)
                {
                    // Move a mão e o papel juntos
                    Coroutine handRoutine = StartCoroutine(handScript.AnimateHand(startPos, targetPos, animationDuration));
                    Coroutine paperRoutine = StartCoroutine(paperScript.AnimatePaper(startPos, targetPos, animationDuration));

                    yield return handRoutine;
                    yield return paperRoutine;
                }

                if (handObj != null) Destroy(handObj);
                DestroyCurrentPaper();

                SpawnNextPaper(true);
            }

            // 3. Diálogo antes da rodada começar de fato
            if (dialogueSystem != null && dialoguesBetweenRounds.Length >= currentRound)
            {
                string[] linesToPlay = dialoguesBetweenRounds[currentRound - 1].lines;
                if (linesToPlay != null && linesToPlay.Length > 0)
                {
                    dialogueSystem.StartDialogue(linesToPlay);
                    yield return new WaitWhile(() => dialogueSystem.IsRunning);
                }
            }

            // 4. Timer começa
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
            SpawnNextPaper(true);
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
            StartCoroutine(TriggerGameOver("Você errou 3 vezes. O patrão não perdoa."));
        }
        else
        {
            SpawnNextPaper(true);
        }
    }

    private void SpawnNextPaper(bool isInteractive)
    {
        DestroyCurrentPaper();

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

        if (paperPrefab != null && spawnPoint != null)
        {
            activePaper = Instantiate(paperPrefab, spawnPoint);

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

    private IEnumerator TriggerGameOver(string reason)
    {
        isRoundActive = false;
        if (fadeController != null) yield return StartCoroutine(fadeController.FadeToBlack(1f));
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    private IEnumerator VictorySequence()
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.StartDialogue(new string[] { "Inacreditável. Você sobreviveu.", "Agora volte ao trabalho." });
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        if (fadeController != null) yield return StartCoroutine(fadeController.FadeToBlack(1f));
        SceneManager.LoadScene(nextSceneName);
    }
}