using UnityEngine;

public class AutoDialogueStarter : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private NPCData npcData;
    [SerializeField] private float delayToStart = 0.5f;

    private bool started = false;

    void Start()
    {
        Invoke(nameof(StartDialogue), delayToStart);
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
    }
}