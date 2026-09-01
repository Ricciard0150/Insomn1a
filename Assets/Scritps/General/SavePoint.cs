using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private string pointName = "Save";
    [SerializeField] private GameObject feedback;

    private bool canSave = true;
    private GameObject player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IStatusPlayer play))
        {
            player = other.gameObject;
            if (feedback != null) feedback.SetActive(true);
            Debug.Log($"📌 {pointName} - Player entrou");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IStatusPlayer play))
        {
            player = null;
            if (feedback != null) feedback.SetActive(false);
        }
    }

    void Update()
    {
        if (player != null && canSave && Input.GetButtonDown("Interact"))
        {
            Salvar();
        }
    }

    void Salvar()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("❌ SaveSystem não encontrado!");
            return;
        }

        // ✅ Salvar - Isso vai marcar hasSavedAtLeastOnce = true
        SaveSystem.Instance.SaveGame(
            player.transform.position,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );

        canSave = false;
        if (feedback != null) feedback.SetActive(false);

        Debug.Log($"💾 SAVE em: {pointName} - PRIMEIRO SAVE REGISTRADO!");

        Invoke(nameof(Reativar), 2f);
    }

    void Reativar()
    {
        canSave = true;
        if (feedback != null) feedback.SetActive(true);
    }
}