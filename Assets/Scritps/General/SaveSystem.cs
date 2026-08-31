using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private string savePath;
    private SaveData currentSave;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "save.json");
            currentSave = new SaveData();
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(Vector3 position, Quaternion rotation)
    {
        currentSave.position = new float[] { position.x, position.y, position.z };
        currentSave.rotation = new float[] { rotation.x, rotation.y, rotation.z, rotation.w };
        currentSave.scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(currentSave);
        File.WriteAllText(savePath, json);
        Debug.Log($"Jogo salvo na cena: {currentSave.scene}");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSave = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Jogo carregado da cena: {currentSave.scene}");
        }
    }

    public void TeleportPlayer(GameObject player)
    {
        if (currentSave == null || player == null) return;

        Vector3 pos = new Vector3(currentSave.position[0], currentSave.position[1], currentSave.position[2]);
        Quaternion rot = new Quaternion(currentSave.rotation[0], currentSave.rotation[1], currentSave.rotation[2], currentSave.rotation[3]);

        player.transform.position = pos;
        player.transform.rotation = rot;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    public bool HasSave()
    {
        return File.Exists(savePath);
    }

    public SaveData GetSaveData()
    {
        return currentSave;
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            currentSave = new SaveData(); 
            Debug.Log(" Save deletado com sucesso!");
        }
        else
        {
            Debug.Log(" Nenhum save para deletar.");
        }
    }

    public bool IsInitialDialogueShown()
    {
        return currentSave.initialDialogueShown;
    }

    public void SetInitialDialogueShown(bool shown)
    {
        currentSave.initialDialogueShown = shown;
        string json = JsonUtility.ToJson(currentSave);
        File.WriteAllText(savePath, json);
    }
}

[System.Serializable]
public class SaveData
{
    public float[] position = new float[3];
    public float[] rotation = new float[4];
    public string scene = "";
    public bool initialDialogueShown = false;
}