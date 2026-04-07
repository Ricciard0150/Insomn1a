using UnityEngine;

public class ObjectSet : MonoBehaviour
{
    [SerializeField] public GameObject _obj;
    public KeyCode teclaInteragir = KeyCode.E;

    private bool isColliding = false;
    private bool isOpen = false;

    private TopDownMovement tdm;

    void Start()
    {
        if (_obj != null)
            _obj.SetActive(false);

        tdm = FindAnyObjectByType<TopDownMovement>();
    }

    void Update()
    {
        if (isColliding && Input.GetKeyDown(teclaInteragir))
        {
            isOpen = !isOpen; 

            _obj.SetActive(isOpen);

            if (tdm != null)
                tdm.canMove = !isOpen; 
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            isColliding = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            isColliding = false;
        }
    }
}
