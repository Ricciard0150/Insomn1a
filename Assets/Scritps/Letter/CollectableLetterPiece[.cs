using UnityEngine;

public class CollectibleLetterPiece : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private string itemName = "Pedaço de carta";
    [SerializeField] private int pieceNumber = 1;

    [Header("References")]
    [SerializeField] private LetterPickupPanel pickupPanel;
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

            if (SaveSystem.Instance == null)
            {
                Debug.LogError("[Carta] SaveSystem é NULL!");
                return;
            }

            // ✅ LOG PARA VER O QUE ESTÁ ACONTECENDO ANTES DE SALVAR
            Debug.Log($"[Carta] Tentando coletar o pedaço NÚMERO {pieceNumber}. O save atual diz: {SaveSystem.Instance.GetCollectedLetterPieceCount()}/3");

            SaveSystem.Instance.CollectLetterPiece(pieceNumber);

            int collectedCount = SaveSystem.Instance.GetCollectedLetterPieceCount();
            int total = SaveSystem.Instance.GetTotalLetterPieces();

            // ✅ LOG APÓS SALVAR
            Debug.Log($"[Carta] Coleta finalizada. Novo total no save: {collectedCount}/{total}");

            if (pickupPanel != null)
                pickupPanel.Show(itemSprite, itemName, collectedCount, total);

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