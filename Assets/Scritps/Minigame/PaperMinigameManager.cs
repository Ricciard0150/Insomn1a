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
    public class DialogueLineData
    {
        [TextArea(2, 5)]
        public string text;

        [Tooltip("Se marcado, a mão vai animar E sortear um novo Spot enquanto essa fala está na tela.")]
        public bool playHandAnimation;
    }

    [System.Serializable]
    public class RoundDialogue
    {
        public DialogueLineData[] lines;
    }

    [Header("Diálogos Entre Rodadas")]
    [SerializeField] private RoundDialogue[] dialoguesBetweenRounds;

    [Header("Spots e Papéis")]
    [SerializeField] private PaperSpot[] allSpots;
    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Animação da Mão (Tutorial)")]
    [SerializeField] private GameObject handPrefab;
    [SerializeField] private Transform handStartPosition;
    [SerializeField] private Transform handTargetPoint;
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

                    dialogueSystem.StartDialogue(new string[] { line.text });
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

        // ✅ Cria o papel falso na raiz da cena (null)
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

        // ✅ Cria a mão na raiz da cena (null)
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
            // ✅ Força o papel real a nascer na raiz da cena também
            Vector3 spawnPos = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0f);
            activePaper = Instantiate(paperPrefab, spawnPos, Quaternion.identity, null);

            Collider2D paperCollider = activePaper.GetComponent<Collider2D>();
            if (paperCollider != null)
                paperCollider.enabled = isInteractive;
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