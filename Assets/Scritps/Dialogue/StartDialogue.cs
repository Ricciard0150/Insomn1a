using UnityEngine;

public class AutoDialogueStarter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private NPCData npcData;

    [Header("Settings")]
    [SerializeField] private float delayToStart = 0.5f;
    [SerializeField] private string dialogueID = "UniqueDialogue";

    private bool started = false;
    private bool dialogueFinished = false;
    private string playerPrefKey;

    void Start()
    {
        playerPrefKey = $"GlobalDialogue_{dialogueID}";

        // Verifica se o diálogo já foi mostrado antes
        if (PlayerPrefs.GetInt(playerPrefKey, 0) == 1)
        {
            Debug.Log($"Diálogo '{dialogueID}' já foi mostrado antes. Destruindo AGORA.");
            Destroy(gameObject);
            return;
        }

        // Verifica se as referências estão configuradas
        if (dialogueSystem == null || npcData == null)
        {
            Debug.LogError($"AutoDialogueStarter: Referências faltando em {gameObject.name}!");
            Destroy(gameObject);
            return;
        }

        // Agenda o início do diálogo
        Invoke(nameof(StartDialogue), delayToStart);
    }

    void Update()
    {
        if (!started || dialogueFinished) return;
        if (dialogueSystem == null) return;

        // Verifica se o diálogo terminou (foi fechado)
        if (!dialogueSystem.DialogoAtivo && started)
        {
            FinishDialogue();
            return;
        }

        // Avança o diálogo com o botão Interact
        if (dialogueSystem.DialogoAtivo && Input.GetButtonDown("Interact"))
        {
            dialogueSystem.AdvanceDialogue();
        }
    }

    void StartDialogue()
    {
        if (dialogueSystem == null || npcData == null)
        {
            Destroy(gameObject);
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
        Debug.Log($"Diálogo '{dialogueID}' finalizado! Destruindo GameObject.");

        // Destrói o GameObject APÓS o diálogo terminar
        Destroy(gameObject);
    }

    void MarkDialogueAsShown()
    {
        PlayerPrefs.SetInt(playerPrefKey, 1);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        // Garante que o diálogo seja fechado se o GameObject for destruído
        if (dialogueSystem != null && dialogueSystem.DialogoAtivo)
        {
            dialogueSystem.CloseDialogue();
        }
    }
}