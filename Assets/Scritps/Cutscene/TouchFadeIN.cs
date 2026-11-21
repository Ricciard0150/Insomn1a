using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TouchFadeIn : MonoBehaviour
{
    [Header("Referências (UI)")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private TMP_Text fadeText;
    [SerializeField] private Button fadeButton;
    [SerializeField] private CanvasGroup fadeObject;

    [Header("Tempos de Fade")]
    [SerializeField] private float tempoFadeImagem = 1.5f;
    [SerializeField] private float tempoFadeTexto = 1.5f;
    [SerializeField] private float tempoFadeBotao = 1.5f;
    [SerializeField] private float tempoFadeObjeto = 1.5f;

    [Header("Configurações")]
    [SerializeField] private bool apenasUmaVez = true;

    private bool jaAtivou = false;

    // Cores originais que serão salvas na hora do trigger
    private Color corOriginalImagem;
    private Color corOriginalTexto;
    private Color corOriginalBotaoFundo;
    private Color corOriginalBotaoTexto;

    void Start()
    {
        // Deixa tudo invisível de início, mas mantém as cores RGB para não bugar
        if (fadeImage != null) fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
        if (fadeText != null) fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, 0f);
        if (fadeObject != null) fadeObject.alpha = 0f;

        if (fadeButton != null)
        {
            Image img = fadeButton.GetComponent<Image>();
            if (img != null) img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);

            TMP_Text txt = fadeButton.GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 0f);
        }

        if (fadeImage != null) fadeImage.raycastTarget = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer _))
        {
            if (jaAtivou && apenasUmaVez) return;

            jaAtivou = true;
            StartCoroutine(FazerFadeIn());
        }
    }

    private IEnumerator FazerFadeIn()
    {
        // 1. ATIVA TUDO PRIMEIRO
        if (fadeImage != null) fadeImage.gameObject.SetActive(true);
        if (fadeText != null) fadeText.gameObject.SetActive(true);
        if (fadeButton != null) fadeButton.gameObject.SetActive(true);
        if (fadeObject != null) fadeObject.gameObject.SetActive(true);

        // 2. Espera um frame para o Unity registrar os componentes
        yield return null;

        // 3. AGORA salva as cores reais (que estão no Inspector)
        if (fadeImage != null) corOriginalImagem = fadeImage.color;
        if (fadeText != null) corOriginalTexto = fadeText.color;

        if (fadeButton != null)
        {
            Image btnImage = fadeButton.GetComponent<Image>();
            if (btnImage != null) corOriginalBotaoFundo = btnImage.color;

            TMP_Text btnText = fadeButton.GetComponentInChildren<TMP_Text>();
            if (btnText != null) corOriginalBotaoTexto = btnText.color;
        }

        // 4. Faz o Fade
        float tempoMaximo = Mathf.Max(tempoFadeImagem, tempoFadeTexto, tempoFadeBotao, tempoFadeObjeto);
        float elapsed = 0f;

        while (elapsed < tempoMaximo)
        {
            elapsed += Time.unscaledDeltaTime;

            float tImg = tempoFadeImagem > 0 ? Mathf.Clamp01(elapsed / tempoFadeImagem) : 1f;
            float tTxt = tempoFadeTexto > 0 ? Mathf.Clamp01(elapsed / tempoFadeTexto) : 1f;
            float tBtn = tempoFadeBotao > 0 ? Mathf.Clamp01(elapsed / tempoFadeBotao) : 1f;
            float tObj = tempoFadeObjeto > 0 ? Mathf.Clamp01(elapsed / tempoFadeObjeto) : 1f;

            // Aplica o Alpha (se o objeto existir)
            if (fadeImage != null)
                fadeImage.color = new Color(corOriginalImagem.r, corOriginalImagem.g, corOriginalImagem.b, tImg);

            if (fadeText != null)
                fadeText.color = new Color(corOriginalTexto.r, corOriginalTexto.g, corOriginalTexto.b, tTxt);

            if (fadeButton != null)
            {
                Image btnImg = fadeButton.GetComponent<Image>();
                if (btnImg != null && corOriginalBotaoFundo != null)
                    btnImg.color = new Color(corOriginalBotaoFundo.r, corOriginalBotaoFundo.g, corOriginalBotaoFundo.b, tBtn);

                TMP_Text btnTxt = fadeButton.GetComponentInChildren<TMP_Text>();
                if (btnTxt != null && corOriginalBotaoTexto != null)
                    btnTxt.color = new Color(corOriginalBotaoTexto.r, corOriginalBotaoTexto.g, corOriginalBotaoTexto.b, tBtn);

                fadeButton.interactable = (tBtn > 0.5f);
            }

            if (fadeObject != null)
                fadeObject.alpha = tObj;

            yield return null;
        }

        // 5. Garante que tudo fique 100% visível no final
        if (fadeImage != null)
            fadeImage.color = new Color(corOriginalImagem.r, corOriginalImagem.g, corOriginalImagem.b, 1f);

        if (fadeText != null)
            fadeText.color = new Color(corOriginalTexto.r, corOriginalTexto.g, corOriginalTexto.b, 1f);

        if (fadeButton != null)
        {
            Image btnImg = fadeButton.GetComponent<Image>();
            if (btnImg != null && corOriginalBotaoFundo != null)
                btnImg.color = new Color(corOriginalBotaoFundo.r, corOriginalBotaoFundo.g, corOriginalBotaoFundo.b, 1f);

            TMP_Text btnTxt = fadeButton.GetComponentInChildren<TMP_Text>();
            if (btnTxt != null && corOriginalBotaoTexto != null)
                btnTxt.color = new Color(corOriginalBotaoTexto.r, corOriginalBotaoTexto.g, corOriginalBotaoTexto.b, 1f);

            fadeButton.interactable = true;
        }

        if (fadeObject != null)
            fadeObject.alpha = 1f;
    }
}