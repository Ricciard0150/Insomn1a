using UnityEngine;

public class CollectibleLetterPiece : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private string itemName = "Pedaço de carta";
    [SerializeField] private int pieceNumber = 1;

    [Header("References")]
    [SerializeField] private LetterPickupPanel pickupPanel;
    [SerializeField] private string interactButton = "Interact";

    private bool playerNear = false;
    private bool collected = false;

    void Start()
    {
        // Se já foi coletado antes, some
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

            // Salva no sistema
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.CollectLetterPiece(pieceNumber);

            // Pega os dados do SaveSystem
            int collectedCount = SaveSystem.Instance != null ? SaveSystem.Instance.GetCollectedLetterPieceCount() : 1;
            int total = SaveSystem.Instance != null ? SaveSystem.Instance.GetTotalLetterPieces() : 3;

            // ✅ É AQUI QUE VOCÊ COLOCA A LINHA
            if (pickupPanel != null)
                pickupPanel.Show(itemName, pieceNumber, collectedCount, total);

            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerNear = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerNear = false;
    }
}