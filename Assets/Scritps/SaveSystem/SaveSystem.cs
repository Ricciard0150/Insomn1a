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

    public void SaveGame(Vector3 pos, string scene)
    {
        currentSave.position = new float[] { pos.x, pos.y, pos.z };
        currentSave.scene = scene;
        currentSave.hasSavedAtLeastOnce = true;
        File.WriteAllText(savePath, JsonUtility.ToJson(currentSave));
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            currentSave = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));
            if (currentSave == null) currentSave = new SaveData();
        }
        else
        {
            currentSave = new SaveData();
        }
    }
    public void Teleport(GameObject player)
    {
        if (player == null)
        {
            return;
        }

        if (currentSave.position == null || currentSave.position.Length < 3)
        {
            return;
        }
        Vector3 pos = new Vector3(
            currentSave.position[0],
            currentSave.position[1],
            currentSave.position[2]
        );

        player.transform.position = pos;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

        TopDownMovement movement = player.GetComponent<TopDownMovement>();
        if (movement != null)
        {
            movement.SetCanMove(true);
        }
    }

    public bool HasSave() => File.Exists(savePath);

    public bool HasSavedAtLeastOnce()
    {
        if (!HasSave()) return false;
        LoadGame();
        return currentSave != null && currentSave.hasSavedAtLeastOnce;
    }

    public SaveData GetData() => currentSave;

    public void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(savePath);
        }
        currentSave = new SaveData();
    }
}

[System.Serializable]
public class SaveData
{
    public float[] position = new float[3];
    public string scene = "";
    public bool hasSavedAtLeastOnce = false;
}