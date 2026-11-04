using UnityEngine;

public class RespawnInGame : MonoBehaviour
{
    private static Vector3? pendingPosition = null;
    private static RespawnInGame instance;

    /// <summary>
    /// Define a posição onde o player vai nascer na próxima cena.
    /// Chame ANTES de SceneManager.LoadScene.
    /// </summary>
    public static void SetSpawnPosition(Vector3 position)
    {
        pendingPosition = position;
    }

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Chamado pelo TopDownMovement no Start.
    /// Se houver posição pendente, teleporta o player.
    /// </summary>
    public static void RegisterPlayer(TopDownMovement player)
    {
        if (instance == null)
        {
            Debug.LogWarning("[SpawnPoint] Nenhum SpawnPoint na cena para registrar o player.");
            return;
        }

        if (!pendingPosition.HasValue)
            return;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        player.transform.position = pendingPosition.Value;
        pendingPosition = null;

        Debug.Log($"[SpawnPoint] Player teleportado para {player.transform.position}");
    }
}