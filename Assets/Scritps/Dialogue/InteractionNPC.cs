using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPCData npcData; 
    [SerializeField] private DialogueSystem dialogueSystem; 
    [SerializeField] private GameObject pressingE;

    private bool playerPerto = false;

    public bool PlayerPerto => playerPerto;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() != null)
        {
            Debug.Log("ds");
            playerPerto = true;
            if (pressingE != null)
                pressingE.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() == null)
            return;

        playerPerto = false;
        if (pressingE != null)
            pressingE.SetActive(false);
    }

    public void Interact()
    {
        if (playerPerto && dialogueSystem != null && npcData != null)
        {
            dialogueSystem.SetNPCDialogue(npcData.falas, npcData.sprites);

            if (!dialogueSystem.DialogoAtivo)
                dialogueSystem.StartDialogue();
            else
                dialogueSystem.AdvanceDialogue();
        }
    }
}