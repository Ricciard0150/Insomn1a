using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private bool loadOnStart = true;

    void Start()
    {
        if (loadOnStart)
        {
            LoadGame();
        }
    }

    void LoadGame()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("❌ SaveSystem não está na cena!");
            return;
        }

        if (!SaveSystem.Instance.HasSavedAtLeastOnce())
        {
            Debug.Log("ℹ️ Nenhum save para carregar");
            return;
        }

        // Recarregar dados
        SaveSystem.Instance.LoadGame();
        SaveData data = SaveSystem.Instance.GetData();

        // Verificar se está na cena certa
        string currentScene = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(data.scene) && currentScene != data.scene)
        {
            Debug.Log($"📂 Carregando cena: {data.scene}");
            SceneManager.LoadScene(data.scene);
            return;
        }

        // ✅ TELEPORTE IMEDIATO (sem delay)
        TeleportPlayer();
    }

    void TeleportPlayer()
    {
        // Procurar player se não foi atribuído
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // ✅ Teleportar instantaneamente
            SaveSystem.Instance.Teleport(player);
            Debug.Log($"📍 Player teleportado IMEDIATAMENTE para: {player.transform.position}");
        }
        else
        {
            Debug.LogWarning("⚠️ Player não encontrado!");
        }
    }

    // ✅ Quando a cena carregar, teleportar IMEDIATAMENTE
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ✅ SEM DELAY - teleporta no mesmo frame
        TeleportPlayer();
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
}