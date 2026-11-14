using UnityEngine;

public class CollectibleLetterPiece : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private string itemName = "Pedaço de carta";
    [SerializeField] private int pieceNumber = 1;
    [SerializeField] private int totalPieces = 3;

    [Header("References")]
    [SerializeField] private LetterPickupPanel pickupPanel;

    [Header("Input")]
    [SerializeField] private string interactButton = "Interact";

    private bool playerNear = false;
    private bool collected = false;

    void Start()
    {
        // Se já foi coletado antes, some
        //if (PlayerPrefs.GetInt("letter_piece_" + pieceNumber, 0) == 1)
        //    gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerNear && !collected && Input.GetButtonDown(interactButton))
        {
            collected = true;

            PlayerPrefs.SetInt("letter_piece_" + pieceNumber, 1);

            int collectedCount = 0;
            for (int i = 1; i <= totalPieces; i++)
                if (PlayerPrefs.GetInt("letter_piece_" + i, 0) == 1)
                    collectedCount++;

            PlayerPrefs.Save();

            if (pickupPanel != null)
                pickupPanel.Show(itemSprite, itemName, collectedCount, totalPieces);
            else
                Debug.LogError("PICKUP PANEL NULL");

            gameObject.SetActive(false);
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
}