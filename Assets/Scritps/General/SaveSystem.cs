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
            Debug.Log($"✅ SaveSystem inicializado. Save existe: {HasSave()}");
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
        currentSave.hasSavedAtLeastOnce = true;  // ✅ MARCA QUE JÁ SALVOU
        File.WriteAllText(savePath, JsonUtility.ToJson(currentSave));
        Debug.Log($"✅ Save: {pos} | {scene} | Primeiro save: {currentSave.hasSavedAtLeastOnce}");
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            currentSave = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));

            // ✅ SE O SAVE NÃO TIVER A FLAG, MARCA COMO FALSE (COMPATIBILIDADE)
            if (currentSave == null)
            {
                currentSave = new SaveData();
            }

            Debug.Log($"📂 Load: {currentSave.scene} | Já salvou: {currentSave.hasSavedAtLeastOnce}");
        }
        else
        {
            currentSave = new SaveData();
        }
    }

    public void Teleport(GameObject player)
    {
        if (player == null || currentSave.position == null) return;

        Vector3 pos = new Vector3(currentSave.position[0], currentSave.position[1], currentSave.position[2]);
        player.transform.position = pos;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        Debug.Log($"📍 Teleport: {pos}");
    }

    public bool HasSave() => File.Exists(savePath);

    // ✅ NOVO - Verifica se já salvou pelo menos uma vez
    public bool HasSavedAtLeastOnce()
    {
        if (!HasSave()) return false;
        LoadGame(); // Recarregar para garantir dados atuais
        return currentSave != null && currentSave.hasSavedAtLeastOnce;
    }

    public SaveData GetData() => currentSave;

    public void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(savePath);
            Debug.Log("🗑️ Save DELETADO!");
        }
        currentSave = new SaveData();
    }
}

[System.Serializable]
public class SaveData
{
    public float[] position = new float[3];
    public string scene = "";
    public bool hasSavedAtLeastOnce = false;  // ✅ FLAG DE PRIMEIRO SAVE
}