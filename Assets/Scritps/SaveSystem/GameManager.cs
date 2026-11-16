using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player References")]
    public TopDownMovement playerMovement;
    public Camera2Dfollowing cameraFollow;
    public BlurController blurController;

    [Header("Spawn Settings")]
    public Vector3 spawnPosition;
    public string spawnSceneName = "Game";

    [Header("State")]
    public bool isGameLocked = false;

    public HashSet<string> completedQuests = new HashSet<string>();
    public Dictionary<string, bool> questResults = new Dictionary<string, bool>();

    private void Awake()
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

    public void LockGame(bool lockState)
    {
        isGameLocked = lockState;

        if (playerMovement != null)
            playerMovement.canMove = !lockState;

        if (cameraFollow != null)
            cameraFollow.canFollow = !lockState;
    }

    public void ToggleBlur(bool active)
    {
        if (blurController == null) return;

        if (active) blurController.TurnOnBlur();
        else blurController.TurnOffBlur();
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
        // 1. Checa na memória
        if (completedQuests.Contains(questId)) return true;

        // 2. Se não achou, checa no arquivo salvo
        if (SaveSystem.Instance != null && SaveSystem.Instance.IsQuestCompleted(questId))
        {
            // Adiciona à memória para agilizar futuras checagens
            completedQuests.Add(questId);
            return true;
        }

        return false;
    }

    public bool GetQuestResult(string questId)
    {
        return questResults.TryGetValue(questId, out bool result) && result;
    }
}