using System.Collections;
using UnityEngine;

public class PaperHandAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;
    }

    public IEnumerator AnimateHand(Vector3 startPos, Vector3 targetPos, float duration)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;

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