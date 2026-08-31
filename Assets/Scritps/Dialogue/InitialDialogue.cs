using UnityEngine;

public class InitialDialogueStarter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private NPCData npcData;
    [SerializeField] private GameObject NPC;

    [Header("Settings")]
    [SerializeField] private float delayToStart = 0.5f;
    [SerializeField] private string dialogueID = "InitialDialogue";

    private bool started = false;
    private bool dialogueFinished = false;

    void Start()
    {
        // ✅ Verificar se o diálogo já foi mostrado (usando o sistema de save)
        if (SaveSystem.Instance != null && SaveSystem.Instance.IsInitialDialogueShown())
        {
            Debug.Log($"Diálogo inicial '{dialogueID}' já foi mostrado (save). Destruindo NPC.");
            Destroy(NPC);
            return;
        }

        if (dialogueSystem == null || npcData == null)
        {
            Debug.LogError($"InitialDialogueStarter: Referências faltando em {gameObject.name}!");
            Destroy(NPC);
            return;
        }

        Invoke(nameof(StartDialogue), delayToStart);
    }

    void Update()
    {
        if (!started || dialogueFinished) return;
        if (dialogueSystem == null) return;

        if (!dialogueSystem.DialogoAtivo && started)
        {
            FinishDialogue();
            return;
        }

        if (dialogueSystem.DialogoAtivo && Input.GetButtonDown("Interact"))
        {
            dialogueSystem.AdvanceDialogue();
        }
    }

    void StartDialogue()
    {
        if (dialogueSystem == null || npcData == null)
        {
            Destroy(this);
            return;
        }

        dialogueSystem.SetNPCDialogue(npcData.falas, npcData.sprites);
        dialogueSystem.StartDialogue();
        started = true;

        // ✅ Marcar como mostrado no sistema de save
        MarkDialogueAsShown();
    }

    void FinishDialogue()
    {
        if (dialogueFinished) return;

        dialogueFinished = true;
        Debug.Log($"Diálogo inicial '{dialogueID}' finalizado! Destruindo NPC.");
        Destroy(NPC);
    }

    void MarkDialogueAsShown()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SetInitialDialogueShown(true);
        }
        else
        {
            PlayerPrefs.SetInt($"GlobalDialogue_{dialogueID}", 1);
            PlayerPrefs.Save();
        }
    }

    void OnDestroy()
    {
        if (dialogueSystem != null && dialogueSystem.DialogoAtivo)
        {
            dialogueSystem.CloseDialogue();
        }
    }
}