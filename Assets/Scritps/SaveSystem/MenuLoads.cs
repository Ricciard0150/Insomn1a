using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoads : MonoBehaviour
{
    [Header("buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button quitButton;

    [Header("config")]
    [SerializeField] private string gameSceneName = "Game";

    void Start()
    {
        var tm = TextManager.Instance;

        Debug.Log("todos os sistemas apareceram");
        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(StartNewGame);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(ContinueGame);
        }

        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(ResetSave);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }

        Invoke(nameof(UpdateButtons), 0.1f);
    }

    void UpdateButtons()
    {
        if (continueButton != null)
        {
            bool canContinue = SaveSystem.Instance != null && SaveSystem.Instance.HasSavedAtLeastOnce();
            continueButton.interactable = canContinue;

            Text btnText = continueButton.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.text = canContinue ? "Continue" : "Continue (Salve primeiro)";
                btnText.color = canContinue ? Color.white : Color.gray;
            }
        }

        if (resetButton != null)
        {
            resetButton.interactable = SaveSystem.Instance != null && SaveSystem.Instance.HasSave();
        }
    }

    public void StartNewGame()
    {
        Debug.Log("new game");

        if (SaveSystem.Instance != null)
            SaveSystem.Instance.DeleteSave();

        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        Debug.Log("continue game");

        if (SaveSystem.Instance == null)
        {
            Debug.LogError("savesystem nao apareceu");
            return;
        }

        if (!SaveSystem.Instance.HasSavedAtLeastOnce())
        {
            Debug.Log("⚠️ Nenhum save. Use New Game!");
            return;
        }

        SaveSystem.Instance.LoadGame();
        SaveData data = SaveSystem.Instance.GetData();

        string sceneToLoad = !string.IsNullOrEmpty(data.scene) ? data.scene : gameSceneName;
        Debug.Log($"loading in : {sceneToLoad}");

        SceneManager.LoadScene(sceneToLoad);
    }

    void ResetSave()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSave();
            Debug.Log("save reset");
            UpdateButtons();
        }
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void OnEnable()
    {
        UpdateButtons();
    }
}