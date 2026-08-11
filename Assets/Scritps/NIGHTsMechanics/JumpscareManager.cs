using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JumpscareManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private JumpscareSequence sequence;
    [SerializeField] private BlurController blur;
    [SerializeField] private DM dialogue;
    [SerializeField] private GlassPunch punch;
    [SerializeField] private TopDownMovement playerMovement;
    [SerializeField] private Door door;
    [SerializeField] private CollectableItem keyItem;
    [SerializeField] private PressEIndicator pressEIndicator;
    [SerializeField] private FadeController fadeController;

    [Header("Input Settings")]
    [SerializeField] private string interactButton = "Interact";
    [SerializeField] private string blinkButton = "Blink";

    [Header("Settings")]
    [SerializeField] private float delayAfterBlink = 2f;
    [SerializeField] private float delayAfterDialogue = 0.5f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private string sceneToLoad = "GameOver";

    private bool isRunning = false;
    private bool waitingBlink = false;
    private bool waitingDialogue = false;
    private bool isActive = false;
    private bool panelAberto = false;
    private bool punchActivated = false;

    void Start()
    {
        isActive = true;
        ResetJumpscare();
        Debug.Log("✅ JumpscareManager INICIADO!");
    }

    void Update()
    {
        // RESETAR COM TECLA R
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetJumpscare();
            isActive = true;
            Debug.Log("🔄 RESETADO!");
        }

        // ESPAÇO PARA PISCAR
        if (waitingBlink && Input.GetButtonDown(blinkButton))
        {
            Debug.Log("✅ ESPAÇO PRESSIONADO! (PISCAR)");
            StartCoroutine(ProcessBlink());
            return;
        }

        if (isRunning) return;

        // E PARA ABRIR O PANEL
        if (Input.GetButtonDown(interactButton) && CanInteract())
        {
            Debug.Log("✅ E PRESSIONADO! Abrindo Panel...");
            StartSequence();
        }
    }

    private bool CanInteract()
    {
        return sequence.IsColliding() && !isRunning && door.JaInteragiuComChave() && keyItem.HasKey() && isActive;
    }

    public void ActivateJumpscare()
    {
        Debug.Log("🎬 ActivateJumpscare() CHAMADO!");
        isActive = true;

        // ✅ NÃO ATIVA O PUNCH AQUI! SÓ ATIVA O JUMPScare
        if (pressEIndicator != null && sequence.IsColliding())
            pressEIndicator.Show();

        Debug.Log("✅ JumpscareManager PRONTO! Aguardando interação com E...");
    }

    private void StartSequence()
    {
        Debug.Log("🎬 [PASSO 1] StartSequence() - Abrindo Panel e Blur");

        isRunning = true;
        waitingBlink = true;
        panelAberto = true;
        punchActivated = false;

        playerMovement.SetCanMove(false);
        blur.AtivarBlur();
        sequence.ShowPanel(true);

        if (pressEIndicator != null)
            pressEIndicator.Hide();

        dialogue.ShowText("Aperte ESPAÇO para piscar...", false);

        Debug.Log("🎬 [PASSO 1] Aguardando ESPAÇO para piscar...");
    }

    private IEnumerator ProcessBlink()
    {
        Debug.Log("👁️ [PASSO 2] ProcessBlink() - Piscando...");

        waitingBlink = false;

        yield return StartCoroutine(blur.PiscarComFade());
        Debug.Log("👁️ [PASSO 2] Piscada COMPLETA!");

        blur.DesativarBlur();
        Debug.Log("👁️ [PASSO 2] Blur DESATIVADO!");

        Debug.Log($"👁️ [PASSO 2] Esperando {delayAfterBlink} segundos...");
        yield return new WaitForSeconds(delayAfterBlink);

        // REVELA O SPRITE
        Debug.Log("👁️ [PASSO 2] Sprite REVELADO!");

        dialogue.Close();

        Debug.Log("👁️ [PASSO 2] Iniciando diálogo principal...");
        dialogue.StartDialogue(sequence.GetDialogueLines());
        waitingDialogue = true;

        // ⏳ ESPERA O DIÁLOGO TERMINAR
        yield return new WaitUntil(() => dialogue.IsFinished());
        waitingDialogue = false;
        Debug.Log("👁️ [PASSO 2] Diálogo FINALIZADO!");

        yield return new WaitForSeconds(delayAfterDialogue);

        // ✅ SÓ AGORA ATIVA O SOCO! (DEPOIS DO DIÁLOGO)
        Debug.Log("👁️ [PASSO 3] Diálogo terminou! ATIVANDO O SOCO...");
        if (punch != null)
        {
            // ATIVA A WINDOW SE NÃO ESTIVER ATIVA
            if (!punch.IsActive())
            {
                punch.ActivateWindow();
                Debug.Log("✅ Window ATIVADA!");
            }

            // HABILITA O SOCO
            punch.EnablePunch();
            Debug.Log("✅ Punch ENABLED! O jogador pode socar a janela agora!");
            punchActivated = true;
        }
        else
        {
            Debug.LogError("❌ punch é NULL no JumpscareManager!");
        }
    }

    // CHAMADO PELO GLASSPUNCH QUANDO O VIDRO QUEBRA
    public void OnGlassBroken()
    {
        Debug.Log("💥 Vidro quebrado! Iniciando FADE e LOAD CENA...");
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (fadeController != null)
        {
            Debug.Log($"🌑 Iniciando FadeToBlack com duração de {fadeDuration} segundos...");
            yield return StartCoroutine(fadeController.FadeToBlack(fadeDuration));
            Debug.Log("🌑 FadeToBlack FINALIZADO!");
        }
        else
        {
            Debug.LogError("❌ fadeController é NULL! Verifique a referência no Inspector.");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"📂 Carregando cena: {sceneToLoad}");

        playerMovement.SetCanMove(true);
        isRunning = false;
        panelAberto = false;
        punchActivated = false;

        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("📂 Cena carregada!");
    }

    public void ResetJumpscare()
    {
        Debug.Log("🔄 ResetJumpscare()");

        isRunning = false;
        waitingBlink = false;
        waitingDialogue = false;
        panelAberto = false;
        punchActivated = false;

        if (playerMovement != null)
            playerMovement.SetCanMove(true);

        if (sequence != null)
            sequence.ShowPanel(false);

        if (blur != null)
            blur.DesativarBlur();

        if (pressEIndicator != null)
            pressEIndicator.Hide();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            sequence.SetColliding(true);
            if (pressEIndicator != null && isActive && !isRunning && !panelAberto)
                pressEIndicator.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            sequence.SetColliding(false);
            if (pressEIndicator != null)
                pressEIndicator.Hide();
        }
    }

    public bool IsActive() => isActive;
}