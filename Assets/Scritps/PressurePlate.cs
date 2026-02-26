using UnityEngine;
using UnityEngine.Events; 

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool _once;
    TopDownMovement _tdm; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer player))
        {
        }
    }
}
