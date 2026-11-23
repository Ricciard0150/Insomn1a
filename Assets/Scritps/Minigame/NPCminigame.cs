using System.Collections;
using UnityEngine;

public class StartMinigameNPC : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PaperMinigameManager minigameManager;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private InterviewQuests dialogueSystem;

    [Header("Configurações")]
    [SerializeField] private DialogueLine[] introDialogueLines;
    [SerializeField] private string interactButton = "Interact";

    private bool playerNear = false;
    private bool isStarting = false;

    void Update()
    {
        if (playerNear && !isStarting && Input.GetButtonDown(interactButton))
        {
            StartCoroutine(StartMinigameSequence());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }

    private IEnumerator StartMinigameSequence()
    {
        if (minigameManager == null) yield break;
        if (minigameManager.IsMinigameRunning) yield break;

        isStarting = true;

        // ✅ 1. Para a música geral IMEDIATAMENTE ao interagir
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PausarMusica();
            Debug.Log("[Áudio] Interação: música geral PAUSADA.");
        }

        // ✅ 2. Fade
        if (fadeController != null)
            yield return StartCoroutine(fadeController.FadeToBlack(1f));

        // ✅ 3. Diálogo de introdução (sem música de fundo)
        if (dialogueSystem != null && introDialogueLines != null && introDialogueLines.Length > 0)
        {
            dialogueSystem.StartDialogue(introDialogueLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        // ✅ 4. Abre o painel → PainelAudioController.OnEnable toca a música do minigame
        minigameManager.StartMinigame();

        isStarting = false;
    }
}