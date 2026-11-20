using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BlurController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Volume volume;

    [Header("Depth of Field")]
    [SerializeField] private float maxFocalLength = 300f;

    [Header("Vignette (Bordas)")]
    [SerializeField] private float maxVignetteIntensity = 0.5f; // O quão escuro as bordas ficam
    [SerializeField] private float maxVignetteSmoothness = 0.5f; // O quão suave é a transição

    private DepthOfField dof;
    private Vignette vignette;

    private void Start()
    {
        if (volume != null && volume.profile.TryGet(out dof))
            dof.active = false;

        if (volume != null && volume.profile.TryGet(out vignette))
        {
            vignette.active = false;
            vignette.intensity.value = 0f;
        }
    }

    /// <summary>
    /// Define a intensidade do blur (DOF + Vignette) de 0 (limpo) a 1 (total).
    /// </summary>
    public void SetBlurIntensity(float t)
    {
        t = Mathf.Clamp01(t);

        // --- Depth of Field ---
        if (dof != null)
        {
            if (t <= 0.001f)
            {
                dof.active = false;
                dof.focalLength.value = 0f;
            }
            else
            {
                dof.active = true;
                dof.focalLength.value = Mathf.Lerp(0f, maxFocalLength, t);
            }
        }

        // --- Vignette (Bordas Escuras) ---
        if (vignette != null)
        {
            if (t <= 0.001f)
            {
                vignette.active = false;
                vignette.intensity.value = 0f;
            }
            else
            {
                vignette.active = true;
                vignette.intensity.value = Mathf.Lerp(0f, maxVignetteIntensity, t);
                vignette.smoothness.value = maxVignetteSmoothness;
            }
        }
    }

    public void TurnOnBlur()
    {
        if (dof != null)
        {
            dof.active = true;
            dof.focalLength.value = maxFocalLength;
        }

        if (vignette != null)
        {
            vignette.active = true;
            vignette.intensity.value = maxVignetteIntensity;
            vignette.smoothness.value = maxVignetteSmoothness;
        }
    }

    public void TurnOffBlur()
    {
        if (dof != null) dof.active = false;
        if (vignette != null) vignette.active = false;
    }

    public IEnumerator Piscar()
    {
        TurnOnBlur();
        yield return new WaitForSeconds(0.15f);
        TurnOffBlur();
    }

    public IEnumerator PiscarComFade()
    {
        yield return StartCoroutine(FadeBlur(0f, 1f, 0.15f));
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(FadeBlur(1f, 0f, 0.15f));
    }

    private IEnumerator FadeBlur(float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float current = Mathf.Lerp(start, end, t);
            SetBlurIntensity(current);
            yield return null;
        }
        SetBlurIntensity(end);
    }
}