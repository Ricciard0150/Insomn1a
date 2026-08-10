using UnityEngine;

public class ThoughtsD : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private NPCData npcData;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private float delayToStart = 0.2f;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IStatusPlayer player)) return;
        if (triggerOnce && triggered) return;
        if (dialogueSystem == null || npcData == null) return;
        if (dialogueSystem.DialogoAtivo) return;

        triggered = true;
        Invoke(nameof(StartDialogue), delayToStart);
    }

    void Update()
    {
        if (dialogueSystem != null && dialogueSystem.DialogoAtivo)
        {
            if (Input.GetButtonDown("Interact"))
            {
                dialogueSystem.AdvanceDialogue(); 
            }
        }
    }

    void StartDialogue()
    {
        dialogueSystem.SetNPCDialogue(npcData.falas, npcData.sprites);
        dialogueSystem.StartDialogue();
    }

    public void ResetTrigger()
    {
        triggered = false;
    }
}