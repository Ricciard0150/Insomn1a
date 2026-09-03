using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTeleport : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform teleportUI;

    [Header("Fade Panel")]
    [SerializeField] private Image fadePanel; 
    [SerializeField] private  float duracaoFade = 1.5f;

    [SerializeField] private float delay = 0.5f;

     private bool isTeleporting = false;

    

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

        target.position = teleportUI.position;

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