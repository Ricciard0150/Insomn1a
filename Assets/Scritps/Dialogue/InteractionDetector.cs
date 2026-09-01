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
        // ✅ REGISTRAR NO TEXTMANAGER (NOVO)
        if (TextManager.Instance != null)
        {
            TextManager.Instance.RegisterNPC(this);
            Debug.Log($"✅ {gameObject.name} registrado no TextManager");
        }
        else
        {
            Debug.LogWarning($"⚠️ TextManager não encontrado para {gameObject.name}");
        }

        // Seu código existente...
        if (pressingE != null)
        {
            if (pressingE.GetComponent<Image>() == null &&
                pressingE.GetComponent<SpriteRenderer>() == null)
            {
                Debug.LogWarning("PressingE não tem Image ou SpriteRenderer!");
            }

            pulseEffect = pressingE.GetComponent<PulseEffect>();
            if (pulseEffect == null)
                pulseEffect = pressingE.AddComponent<PulseEffect>();

            pressingE.SetActive(false);
        }
    }

    // ✅ DESREGISTRAR QUANDO DESTRUÍDO (NOVO)
    private void OnDestroy()
    {
        if (TextManager.Instance != null)
        {
            TextManager.Instance.UnregisterNPC(this);
            Debug.Log($"➖ {gameObject.name} removido do TextManager");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() != null)
        {
            Debug.Log("Player entrou na área do NPC");
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

        Debug.Log("Player saiu da área do NPC");
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
            dialogueSystem.SetNPCDialogue(npcData.falas, npcData.sprites);
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