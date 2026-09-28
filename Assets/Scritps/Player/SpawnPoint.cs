using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Tooltip("Deve ser igual ao 'idDoSpawn' do Portal que leva até aqui")]
    public string idDoSpawn;

    private void Start()
    {
        if (SpawnManager.proximoID != idDoSpawn) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning($"SpawnPoint '{idDoSpawn}' não achou o Player.");
            return;
        }

        player.transform.position = transform.position;

        player.transform.rotation = transform.rotation;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = transform.position;
            cc.enabled = true;
        }

        SpawnManager.proximoID = null;
    }
}