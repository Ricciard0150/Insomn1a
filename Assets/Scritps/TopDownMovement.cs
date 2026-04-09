using UnityEngine;

public class TopDownMovement : MonoBehaviour, ITouchable, IStatusPlayer
{
    [SerializeField] float speed = 10f;
    private Vector2 _movement;
    private Rigidbody2D rb;

    public bool canMove = false; // controla se pode se mover

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            _movement = Vector2.zero;
            return;
        }

        _movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = _movement * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out ITouchable target))
            return;

        target.Active();
    }

    public void Active()
    {
        throw new System.NotImplementedException();
    }
}