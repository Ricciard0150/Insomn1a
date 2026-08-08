using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] public TMP_Text dialogueText;
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject pressingE;

    [Header("Config")]
    [SerializeField] private float velocidadeTexto = 0.05f;
    [SerializeField] private TopDownMovement playerMovement; 

    private string[] currentFalas;
    private Sprite[] currentSprites;
    private bool dialogoAtivo = false;
    private int index = 0;
    private DialogueTyping typingSystem;

    public bool DialogoAtivo => dialogoAtivo;

    void Awake()
    {
        dialoguePanel.SetActive(false);
        pressingE.SetActive(false);

        typingSystem = GetComponent<DialogueTyping>();
        if (typingSystem == null)
            typingSystem = gameObject.AddComponent<DialogueTyping>();

        typingSystem.Initialize(dialogueText, velocidadeTexto);
    }

    public void SetNPCDialogue(string[] falas, Sprite[] sprites)
    {
        currentFalas = falas;
        currentSprites = sprites;
        typingSystem.UpdateDialogue(falas);
    }

    public void StartDialogue()
    {
        if (dialogoAtivo || currentFalas == null || currentFalas.Length == 0)
            return;

        if (playerMovement != null)
            playerMovement.canMove = false;

        dialogoAtivo = true;
        dialoguePanel.SetActive(true);
        index = 0;

        ChangeSprite();
        typingSystem.StartTyping(index);
    }

    public void AdvanceDialogue()
    {
        if (!dialogoAtivo) return;

        if (typingSystem.IsTyping)
        {
            typingSystem.CompleteTyping(index);
        }
        else
        {
            index++;

            if (index < currentFalas.Length)
            {
                ChangeSprite();
                typingSystem.StartTyping(index);
            }
            else
            {
                CloseDialogue();
            }
        }
    }

    private void ChangeSprite()
    {
        if (currentSprites != null && index < currentSprites.Length)
            characterImage.sprite = currentSprites[index];
    }

    public void CloseDialogue()
    {
        typingSystem.StopTyping();
        dialoguePanel.SetActive(false);
        dialogoAtivo = false;

        if (playerMovement != null)
            playerMovement.canMove = true;

        currentFalas = null;
        currentSprites = null;
    }
}