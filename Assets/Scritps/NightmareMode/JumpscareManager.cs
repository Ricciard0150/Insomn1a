using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class JumpscareManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private JumpscareSequence sequence;
    [SerializeField] private BlurController blur;
    [SerializeField] private NightmareLines dialogue;
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
    void Start()
    {
        isActive = true;
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
        if (Input.GetButtonDown(interactButton) && CanInteract())
        {
            StartSequence();
        }
    }

    private bool CanInteract()
    {
        return sequence.IsColliding() && !isRunning && door.interactedwithKey() && keyItem.HasKey() && isActive;
    }

    public void ActivateJumpscare()
    {
        isActive = true;

        if (pressEIndicator != null && sequence.IsColliding())
            pressEIndicator.Show();

    }

    private void StartSequence()
    {

        isRunning = true;
        waitingBlink = true;
        panelAberto = true;
        punchActivated = false;

        asas.Stop();
        playerMovement.SetCanMove(false);
        blur.TurnOnBlur();
        sequence.ShowPanel(true);

        if (pressEIndicator != null)
            pressEIndicator.Hide();

        dialogue.ShowText("Aperte ESPAÇO para piscar...", false);
    }

    private IEnumerator ProcessBlink()
    {
        waitingBlink = false;
        yield return StartCoroutine(blur.PiscarComFade());
        blur.TurnOffBlur();
        yield return new WaitForSeconds(delayAfterBlink);
        dialogue.Close();
        dialogue.StartDialogue(sequence.GetDialogueLines());
        waitingDialogue = true;
        yield return new WaitUntil(() => dialogue.IsFinished());
        waitingDialogue = false;
        yield return new WaitForSeconds(delayAfterDialogue);

        if (punch != null)
        {
            if (!punch.IsActive())
            {
                punch.ActivateWindow();
            }
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
        if (fadeController != null)
        {
            yield return StartCoroutine(fadeController.FadeToBlack(fadeDuration));
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        playerMovement.SetCanMove(true);
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
        if (playerMovement != null)
            playerMovement.SetCanMove(true);
        if (sequence != null)
            sequence.ShowPanel(false);
        if (blur != null)
            blur.TurnOffBlur();
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
    }    public bool IsActive() => isActive;
}