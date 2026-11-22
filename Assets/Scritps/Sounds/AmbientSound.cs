using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ambientClip;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && ambientClip != null)
        {
            audioSource.clip = ambientClip;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }

    public void StopAmbient()
    {
        Debug.Log("<color=yellow>[AmbientSound]</color> Tentando parar o som ambiente...");

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null; // Remove o clipe para garantir que não toque de novo
            Debug.Log("<color=green>[AmbientSound]</color> Som ambiente PARADO com sucesso!");
        }
        else
        {
            Debug.LogError("<color=red>[AmbientSound]</color> O AudioSource está NULO! Não foi possível parar.");
        }
    }

    public void ResumeAmbient()
    {
        if (audioSource != null && ambientClip != null)
        {
            audioSource.clip = ambientClip;
            audioSource.loop = true;
            audioSource.Play();
            Debug.Log("[AmbientSound] Som ambiente retomado.");
        }
    }
}