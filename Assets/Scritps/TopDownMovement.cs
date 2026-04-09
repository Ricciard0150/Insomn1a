using UnityEngine;

public class TopDownMovement : MonoBehaviour, ITouchable, IStatusPlayer
{
    [SerializeField] float speed = 10f;
    private Vector2 _movement;
    private Rigidbody2D rb;

    public bool canMove = false;

    [Header("Áudio de Passo")]
    [SerializeField] private AudioSource footstepAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            _movement = Vector2.zero;
            StopFootstep();
            return;
        }

        _movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Verifica se está andando
        if (_movement != Vector2.zero)
        {
            PlayFootstep();
        }
        else
        {
            StopFootstep();
        }
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

    void PlayFootstep()
    {
        if (!footstepAudio.isPlaying)
        {
            footstepAudio.Play();
        }
    }

    void StopFootstep()
    {
        if (footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }

    public void Active()
    {
        throw new System.NotImplementedException();
    }
}