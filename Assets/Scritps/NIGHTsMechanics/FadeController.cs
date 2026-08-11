using UnityEngine;
using System.Collections;

public class FadeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer fadeSprite; // ← SPRITE RENDERER!
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Colors")]
    [SerializeField] private Color fadeColor = Color.black;

    private void Start()
    {
        if (fadeSprite == null)
        {
            Debug.LogError("❌ FadeController: fadeSprite é NULL! Arraste o SpriteRenderer no Inspector.");
            return;
        }

        // COMEÇA TRANSPARENTE
        Color c = fadeColor;
        c.a = 0f;
        fadeSprite.color = c;

        Debug.Log("✅ FadeController INICIADO! fadeSprite encontrado.");
    }

    public IEnumerator FadeToBlack(float duration)
    {
        if (fadeSprite == null)
        {
            Debug.LogError("❌ FadeToBlack: fadeSprite é NULL!");
            yield break;
        }

        Debug.Log($"🌑 FadeToBlack INICIADO! Duração: {duration}");

        float elapsed = 0f;
        Color startColor = fadeSprite.color;
        Color targetColor = fadeColor;
        targetColor.a = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            fadeSprite.color = Color.Lerp(startColor, targetColor, t);
            Debug.Log($"🌑 Fade alpha: {fadeSprite.color.a}");
            yield return null;
        }

        fadeSprite.color = targetColor;
        Debug.Log("🌑 FadeToBlack FINALIZADO!");
    }

    public IEnumerator FadeFromBlack(float duration)
    {
        if (fadeSprite == null)
        {
            Debug.LogError("❌ FadeFromBlack: fadeSprite é NULL!");
            yield break;
        }

        Debug.Log($"🌕 FadeFromBlack INICIADO! Duração: {duration}");

        float elapsed = 0f;
        Color startColor = fadeSprite.color;
        Color targetColor = fadeColor;
        targetColor.a = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            fadeSprite.color = Color.Lerp(startColor, targetColor, t);
            Debug.Log($"🌕 Fade alpha: {fadeSprite.color.a}");
            yield return null;
        }

        fadeSprite.color = targetColor;
        Debug.Log("🌕 FadeFromBlack FINALIZADO!");
    }

    public IEnumerator FlashScreen(int flashes)
    {
        if (fadeSprite == null)
        {
            Debug.LogError("❌ FlashScreen: fadeSprite é NULL!");
            yield break;
        }

        Debug.Log($"⚡ FlashScreen INICIADO! {flashes} flashes");

        Color visibleColor = fadeColor;
        visibleColor.a = 1f;

        Color invisibleColor = fadeColor;
        invisibleColor.a = 0f;

        for (int i = 0; i < flashes; i++)
        {
            Debug.Log($"⚡ Flash {i + 1}/{flashes} - PRETO");
            fadeSprite.color = visibleColor;
            yield return new WaitForSeconds(flashDuration);

            Debug.Log($"⚡ Flash {i + 1}/{flashes} - TRANSPARENTE");
            fadeSprite.color = invisibleColor;
            yield return new WaitForSeconds(flashDuration);
        }

        // GARANTE QUE TERMINA TRANSPARENTE
        fadeSprite.color = invisibleColor;
        Debug.Log("⚡ FlashScreen FINALIZADO!");
    }

    // MÉTODO PARA DEFINIR A COR DO FADE
    public void SetFadeColor(Color color)
    {
        fadeColor = color;
        Debug.Log($"🎨 Fade color alterado para: {color}");
    }

    // MÉTODO PARA DEFINIR OPACIDADE DIRETAMENTE
    public void SetAlpha(float alpha)
    {
        if (fadeSprite == null) return;

        Color c = fadeSprite.color;
        c.a = Mathf.Clamp01(alpha);
        fadeSprite.color = c;
        Debug.Log($"🎨 Alpha definido para: {alpha}");
    }

    // 🔧 MÉTODOS PARA TESTE - APERTE F, G, H NO JOGO
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("🔧 TESTE: Iniciando FlashScreen com 6 flashes!");
            StartCoroutine(FlashScreen(6));
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("🔧 TESTE: Iniciando FadeToBlack!");
            StartCoroutine(FadeToBlack(1f));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("🔧 TESTE: Iniciando FadeFromBlack!");
            StartCoroutine(FadeFromBlack(1f));
        }

        // TECLA J - ALTERNA VISIBILIDADE
        if (Input.GetKeyDown(KeyCode.J))
        {
            float newAlpha = fadeSprite.color.a >= 0.5f ? 0f : 1f;
            SetAlpha(newAlpha);
            Debug.Log($"🔧 TESTE: Alpha alterado para {newAlpha}");
        }
    }
}