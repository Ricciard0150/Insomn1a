using UnityEngine;

public class TriggerJumpscare : MonoBehaviour
{
    [SerializeField] private Death susto;
    private bool jaAconteceu = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!jaAconteceu && other.CompareTag("Player"))
        {
            jaAconteceu = true;
            if (susto != null) susto.TriggerFlash();
            // Se quiser que o susto possa acontecer de novo depois de 10 seg, é só resetar o jaAconteceu
        }
    }
}