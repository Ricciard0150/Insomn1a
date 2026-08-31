using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoads : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    [Header("Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    private void Start()
    {
        if (newGameButton != null)
            newGameButton.onClick.AddListener(StartNewGame);

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            bool hasSave = SaveSystem.Instance != null && SaveSystem.Instance.HasSave();
            continueButton.interactable = hasSave;

            Text buttonText = continueButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = hasSave ? "Continue" : "New Game";
            }
        }
    }

    public void StartNewGame()
    {
        Debug.Log("🆕 Novo Jogo!");

        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSave(); 
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        Debug.Log("▶️ Continuando Jogo...");

        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSave())
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.Log("Nenhum save encontrado. Iniciando novo jogo...");
            StartNewGame();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnEnable()
    {
        UpdateContinueButton();
    }
}