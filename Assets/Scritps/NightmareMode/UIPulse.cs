using UnityEngine;

public class UIPulse : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float velocidade = 6f;
    [SerializeField] private float escalaMinima = 0.8f;
    [SerializeField] private float escalaMaxima = 1.1f;

    private Vector3 escalaOriginal;

    void Start()
    {
        // Salva a escala que você configurou no Inspector (ex: 1, 1, 1)
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        // Faz o cálculo de pulsar
        float t = (Mathf.Sin(Time.unscaledTime * velocidade) + 1f) / 2f;
        float escalaAtual = Mathf.Lerp(escalaMinima, escalaMaxima, t);

        // Aplica a escala em cima da original
        transform.localScale = escalaOriginal * escalaAtual;
    }
}