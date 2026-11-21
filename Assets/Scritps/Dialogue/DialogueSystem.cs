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

    // ✅ NOVO: Texto do nome
    [SerializeField] private TMP_Text characterNameText;

    [Header("Config")]
    [SerializeField] private float velocidadeTexto = 0.05f;
    [SerializeField] public TopDownMovement playerMovement;

    private string[] currentFalas;
    private Sprite[] currentSprites;
    private string[] currentNomesPorFala; // ✅ NOVO
    private string currentNPCName;       // Fallback
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

    // ✅ Versão original (mantida para não quebrar chamadas antigas)
    public void SetNPCDialogue(string[] falas, Sprite[] sprites, string npcName = "")
    {
        currentFalas = falas;
        currentSprites = sprites;
        currentNPCName = npcName;
        currentNomesPorFala = null; // usa o fallback
        typingSystem.UpdateDialogue(falas);
    }

    // ✅ NOVO: Versão com nomes por fala
    public void SetNPCDialogue(string[] falas, Sprite[] sprites, string[] nomesPorFala, string npcNameFallback = "")
    {
        currentFalas = falas;
        currentSprites = sprites;
        currentNPCName = npcNameFallback;
        currentNomesPorFala = nomesPorFala;
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
        UpdateName();

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
                UpdateName();
                typingSystem.StartTyping(index);
                ShowPressingEIfNeeded();
            }
            else
            {
                CloseDialogue();
            }
        }
    }

    private void UpdateName()
    {
        if (characterNameText == null) return;

        // Prioridade 1: nome específico da fala
        if (currentNomesPorFala != null && index < currentNomesPorFala.Length && !string.IsNullOrEmpty(currentNomesPorFala[index]))
        {
            characterNameText.text = currentNomesPorFala[index];
        }
        // Prioridade 2: nome geral do NPC
        else if (!string.IsNullOrEmpty(currentNPCName))
        {
            characterNameText.text = currentNPCName;
        }
        else
        {
            characterNameText.text = "";
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
            currentNomesPorFala = null;
            currentNPCName = "";
        });
    }
}