using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Vector3 spawnPosition;
    public string spawnSceneName = "Game";

    public HashSet<string> completedQuests = new HashSet<string>();
    public Dictionary<string, bool> questResults = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSpawnPoint(Vector3 position, string sceneName)
    {
        spawnPosition = position;
        spawnSceneName = sceneName;
    }

    public void SaveQuestResult(string questId, bool victory, int points)
    {
        if (!questResults.ContainsKey(questId))
            questResults.Add(questId, victory);
        else
            questResults[questId] = victory;

        if (victory && !completedQuests.Contains(questId))
            completedQuests.Add(questId);
    }

    public void CompleteQuest(string questId)
    {
        if (!completedQuests.Contains(questId))
            completedQuests.Add(questId);
    }

    public bool IsQuestCompleted(string questId)
    {
        return completedQuests.Contains(questId);
    }

    public bool GetQuestResult(string questId)
    {
        return questResults.TryGetValue(questId, out bool result) && result;
    }
}