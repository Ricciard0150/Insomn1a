using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string interactButton = "Interact";
    [SerializeField] private CollectableItem keyItem;
    [SerializeField] private PressEIndicator pressEIndicator;
    [SerializeField] private DialogueNightmare dialogueSystem;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private GameObject objectahead; 
    [SerializeField] private JumpscareManager jumpscareManager;

    [Header("Dialogues")]
    [SerializeField] private string[] noKeyDialogue = { "Preciso de uma chave..." };
    [SerializeField] private string[] hasKeyDialogue = { "Tem alguém batendo na janela!" };

    [Header("Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.3f;

    private bool interactedWithKey = false;
    private bool playerNear = false;
    private InteractionDetector[] npcsNaCena;

    void Start()
    {
        npcsNaCena = FindObjectsOfType<InteractionDetector>();
    }

    void Update()
    {
        if (!playerNear || interactedWithKey) return;
        if (jumpscareManager != null && jumpscareManager.IsRunning()) return;
        if (IsDialogueBlocking()) return;

        if (Input.GetButtonDown(interactButton))
        {
            Interagir();
        }
    }

    private bool IsDialogueBlocking()
    {
        if (dialogueSystem != null && dialogueSystem.DialogoAtivo)
            return true;

        foreach (var npc in npcsNaCena)
            if (npc != null && npc.PlayerPerto)
                return true;

        return false;
    }

    private void Interagir()
    {
        if (!keyItem.HasKey())
        {
            dialogueSystem.StartDialogue(noKeyDialogue);
            return;
        }

        if (!interactedWithKey)
        {
            interactedWithKey = true;
            StartCoroutine(ProcessarComChave());
        }
    }

    private IEnumerator ProcessarComChave()
    {
        if (cameraShake != null)
            yield return StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));

        dialogueSystem.StartDialogue(hasKeyDialogue);
        yield return new WaitUntil(() => dialogueSystem.IsFinished());

        if (dialogueSystem.playerMovement != null)
            dialogueSystem.playerMovement.canMove = true;

        if (objectahead != null)
            objectahead.SetActive(false);

        if (pressEIndicator != null)
            pressEIndicator.Hide();

        if (jumpscareManager != null)
            jumpscareManager.ActivateJumpscare();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = true;
            if (pressEIndicator != null && !interactedWithKey)
                pressEIndicator.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = false;
            if (pressEIndicator != null)
                pressEIndicator.Hide();
        }
    }

    public bool interactedwithKey() => interactedWithKey;
}