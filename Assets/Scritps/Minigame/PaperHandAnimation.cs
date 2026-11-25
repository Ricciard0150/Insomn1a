using System;
using System.Collections;
using UnityEngine;

public class PaperHandAnimation : MonoBehaviour
{
    public static event Action<GameObject> OnPaperDropped;

    [Header("Configuração de Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteSegurandoPapel;
    [SerializeField] private Sprite spriteMaoVazia;

    [Header("Ajustes de Rotação e Flip")]
    [SerializeField] private float anguloZ = -90f;
    [SerializeField] private bool espelharHorizontal = true;

    [Header("Referência do Placeholder")]
    [SerializeField] private GameObject placeholderNaCena;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;

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

        if (spriteRenderer != null)
            spriteRenderer.sprite = spriteMaoVazia;

        OnPaperDropped?.Invoke(placeholderNaCena);

        if (placeholderNaCena != null)
            placeholderNaCena.SetActive(false);

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