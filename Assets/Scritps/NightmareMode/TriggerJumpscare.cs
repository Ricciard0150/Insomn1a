using UnityEngine;
using System.Collections;

public class TriggerJumpscare : MonoBehaviour
{
    [SerializeField] private Death susto;

    [Header("Player")]
    [Tooltip("Arraste o TopDownMovement do Player aqui")]
    [SerializeField] private TopDownMovement playerMovement;

    [Tooltip("Tempo de espera APÓS a death sumir para destravar o player")]
    [SerializeField] private float tempoAposDeath = 2f;

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

            // ✅ 1. TRAVA O MOVIMENTO IMEDIATAMENTE
            if (playerMovement != null)
            {
                playerMovement.SetCanMove(false);
                playerMovement.canMove = false;
                Debug.Log("[TriggerJumpscare] Player travado.");
            }

            // ✅ 2. INICIA O DESAPARECIMENTO DA DEATH
            if (susto != null)
                susto.IniciarDesaparecimento();
        }
    }

    private void OnDeathDesapareceu()
    {
        StartCoroutine(DestravarAposTempo());
    }

    private IEnumerator DestravarAposTempo()
    {
        // ✅ 3. ESPERA O TEMPO EXTRA (configurável)
        yield return new WaitForSecondsRealtime(tempoAposDeath);

        // ✅ 4. DESTRAVA O PLAYER
        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
            playerMovement.canMove = true;
            Debug.Log("[TriggerJumpscare] Player destravado.");
        }
    }
}