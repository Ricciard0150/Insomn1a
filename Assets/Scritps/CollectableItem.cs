using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public bool playerHasKey = false;
    public KeyCode tecla = KeyCode.E;

    private bool playerNear = false;
    public GameObject pressE;

    private void Start()
    {
        pressE.SetActive(false);
    }
    void Update()
    {
        if (playerNear && Input.GetKeyDown(tecla))
        {
            playerHasKey = true;
            gameObject.SetActive(false); // pega a chave
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
            playerNear = true;
            pressE.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
            playerNear = false;
            pressE.SetActive(false);
        }
    }
}