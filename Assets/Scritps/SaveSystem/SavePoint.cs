using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private string pointName = "Save";
    [SerializeField] private GameObject feedback;
    [SerializeField] private SaveNotification saveNotification; 

    private bool canSave = true;
    private GameObject player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IStatusPlayer status))
        {
            player = other.gameObject;
            if (feedback != null) feedback.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IStatusPlayer status))
        {
            player = null;
            if (feedback != null) feedback.SetActive(false);
        }
    }

    void Update()
    {
        if (player != null && canSave && Input.GetButtonDown("Interact"))
        {
            SalvarJogo();
        }
    }

    void SalvarJogo()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("savesystem did not find");
            return;
        }

        SaveSystem.Instance.SaveGame(
            player.transform.position,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );

        if (saveNotification != null)
        {
            saveNotification.Show("saved game");
        }

        canSave = false;
        if (feedback != null) feedback.SetActive(false);

        Debug.Log($"saving in: {pointName} - Pos: {player.transform.position}");

        Invoke(nameof(Reativar), 2f);
    }

    void Reativar()
    {
        canSave = true;
        if (feedback != null) feedback.SetActive(true);
    }
}