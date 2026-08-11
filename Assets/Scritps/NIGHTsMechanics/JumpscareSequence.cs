using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class JumpscareSequence : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject jumpscareImage;

    [Header("Dialogue")]
    [SerializeField] private string[] dialogueLines;

    [Header("Fade")]
    [SerializeField] private FadeController fadeController;

    [Header("Respawn")]
    [SerializeField] private Vector3 respawnPosition;

    [Header("Scene")]
    [SerializeField] private string sceneToLoad = "Game";

    private bool isColliding = false; // ✅ NOVO
    private System.Action onComplete;

    public void ShowPanel(bool show)
    {
        if (panel != null)
            panel.SetActive(show);
    }

    public void StartJumpscare(System.Action callback)
    {
        onComplete = callback;
        StartCoroutine(JumpscareRoutine());
    }

    private IEnumerator JumpscareRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (jumpscareImage != null)
            jumpscareImage.SetActive(true);

        yield return StartCoroutine(fadeController.FlashScreen(6));
        yield return StartCoroutine(fadeController.FadeToBlack(1f));

        SaveRespawnPosition();
        SceneManager.LoadScene(sceneToLoad);

        onComplete?.Invoke();
    }

    private void SaveRespawnPosition()
    {
        PlayerPrefs.SetFloat("ReturnPosX", respawnPosition.x);
        PlayerPrefs.SetFloat("ReturnPosY", respawnPosition.y);
        PlayerPrefs.SetFloat("ReturnPosZ", respawnPosition.z);
        PlayerPrefs.SetString("ReturnScene", sceneToLoad);
        PlayerPrefs.Save();
    }

    // ✅ NOVOS GETTERS/SETTERS
    public bool IsColliding() => isColliding;
    public void SetColliding(bool value) => isColliding = value;
    public string[] GetDialogueLines() => dialogueLines;
}