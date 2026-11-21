using UnityEngine;
using System.Collections;

public class TriggerJumpscare : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Death susto;

    [Tooltip("Arraste o TopDownMovement do Player aqui")]
    [SerializeField] private TopDownMovement playerMovement;

    [Header("Configurações de Tempo")]
    [Tooltip("Tempo de espera APÓS a death sumir para destravar o player")]
    [SerializeField] private float tempoAposDeath = 2f;

    [Header("Diálogo Pós-Death")]
    [Tooltip("Arraste o DialogueNightmare aqui")]
    [SerializeField] private DialogueNightmare dialogueNightmare;

    [Tooltip("As falas que devem aparecer depois de tudo")]
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

            // 1. Trava o movimento do player IMEDIATAMENTE
            if (playerMovement != null)
            {
                playerMovement.SetCanMove(false);
                playerMovement.canMove = false;
                Debug.Log("[TriggerJumpscare] Player travado.");
            }

            // 2. Inicia o desaparecimento da death
            if (susto != null)
                susto.IniciarDesaparecimento();
        }
    }

    private void OnDeathDesapareceu()
    {
        StartCoroutine(DestravarEDialogar());
    }

    private IEnumerator DestravarEDialogar()
    {
        // 3. Espera o tempo extra (configurável)
        yield return new WaitForSecondsRealtime(tempoAposDeath);

        // 4. Destrava o player
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
            playerMovement.canMove = true;
            Debug.Log("[TriggerJumpscare] Player destravado.");
        }

        // 5. Inicia o diálogo usando o DialogueNightmare
        if (dialogueNightmare != null && falasPosDeath != null && falasPosDeath.Length > 0)
        {
            // O DialogueNightmare já tem a sobrecarga que recebe string[]
            dialogueNightmare.StartDialogue(falasPosDeath);
            Debug.Log("[TriggerJumpscare] Diálogo do Nightmare iniciado.");
        }
    }
}