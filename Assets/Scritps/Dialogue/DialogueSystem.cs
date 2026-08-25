using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] public TMP_Text dialogueText;
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject pressingEIndicator; 

    [Header("Config")]
    [SerializeField] private float velocidadeTexto = 0.05f;
    [SerializeField] public TopDownMovement playerMovement;

    private string[] currentFalas;
    private Sprite[] currentSprites;
    private bool dialogoAtivo = false;
    private int index = 0;
    private DialogueTyping typingSystem;
    private DialogueAnim panelAnimator;
    private bool waitingForAnimation = false;
    private PulseEffect pressingEPulse;

    public bool DialogoAtivo => dialogoAtivo;

    void Awake()
    {
        dialoguePanel.SetActive(false);

        if (pressingEIndicator != null)
        {
            pressingEIndicator.SetActive(false);

            pressingEPulse = pressingEIndicator.GetComponent<PulseEffect>();
            if (pressingEPulse == null)
                pressingEPulse = pressingEIndicator.AddComponent<PulseEffect>();
        }

        panelAnimator = dialoguePanel.GetComponent<DialogueAnim>();
        if (panelAnimator == null)
            panelAnimator = dialoguePanel.AddComponent<DialogueAnim>();

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
        index = 0;
        ChangeSprite();

        if (pressingEIndicator != null)
            pressingEIndicator.SetActive(false);

        waitingForAnimation = true;
        panelAnimator.ShowPanel(() => {
            waitingForAnimation = false;
            typingSystem.StartTyping(index);
            ShowPressingEIfNeeded();
        });
    }

    public void AdvanceDialogue()
    {
        if (!dialogoAtivo || waitingForAnimation) return;

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
                ShowPressingEIfNeeded();
            }
            else
            {
                CloseDialogue();
            }
        }
    }

    private void ShowPressingEIfNeeded()
    {
        if (pressingEIndicator != null && index == currentFalas.Length - 1)
        {
            pressingEIndicator.SetActive(true);
            if (pressingEPulse != null)
                pressingEPulse.StartPulse();
        }
        else if (pressingEIndicator != null)
        {
            pressingEIndicator.SetActive(false);
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

        if (pressingEIndicator != null)
        {
            if (pressingEPulse != null)
                pressingEPulse.StopPulse();
            pressingEIndicator.SetActive(false);
        }

        panelAnimator.HidePanel(() => {
            dialogoAtivo = false;

            if (playerMovement != null)
                playerMovement.canMove = true;

            currentFalas = null;
            currentSprites = null;
        });
    }
}