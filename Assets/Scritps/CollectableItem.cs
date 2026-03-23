    using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public Transform carId;
    public Transform maoPlayer;
    bool playerPerto = false;
    bool sendoSegurado = false;


    void Update()
    {
        if (playerPerto && Input.GetKeyDown(KeyCode.E) && !sendoSegurado)
        {
            Pegar();
        }

        if (sendoSegurado && Input.GetKeyDown(KeyCode.Q))
        {
            Dropar();
        }
    }

    void Pegar()
    {
        Debug.Log("lalala");
        sendoSegurado = true;
        transform.SetParent(maoPlayer);
        transform.localPosition = Vector3.zero;
    }

    void Dropar()
    {
        sendoSegurado = false;
        transform.SetParent(null);
    }
    void OnMoveCar()
    {
        Debug.Log("gaga");
        transform.SetParent(carId);
        transform.position = carId.position;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerPerto = true;

        }
        if (collision.CompareTag("Car"))
        {
            OnMoveCar();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerPerto = false;
        }
    }
}
