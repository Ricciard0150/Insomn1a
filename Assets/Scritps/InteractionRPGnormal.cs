using UnityEngine;

public class InteractionRPGnormal : MonoBehaviour
{
    public SystemRPGnormal sistemaRPG; // arrasta no Inspector
    private bool playerPerto = false;
    public GameObject pressE;

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(KeyCode.E))
        {
            sistemaRPG.IniciarSistema();
            pressE.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerPerto = true;
            Debug.Log("Aperte E para interagir");
                pressE.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IStatusPlayer status))
        {
            playerPerto = false;
            pressE.SetActive(false);
        }
    }
}