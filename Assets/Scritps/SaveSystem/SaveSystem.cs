using UnityEngine;
using System.IO;
using System.Collections.Generic;

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
        SaveCurrentState();
    }

    public void SaveCurrentState()
    {
        File.WriteAllText(savePath, JsonUtility.ToJson(currentSave));
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            currentSave = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));
            if (currentSave == null) currentSave = new SaveData();

            // Garante que as listas/arrays não sejam nulos ao carregar saves antigos
            if (currentSave.letterPieces == null || currentSave.letterPieces.Length != 3)
                currentSave.letterPieces = new bool[3];

            if (currentSave.collectedItems == null)
                currentSave.collectedItems = new List<string>();

            if (currentSave.completedQuests == null)
                currentSave.completedQuests = new List<string>();
        }
        else
        {
            currentSave = new SaveData();
        }
    }

    public void Teleport(GameObject player)
    {
        if (player == null) return;
        if (currentSave.position == null || currentSave.position.Length < 3) return;

        Vector3 pos = new Vector3(currentSave.position[0], currentSave.position[1], currentSave.position[2]);
        player.transform.position = pos;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

        TopDownMovement movement = player.GetComponent<TopDownMovement>();
        if (movement != null) movement.SetCanMove(true);
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
        if (HasSave()) File.Delete(savePath);
        currentSave = new SaveData();
    }

    // ---------- PEDAÇOS DE CARTA ----------
    public bool HasLetterPiece(int pieceNumber)
    {
        int index = pieceNumber - 1;
        if (index < 0 || index >= currentSave.letterPieces.Length) return false;
        return currentSave.letterPieces[index];
    }

    public void CollectLetterPiece(int pieceNumber)
    {
        int index = pieceNumber - 1;
        if (index < 0 || index >= currentSave.letterPieces.Length) return;
        currentSave.letterPieces[index] = true;
        SaveCurrentState();
    }

    public int GetCollectedLetterPieceCount()
    {
        int count = 0;
        foreach (bool b in currentSave.letterPieces) if (b) count++;
        return count;
    }

    public int GetTotalLetterPieces() => currentSave.letterPieces.Length;

    // ---------- ITENS COLETÁVEIS ----------
    public bool HasItem(string itemId)
    {
        if (currentSave.collectedItems == null) return false;
        return currentSave.collectedItems.Contains(itemId);
    }

    public void CollectItem(string itemId)
    {
        if (currentSave.collectedItems == null) currentSave.collectedItems = new List<string>();
        if (!currentSave.collectedItems.Contains(itemId))
        {
            currentSave.collectedItems.Add(itemId);
            SaveCurrentState();
        }
    }

    // ---------- QUESTS CONCLUÍDAS ----------
    public bool IsQuestCompleted(string questId)
    {
        if (currentSave.completedQuests == null) return false;
        return currentSave.completedQuests.Contains(questId);
    }

    public void CompleteQuest(string questId)
    {
        if (currentSave.completedQuests == null)
            currentSave.completedQuests = new List<string>();

        if (!currentSave.completedQuests.Contains(questId))
        {
            currentSave.completedQuests.Add(questId);
            SaveCurrentState();
        }
    }
}

[System.Serializable]
public class SaveData
{
    public float[] position = new float[3];
    public string scene = "";
    public bool hasSavedAtLeastOnce = false;

    public bool[] letterPieces = new bool[3];
    public List<string> collectedItems = new List<string>();
    public List<string> completedQuests = new List<string>();
}