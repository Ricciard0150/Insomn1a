using UnityEngine;

public class Door : MonoBehaviour
{
    public KeyItem key;
    public KeyCode tecla = KeyCode.E;

    private bool playerNear = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(tecla))
        {
            if (key.playerHasKey)
            {
                AbrirPorta();
            }
            else
            {
                Debug.Log("Precisa de uma chave!");
            }
        }
    }

    void AbrirPorta()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
            playerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
            playerNear = false;
        }
    }
}
