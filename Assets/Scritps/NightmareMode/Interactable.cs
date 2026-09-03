using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;
    public GameObject pressEIndicator;

    protected bool isPlayerNear = false;

    protected virtual void Start()
    {
        if (pressEIndicator != null)
            pressEIndicator.SetActive(false);
    }

    protected virtual void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(interactionKey))
            OnInteract();
    }

    protected virtual void OnInteract() { }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
            isPlayerNear = true;
            if (pressEIndicator != null)
                pressEIndicator.SetActive(true);
            OnPlayerEnter();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
            isPlayerNear = false;
            if (pressEIndicator != null)
                pressEIndicator.SetActive(false);
            OnPlayerExit();
        }
    }

    protected virtual void OnPlayerEnter() { }
    protected virtual void OnPlayerExit() { }
}