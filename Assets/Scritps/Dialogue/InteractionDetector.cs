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
            TextManager.Instance.RegisterNPC(this);

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

    private void Update()
    {
        // ✅ CORREÇÃO: Se o diálogo está aberto, finge que o player está perto.
        // Isso engana o TextManager e permite que o botão de interagir avance a fala,
        // mesmo que o player tenha saído do collider.
        if (dialogueSystem != null && dialogueSystem.DialogoAtivo)
        {
            playerPerto = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() != null)
        {
            playerPerto = true;
            if (pressingE != null && dialogueSystem != null && !dialogueSystem.DialogoAtivo)
            {
                pressingE.SetActive(true);
                if (pulseEffect != null) pulseEffect.StartPulse();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() == null) return;

        // Se o diálogo estiver ativo, ignora a saída do player (não queremos que ele perca a interação)
        if (dialogueSystem != null && dialogueSystem.DialogoAtivo) return;

        playerPerto = false;
        if (pressingE != null)
        {
            if (pulseEffect != null) pulseEffect.StopPulse();
            pressingE.SetActive(false);
        }
    }

    public void Interact()
    {
        if (dialogueSystem == null || npcData == null) return;

        // ✅ CORREÇÃO: Se o diálogo já está ativo, avança IMEDIATAMENTE.
        // Não checa playerPerto aqui dentro, porque o Update já garantiu que é true.
        if (dialogueSystem.DialogoAtivo)
        {
            dialogueSystem.AdvanceDialogue();
            return;
        }

        // Só abre o diálogo se o player estiver realmente perto
        if (!playerPerto) return;

        bool passouNightmare = SaveSystem.Instance != null && SaveSystem.Instance.HasPassedNightmare();

        string[] falasEscolhidas;
        Sprite[] spritesEscolhidos;
        string[] nomesEscolhidos;
        bool[] mostrarSpriteEscolhido;

        if (passouNightmare && npcData.falasAfterNightmare != null && npcData.falasAfterNightmare.Length > 0)
        {
            falasEscolhidas = npcData.falasAfterNightmare;
            spritesEscolhidos = npcData.spritesAfterNightmare;
            nomesEscolhidos = npcData.nomesAfterNightmare;
            mostrarSpriteEscolhido = npcData.mostrarSpriteAfter;
        }
        else
        {
            falasEscolhidas = npcData.falasBeforeNightmare;
            spritesEscolhidos = npcData.spritesBeforeNightmare;
            nomesEscolhidos = npcData.nomesBeforeNightmare;
            mostrarSpriteEscolhido = npcData.mostrarSpriteBefore;
        }

        dialogueSystem.SetNPCDialogue(falasEscolhidas, spritesEscolhidos, nomesEscolhidos, npcData.npcName, mostrarSpriteEscolhido);
        dialogueSystem.StartDialogue();

        if (pressingE != null)
        {
            if (pulseEffect != null) pulseEffect.StopPulse();
            pressingE.SetActive(false);
        }
    }
}