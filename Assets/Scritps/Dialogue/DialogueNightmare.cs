using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueNightmare : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] public TMP_Text dialogueText;
    [SerializeField] private Image characterImage;
    [SerializeField] private GameObject pressingEIndicator;

    [Header("Config")]
    [SerializeField] private float velocidadeTexto = 0.05f;
    [SerializeField] public TopDownMovement playerMovement;
    [SerializeField] private string interactButton = "Interact"; // ✅ Adicionado

    public static DialogueNightmare Instance { get; private set; }

    private string[] currentFalas;
    private Sprite[] currentSprites;
    private bool dialogoAtivo = false;
    private int index = 0;
    private DialogueTyping typingSystem;
    private DialogueAnim panelAnimator;
    private bool waitingForAnimation = false;
    private PulseEffect pressingEPulse;

    private bool isJumpscareDialogue = false;
    private Action onDialogueFinished;

    public bool DialogoAtivo => dialogoAtivo;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ✅ NOVO: Escuta o botão para pular/fechar o diálogo
    void Update()
    {
        if (!dialogoAtivo || waitingForAnimation) return;

        // Se o jogador apertar E, avança o diálogo
        if (Input.GetButtonDown(interactButton))
        {
            AdvanceDialogue();
        }
    }

    public void SetNPCDialogue(string[] falas, Sprite[] sprites)
    {
        currentFalas = falas;
        currentSprites = sprites;
        typingSystem.UpdateDialogue(falas);
        isJumpscareDialogue = false;
        onDialogueFinished = null;
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
        panelAnimator.ShowPanel(() =>
        {
            waitingForAnimation = false;
            typingSystem.StartTyping(index);
            ShowPressingEIfNeeded();
        });
    }

    public void StartDialogue(string[] falas, Action onFinished = null)
    {
        if (dialogoAtivo || falas == null || falas.Length == 0)
            return;

        isJumpscareDialogue = true;
        onDialogueFinished = onFinished;

        currentFalas = falas;
        currentSprites = null;
        typingSystem.UpdateDialogue(falas);

        if (playerMovement != null)
            playerMovement.canMove = true;

        dialogoAtivo = true;
        index = 0;

        if (pressingEIndicator != null)
            pressingEIndicator.SetActive(false);

        waitingForAnimation = true;
        panelAnimator.ShowPanel(() =>
        {
            waitingForAnimation = false;
            typingSystem.StartTyping(index);
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

                if (!isJumpscareDialogue)
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

        bool wasJumpscare = isJumpscareDialogue;
        Action finishedCallback = onDialogueFinished;

        panelAnimator.HidePanel(() =>
        {
            dialogoAtivo = false;

            if (!wasJumpscare && playerMovement != null)
                playerMovement.canMove = true;

            currentFalas = null;
            currentSprites = null;
            isJumpscareDialogue = false;
            onDialogueFinished = null;

            finishedCallback?.Invoke();
        });
    }

    public bool IsFinished()
    {
        return !dialogoAtivo;
    }

    public void ForceClose()
    {
        if (!dialogoAtivo) return;

        typingSystem.StopTyping();

        if (pressingEIndicator != null)
        {
            if (pressingEPulse != null)
                pressingEPulse.StopPulse();
            pressingEIndicator.SetActive(false);
        }

        dialoguePanel.SetActive(false);
        dialogoAtivo = false;
        waitingForAnimation = false;
        currentFalas = null;
        currentSprites = null;
        isJumpscareDialogue = false;
        onDialogueFinished = null;
    }
}