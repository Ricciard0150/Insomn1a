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
    [SerializeField] private bool resetOnStart = false; 

    private bool started = false;
    private bool dialogueFinished = false;
    private string playerPrefKey;

    void Start()
    {
        playerPrefKey = $"GlobalDialogue_{dialogueID}";

        // Para testes - reseta o PlayerPrefs
        if (resetOnStart)
        {
            PlayerPrefs.DeleteKey(playerPrefKey);
            PlayerPrefs.Save();
            Debug.Log($"Resetando diálogo '{dialogueID}' para teste.");
        }

        //if (PlayerPrefs.GetInt(playerPrefKey, 0) == 1)
        //{
        //    Debug.Log($"Diálogo inicial '{dialogueID}' já foi mostrado. Destruindo script.");
        //    Destroy(NPC); 
        //    return;
        //}

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

        // Configura e inicia o diálogo
        dialogueSystem.SetNPCDialogue(npcData.falas, npcData.sprites);
        dialogueSystem.StartDialogue();
        started = true;

        // Marca como mostrado
        MarkDialogueAsShown();
    }

    void FinishDialogue()
    {
        if (dialogueFinished) return;

        dialogueFinished = true;
        Debug.Log($"Diálogo inicial '{dialogueID}' finalizado! Destruindo script.");

        // Destroi APENAS este script, mantendo o GameObject
        Destroy(NPC);
    }

    void MarkDialogueAsShown()
    {
        PlayerPrefs.SetInt(playerPrefKey, 1);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        // Se o diálogo ainda estiver ativo quando o script for destruído, fecha
        if (dialogueSystem != null && dialogueSystem.DialogoAtivo)
        {
            dialogueSystem.CloseDialogue();
        }
    }
}