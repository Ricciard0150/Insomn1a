using System.Collections;
using UnityEngine;

public class CollectibleLetterPiece : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private string itemName = "Pedaço de carta";
    [SerializeField] private int pieceNumber = 1;

    [Header("References")]
    [SerializeField] private LetterPickupPanel pickupPanel;
    [SerializeField] private InterviewQuests dialogueSystem;

    [Header("Diálogo Final (Ao pegar o pedaço 3)")]
    [TextArea(2, 5)]
    [SerializeField] private string[] finalLetterDialogue;

    [Header("Consequences (Após o diálogo final)")]
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    [Header("Input")]
    [SerializeField] private string interactButton = "Interact";

    private bool playerNear = false;
    private bool collected = false;

    void Start()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasLetterPiece(pieceNumber))
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerNear && !collected && Input.GetButtonDown(interactButton))
        {
            collected = true;

            if (SaveSystem.Instance != null)
                SaveSystem.Instance.CollectLetterPiece(pieceNumber);

            int collectedCount = SaveSystem.Instance != null ? SaveSystem.Instance.GetCollectedLetterPieceCount() : 1;
            int total = SaveSystem.Instance != null ? SaveSystem.Instance.GetTotalLetterPieces() : 3;

            if (pickupPanel != null)
            {
                pickupPanel.Show(itemName, pieceNumber, collectedCount, total);

                // Se for o ÚLTIMO pedaço, inscreve no evento do painel para tocar o diálogo quando ele fechar
                if (collectedCount >= total)
                {
                    pickupPanel.OnPanelClosed += ShowFinalDialogue;
                }
            }

            gameObject.SetActive(false);
        }
    }

    // ✅ CHAMA AS CONSEQUÊNCIAS E DEPOIS O DIÁLOGO
    private void ShowFinalDialogue()
    {
        // Remove a inscrição para não tocar duas vezes
        pickupPanel.OnPanelClosed -= ShowFinalDialogue;

        // 1. Aplica as consequências PRIMEIRO (ativa/desativa objetos)
        ApplyConsequences();

        // 2. Depois inicia o diálogo
        if (dialogueSystem != null && finalLetterDialogue.Length > 0)
        {
            dialogueSystem.StartDialogue(finalLetterDialogue);
        }
    }

    private void ApplyConsequences()
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

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) playerNear = true; }
    void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) playerNear = false; }
}