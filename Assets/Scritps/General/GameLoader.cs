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
            Invoke(nameof(LoadGame), 0.3f);
        }
    }

    void LoadGame()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("❌ SaveSystem não está na cena!");
            return;
        }

        // ✅ Só carrega se já salvou pelo menos uma vez
        if (!SaveSystem.Instance.HasSavedAtLeastOnce())
        {
            Debug.Log("ℹ️ Nenhum save para carregar (primeiro save ainda não feito)");
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

        // Teleportar player
        TeleportPlayer();
    }

    void TeleportPlayer()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            SaveSystem.Instance.Teleport(player);
        }
        else
        {
            Debug.LogWarning("⚠️ Player não encontrado!");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Teleportar depois que a cena carregar
        Invoke(nameof(TeleportPlayer), 0.3f);
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
}