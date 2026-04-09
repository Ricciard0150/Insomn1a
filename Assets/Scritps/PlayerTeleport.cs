using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTeleport : MonoBehaviour
{
    public Transform target;
    public Transform teleportUI;

    [Header("Fade Panel")]
    public Image fadePanel; // panel preto
    public float duracaoFade = 1.5f;

    public float delay = 0.5f;

    bool isTeleporting = false;

    void Start()
    {
        // começa transparente
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            fadePanel.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            if (!isTeleporting)
            {
                StartCoroutine(TeleportComFade());
            }
        }
    }

    IEnumerator TeleportComFade()
    {
        isTeleporting = true;

        Color cor = fadePanel.color;

        // 🔥 ESCURECER
        float t = 0f;
        while (t < duracaoFade)
        {
            t += Time.deltaTime;
            float alpha = t / duracaoFade;

            fadePanel.color = new Color(cor.r, cor.g, cor.b, alpha);
            yield return null;
        }

        fadePanel.color = new Color(cor.r, cor.g, cor.b, 1f);

        yield return new WaitForSeconds(delay);

        // 📍 TELEPORTA
        target.position = teleportUI.position;

        // 🌅 VOLTA AO NORMAL
        t = 0f;
        while (t < duracaoFade)
        {
            t += Time.deltaTime;
            float alpha = 1f - (t / duracaoFade);

            fadePanel.color = new Color(cor.r, cor.g, cor.b, alpha);
            yield return null;
        }

        fadePanel.color = new Color(cor.r, cor.g, cor.b, 0f);

        isTeleporting = false;
    }
}