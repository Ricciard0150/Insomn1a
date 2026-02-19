using UnityEngine;

public class TopDownMovement : MonoBehaviour, ITouchabale
{
    [SerializeField] float speed = 10f;
    private Vector2 _movement;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //botao 
        _movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    private void FixedUpdate()
    {
        //fisica
        rb.linearVelocity = _movement * speed;
    }
    private void LateUpdate()
    {
        //mover a camera 
        //objetos que seguem
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out ITouchabale target))
            return;
        //tryget situa??es q qremos pegar o component mas nao queremos q de erro (tipo bool) e retorna + de um valor (out possibilita criar um novo objeto) 
        target.Active();
        // target.

    }

    public void Active()
    {
        throw new System.NotImplementedException();
    }
}
