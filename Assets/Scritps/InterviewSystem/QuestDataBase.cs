using System.Collections.Generic;
using UnityEngine;

public class QuestDatabase : MonoBehaviour
{
    [Header("Quests")]
    public QuestData[] quests;

    private Dictionary<string, QuestData> questDictionary = new Dictionary<string, QuestData>();
    private bool isInitialized = false;

    void Awake()
    {
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (isInitialized) return;

        questDictionary.Clear();

        if (quests != null)
        {
            foreach (var quest in quests)
            {
                if (quest != null && !questDictionary.ContainsKey(quest.questId))
                {
                    questDictionary.Add(quest.questId, quest);
                }
                else if (quest != null)
                {
                    Debug.LogWarning($"[QuestDatabase] ID de quest duplicado encontrado: '{quest.questId}'");
                }
            }
        }
        isInitialized = true;
    }

    public QuestData GetQuest(string questId)
    {
        // Garante que está inicializado antes de buscar
        if (!isInitialized) InitializeDatabase();

        if (string.IsNullOrEmpty(questId)) return null;

        questDictionary.TryGetValue(questId, out QuestData quest);

        if (quest == null)
        {
            Debug.LogError($"[QuestDatabase] Quest '{questId}' não encontrada no banco de dados!");
        }

        return quest;
    }

    public QuestData[] GetAllQuests()
    {
        return quests;
    }
}