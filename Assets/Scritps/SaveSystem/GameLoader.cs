using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private bool loadOnStart = true;

    private bool alreadyTeleported = false;
    private bool sceneLoadRequested = false;

    void Start()
    {
        if (loadOnStart)
            LoadGame();
    }

    void LoadGame()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("[GameLoader] SaveSystem não está na cena");
            return;
        }

        if (!SaveSystem.Instance.HasSavedAtLeastOnce())
        {
            Debug.Log("[GameLoader] Nenhum save para carregar");
            return;
        }

        SaveData data = SaveSystem.Instance.GetData();
        string currentScene = SceneManager.GetActiveScene().name;

        // Se o save é de outra cena, carrega ela
        if (!string.IsNullOrEmpty(data.scene) && currentScene != data.scene)
        {
            if (!sceneLoadRequested)
            {
                sceneLoadRequested = true;
                Debug.Log($"[GameLoader] Carregando cena do save: {data.scene}");
                SceneManager.LoadScene(data.scene);
            }
            return;
        }

        // Mesma cena → teleporta direto
        TeleportPlayer();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoadRequested = false;

        if (SaveSystem.Instance == null) return;

        SaveData data = SaveSystem.Instance.GetData();
        if (string.IsNullOrEmpty(data.scene) || data.scene != scene.name) return;
        if (alreadyTeleported) return;

        StartCoroutine(TeleportNextFrame());
    }

    IEnumerator TeleportNextFrame()
    {
        yield return null; // espera 1 frame pra garantir que o player existe
        TeleportPlayer();
    }

    void TeleportPlayer()
    {
        if (alreadyTeleported) return;
        alreadyTeleported = true;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogWarning("[GameLoader] Player não encontrado");
            return;
        }

        SaveSystem.Instance.Teleport(player);
        Debug.Log($"[GameLoader] Player teleportado para: {player.transform.position}");
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
}