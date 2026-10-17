using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class JumpscareManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private JumpscareSequence sequence;
    [SerializeField] private BlurController blur;
    [SerializeField] private DialogueNightmare dialogue; // ✅ Trocado para DialogueSystem
    [SerializeField] private GlassPunch punch;
    [SerializeField] private TopDownMovement playerMovement;
    [SerializeField] private Door door;
    [SerializeField] private CollectableItem keyItem;
    [SerializeField] private PressEIndicator pressEIndicator;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private AudioSource asas;

    [Header("Input Settings")]
    [SerializeField] private string interactButton = "Interact";
    [SerializeField] private string blinkButton = "Blink";

    [Header("Settings")]
    [SerializeField] private float delayAfterBlink = 2f;
    [SerializeField] private float delayAfterDialogue = 0.5f;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private string sceneToLoad;

    private bool isRunning = false;
    private bool waitingBlink = false;
    private bool waitingDialogue = false;
    private bool isActive = false;
    private bool panelAberto = false;
    private bool punchActivated = false;

    private InteractionDetector[] npcsNaCena;

    void Start()
    {
        isActive = true;
        npcsNaCena = FindObjectsOfType<InteractionDetector>();
        ResetJumpscare();
    }

    void Update()
    {
        if (waitingBlink && Input.GetButtonDown(blinkButton))
        {
            StartCoroutine(ProcessBlink());
            return;
        }

        if (isRunning) return;

        if (Input.GetButtonDown(interactButton) && CanInteract() && !IsDialogueBlocking())
        {
            StartSequence();
        }
    }

    private bool IsDialogueBlocking()
    {
        if (DialogueNightmare.Instance != null && DialogueNightmare.Instance.DialogoAtivo)
            return true;

        foreach (var npc in npcsNaCena)
            if (npc != null && npc.PlayerPerto)
                return true;

        return false;
    }

    private bool CanInteract()
    {
        return sequence != null && sequence.IsColliding()
            && !isRunning
            && door != null && door.interactedwithKey()
            && keyItem != null && keyItem.HasKey()
            && isActive;
    }

    public void ActivateJumpscare()
    {
        isActive = true;
        if (pressEIndicator != null && sequence != null && sequence.IsColliding())
            pressEIndicator.Show();
    }

    private void StartSequence()
    {
        isRunning = true;
        waitingBlink = true;
        panelAberto = true;
        punchActivated = false;

        if (asas != null) asas.Stop();
        if (playerMovement != null) playerMovement.SetCanMove(false);
        if (blur != null) blur.TurnOnBlur();
        if (sequence != null) sequence.ShowPanel(true);

        if (pressEIndicator != null) pressEIndicator.Hide();

        // ✅ Ajustado para usar o overload do DialogueSystem
        if (dialogue != null)
            dialogue.StartDialogue(new string[] { "Aperte ESPAÇO para piscar..." });
    }

    private IEnumerator ProcessBlink()
    {
        waitingBlink = false;
        if (blur != null) yield return StartCoroutine(blur.PiscarComFade());
        if (blur != null) blur.TurnOffBlur();

        yield return new WaitForSeconds(delayAfterBlink);

        if (dialogue != null)
        {
            dialogue.ForceClose(); // ✅ Em vez de Close()
            dialogue.StartDialogue(sequence.GetDialogueLines());
            waitingDialogue = true;
            yield return new WaitUntil(() => dialogue.IsFinished());
            waitingDialogue = false;
            yield return new WaitForSeconds(delayAfterDialogue);
        }

        if (punch != null)
        {
            if (!punch.IsActive()) punch.ActivateWindow();
            punch.EnablePunch();
            punchActivated = true;
        }
    }

    public void OnGlassBroken()
    {
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (fadeController != null) yield return StartCoroutine(fadeController.FadeToBlack(fadeDuration));
        else yield return new WaitForSeconds(1f);

        if (playerMovement != null) playerMovement.SetCanMove(true);
        isRunning = false;
        panelAberto = false;
        punchActivated = false;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ResetJumpscare()
    {
        isRunning = false;
        waitingBlink = false;
        waitingDialogue = false;
        panelAberto = false;
        punchActivated = false;

        if (playerMovement != null) playerMovement.SetCanMove(true);
        if (sequence != null) sequence.ShowPanel(false);
        if (blur != null) blur.TurnOffBlur();
        if (pressEIndicator != null) pressEIndicator.Hide();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            if (sequence != null) sequence.SetColliding(true);
            if (pressEIndicator != null && isActive && !isRunning && !panelAberto)
                pressEIndicator.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            if (sequence != null) sequence.SetColliding(false);
            if (pressEIndicator != null) pressEIndicator.Hide();
        }
    }

    public bool IsActive() => isActive;
    public bool IsRunning() => isRunning;
}