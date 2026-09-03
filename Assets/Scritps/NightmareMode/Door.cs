using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string interactButton = "Interact";
    [SerializeField] private CollectableItem keyItem;
    [SerializeField] private PressEIndicator pressEIndicator;
    [SerializeField] private NightmareLines dialogue;
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

    void Update()
    {
        if (playerNear && Input.GetButtonDown(interactButton))
        {
            Interagir();
        }
    }

    private void Interagir()
    {

        if (!keyItem.HasKey())
        {
            dialogue.StartDialogue(noKeyDialogue);
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
        {
            yield return StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
        }

        dialogue.StartDialogue(hasKeyDialogue);
        yield return new WaitUntil(() => dialogue.IsFinished());

        if (objectahead != null)
        {
            objectahead.SetActive(false);
        }

        if (jumpscareManager != null)
        {
            jumpscareManager.ActivateJumpscare();
        }
        else

        if (pressEIndicator != null)
            pressEIndicator.Hide();

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