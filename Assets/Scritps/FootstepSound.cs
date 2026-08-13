using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepSound : MonoBehaviour
{
    public AudioClip[] sonsPasso;
    public float intervaloPasso = 0.4f;

    private AudioSource audioSource;
    private TopDownMovement player;

    private float timer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GetComponent<TopDownMovement>();
    }

    void Update()
    {
        if (player.isWalking)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                TocarPasso();
                timer = intervaloPasso;
            }
        }
        else
        {
            // 🔥 PARA IMEDIATAMENTE
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            timer = 0f;
        }
    }

    void TocarPasso()
    {
        if (sonsPasso.Length == 0) return;

        int i = Random.Range(0, sonsPasso.Length);

        audioSource.clip = sonsPasso[i];
        audioSource.Play(); // 👈 NÃO usa PlayOneShot
    }
}