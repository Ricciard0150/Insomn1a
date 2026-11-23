using UnityEngine;

public class TriggerToggleObjects : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Marque para ativar ao entrar e desativar ao sair. Se desmarcar, vai ativar/desativar apenas uma vez ao entrar.")]
    [SerializeField] private bool toggleOnExit = true;

    [Header("Objetos que Vão ATIVAR")]
    [SerializeField] private GameObject[] objectsToActivate;

    [Header("Objetos que Vão DESATIVAR")]
    [SerializeField] private GameObject[] objectsToDeactivate;

    [Header("Tag do Player")]
    [SerializeField] private string playerTag = "Player";

    private bool jaAtivou = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !jaAtivou)
        {
            jaAtivou = true;

            // Ativa os objetos
            if (objectsToActivate != null)
            {
                foreach (GameObject obj in objectsToActivate)
                {
                    if (obj != null) obj.SetActive(true);
                }
            }

            // Desativa os objetos
            if (objectsToDeactivate != null)
            {
                foreach (GameObject obj in objectsToDeactivate)
                {
                    if (obj != null) obj.SetActive(false);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && toggleOnExit)
        {
            jaAtivou = false;

            // Reverte os objetos (Desativa os que ativou)
            if (objectsToActivate != null)
            {
                foreach (GameObject obj in objectsToActivate)
                {
                    if (obj != null) obj.SetActive(false);
                }
            }

            // Reverte os objetos (Ativa os que desativou)
            if (objectsToDeactivate != null)
            {
                foreach (GameObject obj in objectsToDeactivate)
                {
                    if (obj != null) obj.SetActive(true);
                }
            }
        }
    }
}