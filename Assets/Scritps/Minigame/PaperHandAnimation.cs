using System;
using System.Collections;
using UnityEngine;

public class PaperHandAnimation : MonoBehaviour
{
    [Header("Configuração de Sprites")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite spriteSegurandoPapel;
    [SerializeField] private Sprite spriteMaoVazia;

    [Header("Ajustes de Rotação e Flip")]
    [SerializeField] private float anguloZ = -90f;
    [SerializeField] private bool espelharHorizontal = true;

    // ✅ Evento estático para o DraggablePaper escutar
    public static event Action<GameObject> OnPaperDropped;

    void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        transform.rotation = Quaternion.Euler(0f, 0f, anguloZ);
        Vector3 scale = transform.localScale;
        scale.x = espelharHorizontal ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public IEnumerator AnimateHand(
        Vector3 startPos,
        Vector3 targetPos,
        float duration,
        Action onPaperReleased = null)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = spriteSegurandoPapel;
            spriteRenderer.enabled = true;
        }

        // 1. Sobe com o papel
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

        // 2. Pausa no topo
        yield return new WaitForSeconds(0.5f);

        if (spriteRenderer != null)
            spriteRenderer.sprite = spriteMaoVazia;

        // ►►► DISPARA OS EVENTOS NO INÍCIO DA VOLTA ◄◄◄
        onPaperReleased?.Invoke();     // Avisa o MinigameManager (Action)
        OnPaperDropped?.Invoke(null);  // Avisa o DraggablePaper (Evento Estático)

        // 3. Desce vazia
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