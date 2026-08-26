using UnityEngine;

public class ResetDialoguePrefs : MonoBehaviour
{
    [SerializeField] private string dialogueID = "UniqueDialogue";

    void Start()
    {
        string playerPrefKey = $"GlobalDialogue_{dialogueID}";

        if (PlayerPrefs.HasKey(playerPrefKey))
        {
            Debug.Log($"Removendo PlayerPref: {playerPrefKey}");
            PlayerPrefs.DeleteKey(playerPrefKey);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log($"PlayerPref '{playerPrefKey}' não encontrado.");
        }

        Destroy(gameObject);
    }
}
