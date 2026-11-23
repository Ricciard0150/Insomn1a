using UnityEngine;

public class PainelAudioController : MonoBehaviour
{
    [Header("Música de Fundo (O AudioManager da Cena)")]
    [SerializeField] private AudioManager audioManagerGeral;

    [Header("Música do Minigame (O AudioSource DESTE painel)")]
    [SerializeField] private AudioSource musicaDoPainel;

    void OnEnable()
    {
        // 1. Para a música de fundo GERAL
        if (audioManagerGeral != null)
        {
            audioManagerGeral.PausarMusica();
            Debug.Log("[Áudio] Painel abriu: Música geral PARADA.");
        }
        else
        {
            // Fallback: tenta achar o AudioManager na cena se não foi arrastado
            AudioManager manager = FindAnyObjectByType<AudioManager>();
            if (manager != null)
            {
                manager.PausarMusica();
            }
        }

        // 2. Toca a música do minigame
        if (musicaDoPainel != null)
        {
            musicaDoPainel.Play();
            Debug.Log("[Áudio] Música do minigame começou a tocar.");
        }
    }

    void OnDisable()
    {
        // 1. Para a música do minigame
        if (musicaDoPainel != null)
        {
            musicaDoPainel.Stop();
        }

        // 2. Volta a música de fundo
        if (audioManagerGeral != null)
        {
            audioManagerGeral.RetomarMusica();
            Debug.Log("[Áudio] Música geral voltou a tocar.");
        }
        else
        {
            AudioManager manager = FindAnyObjectByType<AudioManager>();
            if (manager != null)
            {
                manager.RetomarMusica();
            }
        }
    }
}