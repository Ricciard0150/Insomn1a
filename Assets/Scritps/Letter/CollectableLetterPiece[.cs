using System.Collections;
using UnityEngine;

public class CollectibleLetterPiece : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private string itemName = "Pedaço de carta";
    [Tooltip("Qual é o número DESTA carta? (1, 2 ou 3)")]
    [SerializeField] private int pieceNumber = 1;

    [Header("References")]
    [SerializeField] private LetterPickupPanel pickupPanel;
    [SerializeField] private InterviewQuests dialogueSystem;

    // ✅ AGORA: cada linha tem nome e sprite
    [Header("Diálogo Final (Ao pegar o pedaço 3)")]
    [SerializeField] private DialogueLine[] finalLetterDialogue;

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
            gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerNear && !collected && Input.GetButtonDown(interactButton))
        {
            collected = true;

            if (SaveSystem.Instance != null)
                SaveSystem.Instance.CollectLetterPiece(pieceNumber);

            int collectedCount = SaveSystem.Instance != null
                ? SaveSystem.Instance.GetCollectedLetterPieceCount() : 1;
            int total = SaveSystem.Instance != null
                ? SaveSystem.Instance.GetTotalLetterPieces() : 3;

            if (pickupPanel != null)
            {
                bool isFinal = collectedCount >= total;
                pickupPanel.Show(itemName, pieceNumber, collectedCount, total, () =>
                {
                    if (isFinal) ShowFinalDialogue();
                });
            }

            StartCoroutine(HideVisualsThenDisable());
        }
    }

    private IEnumerator HideVisualsThenDisable()
    {
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
            sr.enabled = false;

        if (pickupPanel != null)
            yield return new WaitWhile(() =>
                pickupPanel != null && pickupPanel.gameObject.activeSelf);

        gameObject.SetActive(false);
    }

    private void ShowFinalDialogue()
    {
        ApplyConsequences();

        if (dialogueSystem != null && finalLetterDialogue != null && finalLetterDialogue.Length > 0)
            dialogueSystem.StartDialogue(finalLetterDialogue);
        else
            Debug.LogWarning("[CollectibleLetterPiece] dialogueSystem nulo ou finalLetterDialogue vazio.");
    }

    private void ApplyConsequences()
    {
        if (objectsToActivate != null)
            foreach (GameObject obj in objectsToActivate)
                if (obj != null) obj.SetActive(true);

        if (objectsToDeactivate != null)
            foreach (GameObject obj in objectsToDeactivate)
                if (obj != null) obj.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) playerNear = true; }
    void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) playerNear = false; }
}