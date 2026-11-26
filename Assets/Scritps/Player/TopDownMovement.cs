using UnityEngine;

public class TopDownMovement : MonoBehaviour, ITouchable, IStatusPlayer
{
    [Header("Movimento")]
    [SerializeField] float speed = 10f;

    [Header("Áudio (Footstep)")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float footstepInterval = 0.35f;

    private Vector2 _movement;
    private Rigidbody2D rb;
    private float footstepTimer;

    public bool canMove = false;
    public bool isWalking { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RespawnInGame.RegisterPlayer(this);
    }

    void Update()
    {
        if (!canMove)
        {
            _movement = Vector2.zero;
            isWalking = false;
            footstepTimer = 0f;
            return;
        }

        _movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isWalking = _movement != Vector2.zero;

        if (isWalking)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null || !canMove) return;
        rb.linearVelocity = _movement * speed;
    }

    private void PlayFootstep()
    {
        if (footstepAudioSource != null && footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            footstepAudioSource.PlayOneShot(footstepSounds[randomIndex]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out ITouchable target))
            return;

        target.Active();
    }

    public void Active()
    {
        // Implementação vazia
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        // ✅ Desliga totalmente a física do jogador enquanto o minigame está aberto
        if (rb != null)
        {
            rb.simulated = value;
            if (!value) rb.linearVelocity = Vector2.zero;
        }
    }
}