using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string interactButton = "Interact";
    [SerializeField] private CollectableItem keyItem;
    [SerializeField] private PressEIndicator pressEIndicator;
    [SerializeField] private DM dialogue;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private GameObject objetoNaFrente; // OBJETO NA FRENTE DA WINDOW
    [SerializeField] private JumpscareManager jumpscareManager;

    [Header("Dialogues")]
    [SerializeField] private string[] noKeyDialogue = { "Preciso de uma chave..." };
    [SerializeField] private string[] hasKeyDialogue = { "Tem alguém batendo na janela!" };

    [Header("Settings")]
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.3f;

    private bool jaInteragiuComChave = false;
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
        Debug.Log($"🚪 INTERAGIR! HasKey={keyItem.HasKey()}, jaInteragiu={jaInteragiuComChave}");

        if (!keyItem.HasKey())
        {
            dialogue.StartDialogue(noKeyDialogue);
            return;
        }

        if (!jaInteragiuComChave)
        {
            jaInteragiuComChave = true;
            StartCoroutine(ProcessarComChave());
        }
    }

    private IEnumerator ProcessarComChave()
    {
        Debug.Log("🎬 ProcessarComChave() INICIADO!");

        if (cameraShake != null)
        {
            yield return StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
        }

        dialogue.StartDialogue(hasKeyDialogue);
        yield return new WaitUntil(() => dialogue.IsFinished());

        // ✅ DESATIVA O OBJETO NA FRENTE DA WINDOW
        if (objetoNaFrente != null)
        {
            Debug.Log($"🎬 Desativando objeto na frente: {objetoNaFrente.name}");
            objetoNaFrente.SetActive(false);
            Debug.Log($"✅ Objeto na frente desativado!");
        }

        // ✅ ATIVA O JUMPScare MANAGER (ELE VAI ATIVAR A WINDOW E O SOCO NO MOMENTO CERTO)
        if (jumpscareManager != null)
        {
            jumpscareManager.ActivateJumpscare();
            Debug.Log("✅ JumpscareManager ATIVADO!");
        }
        else
        {
            Debug.LogError("❌ jumpscareManager é NULL! Verifique a referência na Door.");
        }

        if (pressEIndicator != null)
            pressEIndicator.Hide();

        Debug.Log("🎬 ProcessarComChave() FINALIZADO!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = true;
            if (pressEIndicator != null && !jaInteragiuComChave)
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

    public bool JaInteragiuComChave() => jaInteragiuComChave;
}