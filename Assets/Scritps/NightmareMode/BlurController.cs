using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BlurController : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private float blinkDuration = 0.15f;
    [SerializeField] private float maxFocalLength = 300f;

    private DepthOfField dof;

    private void Start()
    {
        if (volume != null && volume.profile.TryGet(out dof))
            dof.active = false;
    }

    public void TurnOnBlur()
    {
        if (dof != null)
            dof.active = true;
    }

    public void TurnOffBlur()
    {
        if (dof != null)
            dof.active = false;
    }

    public IEnumerator Piscar()
    {
        TurnOnBlur();
        yield return new WaitForSeconds(blinkDuration);
        TurnOffBlur();
    }

    public IEnumerator PiscarComFade()
    {
        yield return StartCoroutine(FadeBlur(0f, 1f, blinkDuration));
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(FadeBlur(1f, 0f, blinkDuration));
    }

    private IEnumerator FadeBlur(float start, float end, float duration)
    {
        if (dof == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float current = Mathf.Lerp(start, end, t);
            dof.focalLength.value = Mathf.Lerp(0f, maxFocalLength, current);
            dof.active = true;
            yield return null;
        }

        if (end == 0)
            dof.active = false;
    }
}