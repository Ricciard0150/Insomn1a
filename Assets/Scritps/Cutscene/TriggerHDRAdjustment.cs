using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TriggerHDREffect : MonoBehaviour
{
    [Header("Volume")]
    [Tooltip("Arraste o Global Volume que tem o Color Adjustments aqui")]
    [SerializeField] private Volume globalVolume;

    [Header("Configuração do HDR")]
    [Tooltip("A cor que o HDR vai ficar (ex: vermelho, roxo, etc)")]
    [SerializeField] private Color corHDR = new Color(1f, 0f, 0f);

    [Tooltip("Intensidade do brilho (HDR). Quanto maior, mais forte a luz.")]
    [SerializeField] private float intensidadeHDR = 2.5f;

    [Header("Player")]
    [Tooltip("Arraste o TopDownMovement do Player")]
    [SerializeField] private TopDownMovement playerMovement;

    [Header("Tempo")]
    [Tooltip("Quanto tempo o efeito dura antes de voltar ao normal")]
    [SerializeField] private float tempoDuracao = 3f;

    [Header("Configurações")]
    [SerializeField] private bool apenasUmaVez = true;

    // ✅ NOVO: Som de efeito
    [Header("Efeito Sonoro")]
    [Tooltip("AudioSource que toca o som do efeito (Play On Awake DESMARCADO, Loop DESMARCADO)")]
    [SerializeField] private AudioSource sfxAudioSource;
    [Tooltip("Som de impacto/sting tocado no momento em que o efeito ativa")]
    [SerializeField] private AudioClip sfxClip;
    [Tooltip("Volume do efeito sonoro")]
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private bool jaAtivou = false;
    private ColorAdjustments colorAdjustments;

    void Start()
    {
        // Pega o Color Adjustments do Volume
        if (globalVolume != null)
            globalVolume.profile.TryGet(out colorAdjustments);

        // ✅ Garante que o AudioSource está configurado corretamente
        if (sfxAudioSource != null)
        {
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.loop = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IStatusPlayer _)) return;
        if (apenasUmaVez && jaAtivou) return;

        jaAtivou = true;
        StartCoroutine(SequenciaHDR());
    }

    private IEnumerator SequenciaHDR()
    {
        // 1. Trava o player
        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        // 2. Toca o som do efeito
        PlaySFX();

        // 3. Muda o HDR (Color Filter + Post Exposure)
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.overrideState = true;
            colorAdjustments.colorFilter.value = corHDR;

            colorAdjustments.postExposure.overrideState = true;
            colorAdjustments.postExposure.value = intensidadeHDR;
        }

        Debug.Log("[TriggerHDREffect] Efeito HDR ativado!");

        // 4. Espera a duração do efeito
        yield return new WaitForSecondsRealtime(tempoDuracao);

        // 5. Volta ao normal (branco e exposição zero)
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = Color.white;
            colorAdjustments.postExposure.value = 0f;

            // Se quiser desativar a sobrescrita, descomente:
            // colorAdjustments.colorFilter.overrideState = false;
            // colorAdjustments.postExposure.overrideState = false;
        }

        Debug.Log("[TriggerHDREffect] Efeito HDR desativado!");

        // 6. Destrava o player
        if (playerMovement != null)
            playerMovement.SetCanMove(true);
    }

    // ✅ Toca o som uma vez
    private void PlaySFX()
    {
        if (sfxAudioSource == null || sfxClip == null) return;

        // PlayOneShot permite vários sons em paralelo sem cortar
        sfxAudioSource.PlayOneShot(sfxClip, sfxVolume);
    }
}