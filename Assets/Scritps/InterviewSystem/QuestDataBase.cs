using System.Collections.Generic;
using UnityEngine;

public class QuestDatabase : MonoBehaviour
{
    [Header("Quests")]
    public QuestData[] quests;

    private Dictionary<string, QuestData> questDictionary = new Dictionary<string, QuestData>();

    void Awake()
    {
        foreach (var quest in quests)
        {
            if (!questDictionary.ContainsKey(quest.questId))
                questDictionary.Add(quest.questId, quest);
        }
    }

    public QuestData GetQuest(string questId)
    {
        questDictionary.TryGetValue(questId, out QuestData quest);
        return quest;
    }

    public QuestData[] GetAllQuests()
    {
        return quests;
    }
}
