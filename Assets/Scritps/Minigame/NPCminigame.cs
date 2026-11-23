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
    private bool waitingForMinigame = false;

    void Update()
    {
        if (playerNear && !isStarting && Input.GetButtonDown(interactButton))
        {
            StartCoroutine(StartMinigameSequence());
        }

        // ✅ Monitora quando o último diálogo do minigame terminar para voltar a música
        if (waitingForMinigame && dialogueSystem != null)
        {
            // Se o minigame não está mais ativo E o diálogo terminou
            if (!dialogueSystem.IsRunning)
            {
                waitingForMinigame = false;
                // Volta a música de fundo
                if (AudioManager.Instance != null)
                    AudioManager.Instance.RetomarMusica();
            }
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
        isStarting = true;

        // ✅ 1. Para a música de fundo IMEDIATAMENTE ao interagir
        if (AudioManager.Instance != null)
            AudioManager.Instance.PausarMusica();

        if (fadeController != null)
            yield return StartCoroutine(fadeController.FadeToBlack(1f));

        if (dialogueSystem != null && introDialogueLines.Length > 0)
        {
            dialogueSystem.StartDialogue(introDialogueLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        if (minigameManager != null)
        {
            // ✅ 2. Inicia o minigame. O PainelAudioController vai cuidar de tocar a música do minigame.
            minigameManager.StartMinigame();

            // ✅ 3. Ativa a flag para começar a monitorar o fim do minigame
            waitingForMinigame = true;
        }
    }
}