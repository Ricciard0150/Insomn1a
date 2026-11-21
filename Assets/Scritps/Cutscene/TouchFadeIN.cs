using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TouchFadeIn : MonoBehaviour
{
    [Header("Referências (UI)")]
    [Tooltip("Arraste a Imagem preta que está no seu Canvas aqui")]
    [SerializeField] private Image fadeImage;

    [Header("Configurações")]
    [Tooltip("Tempo que a tela leva para ficar totalmente preta")]
    [SerializeField] private float tempoFade = 1.5f;

    [Tooltip("Se marcado, o fade só acontece uma vez")]
    [SerializeField] private bool apenasUmaVez = true;

    private bool jaAtivou = false;

    void Start()
    {
        // Garante que a imagem comece invisível (Alpha 0)
        if (fadeImage != null)
        {
            Color cor = fadeImage.color;
            cor.a = 0f;
            fadeImage.color = cor;

            // Opcional: desativa o objeto para não bloquear cliques
            fadeImage.raycastTarget = false;
        }
        else
        {
            Debug.LogWarning("[TouchFadeIn] A Imagem de Fade não foi arrastada no Inspector!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem encostou foi o Player
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            if (jaAtivou && apenasUmaVez) return;

            jaAtivou = true;
            StartCoroutine(FazerFadeIn());
        }
    }

    private IEnumerator FazerFadeIn()
    {
        if (fadeImage == null) yield break;

        // Garante que o objeto da imagem esteja ativo para poder aparecer
        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        Color cor = fadeImage.color;

        // Loop para escurecer a tela
        while (elapsed < tempoFade)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / tempoFade;

            // Muda apenas o Alpha (transparência)
            cor.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = cor;

            yield return null;
        }

        // Garante que fique 100% preto no final
        cor.a = 1f;
        fadeImage.color = cor;
    }
}