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
    [SerializeField] private string interactButton = "Interact";

    [Header("Comportamento")]
    [Tooltip("Se true, o texto aparece letra por letra. Se false, aparece de uma vez.")]
    [SerializeField] private bool usarTypewriter = false;

    [Tooltip("Bloquear o movimento do player enquanto o diálogo estiver aberto?")]
    [SerializeField] private bool bloquearMovimento = true;

    [Tooltip("Overload do Door que antes liberava o player (jumpscare). Se true, mantém o player livre nesse overload.")]
    [SerializeField] private bool liberarPlayerNoOverloadJumpscare = true;

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
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        dialoguePanel.SetActive(false);

        if (dialogueText != null) dialogueText.text = ""; // limpa o "New Text"

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

        // ✅ passa usarTypewriter para o typing system
        typingSystem.Initialize(dialogueText, velocidadeTexto, usarTypewriter);
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    void Update()
    {
        if (!dialogoAtivo || waitingForAnimation) return;
        if (Input.GetButtonDown(interactButton)) AdvanceDialogue();
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
        if (dialogoAtivo || currentFalas == null || currentFalas.Length == 0) return;

        if (bloquearMovimento && playerMovement != null)
            playerMovement.canMove = false;

        dialogoAtivo = true;
        index = 0;
        ChangeSprite();

        if (pressingEIndicator != null) pressingEIndicator.SetActive(false);

        // ✅ limpa o texto ANTES de animar — resolve o "New Text" aparecendo durante o slide
        if (dialogueText != null) dialogueText.text = "";

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
        if (dialogoAtivo || falas == null || falas.Length == 0) return;

        isJumpscareDialogue = true;
        onDialogueFinished = onFinished;

        currentFalas = falas;
        currentSprites = null;
        typingSystem.UpdateDialogue(falas);

        // ✅ controla o player conforme configuração
        if (playerMovement != null)
            playerMovement.canMove = liberarPlayerNoOverloadJumpscare;

        dialogoAtivo = true;
        index = 0;

        if (pressingEIndicator != null) pressingEIndicator.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

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
                if (!isJumpscareDialogue) ShowPressingEIfNeeded();
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
            if (pressingEPulse != null) pressingEPulse.StartPulse();
        }
        else if (pressingEIndicator != null)
        {
            pressingEIndicator.SetActive(false);
        }
    }

    private void ChangeSprite()
    {
        if (currentSprites != null && index < currentSprites.Length && characterImage != null)
            characterImage.sprite = currentSprites[index];
    }

    public void CloseDialogue()
    {
        typingSystem.StopTyping();
        if (pressingEIndicator != null)
        {
            if (pressingEPulse != null) pressingEPulse.StopPulse();
            pressingEIndicator.SetActive(false);
        }

        bool wasJumpscare = isJumpscareDialogue;
        Action finishedCallback = onDialogueFinished;

        panelAnimator.HidePanel(() =>
        {
            dialogoAtivo = false;

            // ✅ libera o player sempre ao fechar (exceto se era jumpscare — que já estava livre)
            if (playerMovement != null && !wasJumpscare)
                playerMovement.canMove = true;

            currentFalas = null;
            currentSprites = null;
            isJumpscareDialogue = false;
            onDialogueFinished = null;

            if (dialogueText != null) dialogueText.text = ""; // ✅ limpa ao fechar

            finishedCallback?.Invoke();
        });
    }

    public bool IsFinished() => !dialogoAtivo;

    public void ForceClose()
    {
        if (!dialogoAtivo) return;
        typingSystem.StopTyping();
        if (pressingEIndicator != null)
        {
            if (pressingEPulse != null) pressingEPulse.StopPulse();
            pressingEIndicator.SetActive(false);
        }
        dialoguePanel.SetActive(false);
        dialogoAtivo = false;
        waitingForAnimation = false;
        currentFalas = null;
        currentSprites = null;
        isJumpscareDialogue = false;
        onDialogueFinished = null;
        if (playerMovement != null) playerMovement.canMove = true; // ✅ libera
        if (dialogueText != null) dialogueText.text = "";
    }
}