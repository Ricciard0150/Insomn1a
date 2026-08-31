using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [Header("Configuração")]
    public string pointName = "Ponto de Save";
    public GameObject visualFeedback; 

    private bool isActive = true;
    private GameObject player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IStatusPlayer play))
        {
            player = other.gameObject;
            if (visualFeedback != null)
                visualFeedback.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            if (visualFeedback != null)
                visualFeedback.SetActive(false);
        }
    }

    void Update()
    {
        if (player != null && isActive && Input.GetButtonDown("Interact"))
        {
            SaveGame();
        }
    }

    void SaveGame()
    {
        SaveSystem.Instance.SaveGame(player.transform.position, player.transform.rotation);

        isActive = false;
        if (visualFeedback != null)
            visualFeedback.SetActive(false);

        Debug.Log($"Jogo salvo em: {pointName}");

        Invoke("ReactiveSavePoint", 2f);
    }

    void ReactiveSavePoint()
    {
        isActive = true;
    }
}