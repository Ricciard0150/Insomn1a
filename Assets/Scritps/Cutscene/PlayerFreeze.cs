using UnityEngine;

public class DestroyPlayerScriptTrigger : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Digite o nome exato do script que você quer destruir (ex: TopDownMovement, PlayerJump, etc)")]
    [SerializeField] private string nomeDoScriptParaDestruir = "TopDownMovement";

    [Tooltip("Se marcado, só destrói uma vez (evita erros se o player passar várias vezes)")]
    [SerializeField] private bool destruirApenasUmaVez = true;

    [Tooltip("Opcional: Arraste o Player aqui. Se deixar vazio, ele tenta achar pelo nome.")]
    [SerializeField] private GameObject player;

    private bool jaDestruiu = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se é o player
        if (!collision.TryGetComponent(out IStatusPlayer _)) return;

        // Se já destruiu e a opção for para destruir apenas uma vez, para aqui
        if (destruirApenasUmaVez && jaDestruiu) return;

        // Descobre quem é o player
        GameObject targetPlayer = player != null ? player : collision.gameObject;

        // Procura o script específico no Player
        Component scriptEncontrado = targetPlayer.GetComponent(nomeDoScriptParaDestruir);

        if (scriptEncontrado != null)
        {
            Destroy(scriptEncontrado);
            jaDestruiu = true;
            Debug.Log($"[DestroyScript] O script '{nomeDoScriptParaDestruir}' foi destruído do Player!");
        }
        else
        {
            Debug.LogWarning($"[DestroyScript] O script '{nomeDoScriptParaDestruir}' não foi encontrado no objeto '{targetPlayer.name}'.");
        }
    }
}