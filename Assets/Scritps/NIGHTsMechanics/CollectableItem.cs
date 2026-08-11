using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [SerializeField] private string interactButton = "Interact";
    [SerializeField] private PressEIndicator pressEIndicator;

    private bool playerHasKey = false;
    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetButtonDown(interactButton))
        {
            playerHasKey = true;
            gameObject.SetActive(false);

            if (pressEIndicator != null)
                pressEIndicator.Hide();

            Debug.Log("🗝️ CHAVE PEGA! HasKey=true");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = true;
            if (pressEIndicator != null)
                pressEIndicator.Show();
            Debug.Log("🗝️ Player perto da chave!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = false;
            if (pressEIndicator != null)
                pressEIndicator.Hide();
        }
    }

    public bool HasKey() => playerHasKey;
}