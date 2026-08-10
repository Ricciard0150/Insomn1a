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
        if (blurController == null)
            return;

        if (active)
            blurController.AtivarBlur();
        else
            blurController.DesativarBlur();
    }

    public void RespawnPlayer()
    {
        SetSpawnPoint(spawnPosition, spawnSceneName);

        PlayerPrefs.SetFloat("ReturnPosX", spawnPosition.x);
        PlayerPrefs.SetFloat("ReturnPosY", spawnPosition.y);
        PlayerPrefs.SetFloat("ReturnPosZ", spawnPosition.z);
        PlayerPrefs.SetString("ReturnScene", spawnSceneName);
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(spawnSceneName);
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