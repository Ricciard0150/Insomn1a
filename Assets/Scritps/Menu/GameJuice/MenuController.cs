using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    void Start()
    {
        // Show only the main menu
        if (startPanel != null) startPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    // ===== BUTTON METHODS =====

    public void Play()
    {
        Debug.Log("▶️ Starting game...");
        // Load the game scene
        // SceneManager.LoadScene("GameScene");
    }

    public void OpenOptions()
    {
        Debug.Log("⚙️ Opening options...");
        if (startPanel != null) startPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        Debug.Log("📜 Opening credits...");
        if (startPanel != null) startPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        Debug.Log("🔙 Back to menu...");
        if (startPanel != null) startPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("🚪 Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}