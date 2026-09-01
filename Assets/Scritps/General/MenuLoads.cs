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
    [SerializeField] private GameObject confirmPanel;

    void Start()
    {
        // Configurar New Game
        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveAllListeners();
            newGameButton.onClick.AddListener(StartNewGame);
        }

        // Configurar Continue
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(ContinueGame);
        }

        // Configurar Reset
        if (resetButton != null)
        {
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(ShowResetConfirm);
        }

        // Configurar Quit
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }

        // Atualizar botões após inicialização
        Invoke(nameof(UpdateButtons), 0.1f);
    }

    void UpdateButtons()
    {
        if (continueButton != null)
        {
            // ✅ Continue SÓ funciona se já salvou pelo menos uma vez
            bool canContinue = SaveSystem.Instance != null && SaveSystem.Instance.HasSavedAtLeastOnce();
            continueButton.interactable = canContinue;

            // Mudar texto do botão (opcional)
            Text btnText = continueButton.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.text = canContinue ? "Continue" : "Continue (Salve primeiro)";
                btnText.color = canContinue ? Color.white : Color.gray;
            }

            Debug.Log($"Continue Button: {(canContinue ? "✅ ATIVADO" : "❌ DESATIVADO - Sem save")}");
        }

        if (resetButton != null)
        {
            bool hasSave = SaveSystem.Instance != null && SaveSystem.Instance.HasSave();
            resetButton.interactable = hasSave;
        }
    }

    // ✅ NEW GAME - Começa do zero
    public void StartNewGame()
    {
        Debug.Log("🆕 NOVO JOGO - Começando do zero!");

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSave();  // Deleta qualquer save existente
        }

        // Carregar cena do jogo
        SceneManager.LoadScene(gameSceneName);
    }

    // ✅ CONTINUE - Só funciona se já salvou
    public void ContinueGame()
    {
        Debug.Log("▶️ CONTINUAR JOGO");

        if (SaveSystem.Instance == null)
        {
            Debug.LogError("❌ SaveSystem não encontrado!");
            return;
        }

        // ✅ Verifica se já salvou pelo menos uma vez
        if (!SaveSystem.Instance.HasSavedAtLeastOnce())
        {
            Debug.Log("⚠️ Nenhum save encontrado. Use New Game!");
            return;
        }

        // Carregar dados
        SaveSystem.Instance.LoadGame();
        SaveData data = SaveSystem.Instance.GetData();

        // Carregar cena salva
        string sceneToLoad = !string.IsNullOrEmpty(data.scene) ? data.scene : gameSceneName;
        Debug.Log($"📂 Carregando cena: {sceneToLoad}");

        SceneManager.LoadScene(sceneToLoad);
    }

    // Resetar save
    void ShowResetConfirm()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(true);
        else
            ResetSave();
    }

    public void ConfirmReset()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
        ResetSave();
    }

    public void CancelReset()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
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