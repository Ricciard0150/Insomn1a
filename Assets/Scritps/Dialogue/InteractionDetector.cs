using UnityEngine;
using UnityEngine.UI;

public class InteractionDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPCData npcData;
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private GameObject pressingE;

    private bool playerPerto = false;
    private PulseEffect pulseEffect;

    public bool PlayerPerto => playerPerto;

    private void Awake()
    {
        if (TextManager.Instance != null)
        {
            TextManager.Instance.RegisterNPC(this);
        }

        if (pressingE != null)
        {
            pulseEffect = pressingE.GetComponent<PulseEffect>();
            if (pulseEffect == null)
                pulseEffect = pressingE.AddComponent<PulseEffect>();

            pressingE.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (TextManager.Instance != null)
            TextManager.Instance.UnregisterNPC(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() != null)
        {
            playerPerto = true;

            if (pressingE != null && !dialogueSystem.DialogoAtivo)
            {
                pressingE.SetActive(true);
                if (pulseEffect != null)
                    pulseEffect.StartPulse();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() == null)
            return;

        playerPerto = false;

        if (pressingE != null)
        {
            if (pulseEffect != null)
                pulseEffect.StopPulse();
            pressingE.SetActive(false);
        }
    }

    public void Interact()
    {
        if (!playerPerto || dialogueSystem == null || npcData == null)
            return;

        if (!dialogueSystem.DialogoAtivo)
        {
            bool passouNightmare = SaveSystem.Instance != null && SaveSystem.Instance.HasPassedNightmare();

            string[] falasEscolhidas;
            Sprite[] spritesEscolhidos;

            if (passouNightmare && npcData.falasAfterNightmare != null && npcData.falasAfterNightmare.Length > 0)
            {
                falasEscolhidas = npcData.falasAfterNightmare;
                spritesEscolhidos = npcData.spritesAfterNightmare;
            }
            else
            {
                falasEscolhidas = npcData.falasBeforeNightmare;
                spritesEscolhidos = npcData.spritesBeforeNightmare;
            }

            // ✅ Agora passa o nome do NPC também
            dialogueSystem.SetNPCDialogue(falasEscolhidas, spritesEscolhidos, npcData.npcName);
            dialogueSystem.StartDialogue();

            if (pressingE != null)
            {
                if (pulseEffect != null)
                    pulseEffect.StopPulse();
                pressingE.SetActive(false);
            }
        }
        else
        {
            dialogueSystem.AdvanceDialogue();
        }
    }
}