using UnityEngine;
using System.Collections;

public class TriggerJumpscare : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Death susto;
    [SerializeField] private TopDownMovement playerMovement;

    [Header("Áudio de Desaparecimento")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip somDesaparecer;

    [Header("Configurações de Tempo")]
    [SerializeField] private float tempoAposDeath = 2f;

    [Header("Diálogo Pós-Death")]
    [SerializeField] private DialogueNightmare dialogueNightmare;
    [TextArea(2, 5)]
    [SerializeField] private string[] falasPosDeath;

    private bool jaAconteceu = false;

    void Start()
    {
        if (susto != null)
            susto.OnDeathDesapareceu += OnDeathDesapareceu;
    }

    void OnDestroy()
    {
        if (susto != null)
            susto.OnDeathDesapareceu -= OnDeathDesapareceu;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!jaAconteceu && other.CompareTag("Player"))
        {
            jaAconteceu = true;

            // ✅ 1. Pausa a música de fundo imediatamente
            if (AudioManager.Instance != null)
                AudioManager.Instance.PausarMusica();

            if (playerMovement != null)
            {
                playerMovement.SetCanMove(false);
                playerMovement.canMove = false;
                Debug.Log("[TriggerJumpscare] Player travado.");
            }

            if (susto != null)
                susto.IniciarDesaparecimento();
        }
    }

    private void OnDeathDesapareceu()
    {
        if (audioSource != null && somDesaparecer != null)
            audioSource.PlayOneShot(somDesaparecer);

        StartCoroutine(DestravarEDialogar());
    }

    private IEnumerator DestravarEDialogar()
    {
        yield return new WaitForSecondsRealtime(tempoAposDeath);

        // ✅ 2. Destrava o player
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
            playerMovement.canMove = true;
            Debug.Log("[TriggerJumpscare] Player destravado.");
        }

        // ✅ 3. Volta a música de fundo
        if (AudioManager.Instance != null)
            AudioManager.Instance.RetomarMusica();

        if (dialogueNightmare != null && falasPosDeath != null && falasPosDeath.Length > 0)
        {
            dialogueNightmare.StartDialogue(falasPosDeath);
            Debug.Log("[TriggerJumpscare] Diálogo do Nightmare iniciado.");
        }
    }
}