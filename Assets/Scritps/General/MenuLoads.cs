using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoads : MonoBehaviour
{
    [Header("Botões")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button quitButton;

    [Header("Config")]
    [SerializeField] private string gameSceneName = "GameScene";

    void Start()
    {
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
        Debug.Log("🆕 NOVO JOGO");

        if (SaveSystem.Instance != null)
            SaveSystem.Instance.DeleteSave();

        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        Debug.Log("▶️ CONTINUAR JOGO");

        if (SaveSystem.Instance == null)
        {
            Debug.LogError("❌ SaveSystem não encontrado!");
            return;
        }

        if (!SaveSystem.Instance.HasSavedAtLeastOnce())
        {
            Debug.Log("⚠️ Nenhum save. Use New Game!");
            return;
        }

        // ✅ Carregar dados e cena IMEDIATAMENTE
        SaveSystem.Instance.LoadGame();
        SaveData data = SaveSystem.Instance.GetData();

        string sceneToLoad = !string.IsNullOrEmpty(data.scene) ? data.scene : gameSceneName;
        Debug.Log($"📂 Carregando cena: {sceneToLoad}");

        // ✅ Carregar cena (GameLoader vai teleportar)
        SceneManager.LoadScene(sceneToLoad);
    }

    void ResetSave()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSave();
            Debug.Log("🗑️ SAVE RESETADO!");
            UpdateButtons();
        }
    }

    public void QuitGame()
    {
        Debug.Log("👋 Sair");
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