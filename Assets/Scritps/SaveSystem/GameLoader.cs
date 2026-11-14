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
        if (SaveSystem.Instance == null) return;
        if (!SaveSystem.Instance.HasSavedAtLeastOnce()) return;

        SaveData data = SaveSystem.Instance.GetData();
        string currentScene = SceneManager.GetActiveScene().name;

        // ✅ Compara com Trim() pra evitar espaço
        if (!string.IsNullOrEmpty(data.scene) && data.scene.Trim() != currentScene.Trim())
        {
            Debug.Log($"[GameLoader] Recarregando: '{data.scene}' != '{currentScene}'");
            SceneManager.LoadScene(data.scene.Trim());
            return;
        }

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