using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Música de Fundo")]
    [SerializeField] private AudioSource bgmSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.Play();
    }

    // ✅ Para a música completamente (não só pausa)
    public void PausarMusica()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop(); // Stop é mais forte que Pause
            Debug.Log("[AudioManager] Música de fundo PARADA.");
        }
    }

    // ✅ Retoma a música do zero
    public void RetomarMusica()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
            Debug.Log("[AudioManager] Música de fundo TOCANDO.");
        }
    }
}