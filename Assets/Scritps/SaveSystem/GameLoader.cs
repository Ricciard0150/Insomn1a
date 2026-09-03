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
            Debug.LogError("savesystem is not in scene");
            return;
        }

        if (!SaveSystem.Instance.HasSavedAtLeastOnce())
        {
            Debug.Log("aint no save to load");
            return;
        }

        SaveSystem.Instance.LoadGame();
        SaveData data = SaveSystem.Instance.GetData();

        string currentScene = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(data.scene) && currentScene != data.scene)
        {
            Debug.Log($"loading in: {data.scene}");
            SceneManager.LoadScene(data.scene);
            return;
        }

        TeleportPlayer();
    }

    void TeleportPlayer()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            SaveSystem.Instance.Teleport(player);
            Debug.Log($"player just teleported to: {player.transform.position}");
        }
        else
        {
            Debug.LogWarning("player did not found");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TeleportPlayer();
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
}