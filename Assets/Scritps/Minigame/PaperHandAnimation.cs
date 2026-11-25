using System;
using System.Collections;
using UnityEngine;

public class PaperHandAnimation : MonoBehaviour
{
    public static event Action OnPaperDropped; // ✅ Evento para avisar quando soltar o papel

    [Header("Configuração de Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteSegurandoPapel;
    [SerializeField] private Sprite spriteMaoVazia;

    [Header("Ajustes de Rotação e Flip")]
    [Tooltip("Ângulo em Z para a mão ficar em pé.")]
    [SerializeField] private float anguloZ = -90f;

    [Tooltip("Marque para espelhar a mão (inverter o lado).")]
    [SerializeField] private bool espelharHorizontal = true;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        // ✅ APLICA A ROTAÇÃO E O FLIP AQUI
        transform.rotation = Quaternion.Euler(0f, 0f, anguloZ);

        Vector3 scale = transform.localScale;
        scale.x = espelharHorizontal ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public IEnumerator AnimateHand(Vector3 startPos, Vector3 targetPos, float duration)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteSegurandoPapel;
            spriteRenderer.enabled = true;
        }

        // 1. Sobe segurando o papel falso
        transform.position = startPos;
        yield return null;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        transform.position = targetPos;

        yield return new WaitForSeconds(0.5f);

        // 2. Troca para mão vazia
        if (spriteRenderer != null)
            spriteRenderer.sprite = spriteMaoVazia;

        // ✅ 3. AVISA QUE O PAPEL FOI SOLTO (para o papel real aparecer)
        OnPaperDropped?.Invoke();

        // 4. Volta com a mão vazia
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(targetPos, startPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transform.position = startPos;
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }
}