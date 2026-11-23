using UnityEngine;

public class TopDownMovement : MonoBehaviour, ITouchable, IStatusPlayer
{
    [Header("Movimento")]
    [SerializeField] float speed = 10f;

    [Header("Áudio (Footstep)")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepSounds; // Array para variar o som
    [SerializeField] private float footstepInterval = 0.35f; // Tempo entre um passo e outro

    private Vector2 _movement;
    private Rigidbody2D rb;
    private float footstepTimer;

    public bool canMove = false;
    public bool isWalking { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        RespawnInGame.RegisterPlayer(this); // ✅ registra no SpawnPoint da cena
    }

    void Update()
    {
        if (!canMove)
        {
            _movement = Vector2.zero;
            isWalking = false;

            // Se parar de andar, reseta o timer do passo
            footstepTimer = 0f;
            return;
        }

        _movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        isWalking = _movement != Vector2.zero;

        // ✅ Lógica do som de passos
        if (isWalking)
        {
            footstepTimer -= Time.deltaTime;

            // Quando o tempo zerar, toca o som e reseta o timer
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            // Se parar de andar, o timer zera para o próximo passo ser imediato
            footstepTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.linearVelocity = _movement * speed;
    }

    private void PlayFootstep()
    {
        if (footstepAudioSource != null && footstepSounds.Length > 0)
        {
            // Sorteia um dos sons do array para não ficar repetitivo
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
        // Se precisar implementar algo, coloque aqui
    }

    public void SetCanMove(bool value) => canMove = value;
}