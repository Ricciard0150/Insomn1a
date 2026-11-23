using UnityEngine;

public class PainelAudioController : MonoBehaviour
{
    [Header("Música de Fundo (O AudioManager da Cena)")]
    [SerializeField] private AudioManager audioManagerGeral;

    [Header("Música do Minigame (O AudioSource DESTE painel)")]
    [SerializeField] private AudioSource musicaDoPainel;

    private AudioManager _manager;

    void Awake()
    {
        _manager = audioManagerGeral != null
            ? audioManagerGeral
            : FindAnyObjectByType<AudioManager>();

        if (_manager == null)
            Debug.LogWarning("[Áudio] PainelAudioController não encontrou nenhum AudioManager na cena.");
    }

    void OnEnable()
    {
        // A música geral JÁ foi pausada pelo StartMinigameNPC no momento da interação.
        // Aqui só precisamos tocar a música do minigame.
        if (musicaDoPainel != null)
        {
            musicaDoPainel.Play();
            Debug.Log("[Áudio] Painel abriu: música do minigame TOCANDO.");
        }
    }

    void OnDisable()
    {
        // 1. Para a música do minigame
        if (musicaDoPainel != null)
        {
            musicaDoPainel.Stop();
            Debug.Log("[Áudio] Painel fechou: música do minigame PARADA.");
        }

        // 2. Retoma a música geral do jogo
        if (_manager != null)
        {
            _manager.RetomarMusica();
            Debug.Log("[Áudio] Música geral do jogo RETOMADA.");
        }
    }
}