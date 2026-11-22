using System.Collections;
using UnityEngine;

public class StartMinigameNPC : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PaperMinigameManager minigameManager;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private InterviewQuests dialogueSystem;

    [Header("Configurações")]
    // ✅ AGORA É DialogueLine[] EM VEZ DE string[]
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
        isStarting = true;

        if (fadeController != null)
            yield return StartCoroutine(fadeController.FadeToBlack(1f));

        if (dialogueSystem != null && introDialogueLines.Length > 0)
        {
            // ✅ Agora passa o DialogueLine[] direto
            dialogueSystem.StartDialogue(introDialogueLines);
            yield return new WaitWhile(() => dialogueSystem.IsRunning);
        }

        if (minigameManager != null)
        {
            minigameManager.StartMinigame();
        }
    }
}