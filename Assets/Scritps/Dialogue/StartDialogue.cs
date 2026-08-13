using UnityEngine;

public class AutoDialogueStarter : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private NPCData npcData;
    [SerializeField] private float delayToStart = 0.5f;
    [SerializeField] private string dialogueID = "UniqueDialogue"; // Identificador único para este diálogo em todo o jogo

    private bool started = false;

    void Start()
    {
        // Verifica se este diálogo específico já foi executado em TODO O JOGO
        string playerPrefKey = $"GlobalDialogue_{dialogueID}";

        if (PlayerPrefs.GetInt(playerPrefKey, 0) == 0)
        {
            // Primeira vez que este diálogo é executado em toda a vida do jogo
            Invoke(nameof(StartDialogue), delayToStart);
        }
        else
        {
            // Já foi executado antes em alguma cena anterior
            Debug.Log($"Diálogo '{dialogueID}' já foi executado anteriormente em todo o jogo.");
            Destroy(this); // Ou gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!started) return;
        if (!dialogueSystem.DialogoAtivo) return;

        if (Input.GetButtonDown("Interact"))
        {
            dialogueSystem.AdvanceDialogue();
        }
    }

    void StartDialogue()
    {
        if (dialogueSystem == null || npcData == null) return;

        dialogueSystem.SetNPCDialogue(npcData.falas, npcData.sprites);
        dialogueSystem.StartDialogue();
        started = true;

        // Marca que este diálogo já foi executado em todo o jogo
        string playerPrefKey = $"GlobalDialogue_{dialogueID}";
        PlayerPrefs.SetInt(playerPrefKey, 1);
        PlayerPrefs.Save();
    }
}