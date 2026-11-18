using System.Collections;
using UnityEngine;

public class PaperVisualDemo : MonoBehaviour
{
    public IEnumerator AnimatePaper(Vector3 startPos, Vector3 targetPos, float duration)
    {
        transform.position = startPos;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transform.position = targetPos;
    }
}