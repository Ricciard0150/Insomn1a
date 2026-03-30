using UnityEngine;

public class NPC_RPGTrigger : MonoBehaviour
{
    public BattleRPG rpgSystem;
    public KeyCode teclaInteragir = KeyCode.E;

    private bool playerPerto = false;

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(teclaInteragir) && !rpgSystem.estaAtivo)
        {
            rpgSystem.IniciarSistema();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IStatusPlayer>() != null)
        {
            playerPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IStatusPlayer>() != null)
        {
            playerPerto = false;
        }
    }
}
