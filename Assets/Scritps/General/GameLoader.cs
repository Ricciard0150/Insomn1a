using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField] private GameObject player;     
    public bool load = true;

    void Start()
    {
        if (load && SaveSystem.Instance.HasSave())
        {
            LoadGame();
        }
    }

    public void LoadGame()
    {
        if (player != null && SaveSystem.Instance.HasSave())
        {
            SaveSystem.Instance.TeleportPlayer(player);
            Debug.Log("Jogo carregado!");
        }
        else
        {
            Debug.LogWarning("Player não atribuído no GameLoader!");
        }
    }

    public void LoadButton()
    {
        LoadGame();
    }
}