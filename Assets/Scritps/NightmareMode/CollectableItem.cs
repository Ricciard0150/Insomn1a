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

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            playerNear = true;
            if (pressEIndicator != null)
                pressEIndicator.Show();
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