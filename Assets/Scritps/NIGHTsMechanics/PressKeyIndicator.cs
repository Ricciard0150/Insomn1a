using UnityEngine;

public class PressEIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject indicatorObject;

    [Header("Pulse Settings")]
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float pulseSpeed = 2f;

    private PulseEffect pulseEffect;

    void Start()
    {
        if (indicatorObject != null)
            indicatorObject.SetActive(false);

        // Pega o PulseEffect do mesmo GameObject
        pulseEffect = GetComponent<PulseEffect>();

        // Se não tiver PulseEffect, adiciona
        if (pulseEffect == null)
        {
            pulseEffect = gameObject.AddComponent<PulseEffect>();
        }

        // Configura o PulseEffect
        pulseEffect.SetPulseRange(minScale, maxScale);
        pulseEffect.SetPulseSpeed(pulseSpeed);
    }

    public void Show()
    {
        if (indicatorObject != null)
        {
            indicatorObject.SetActive(true);

            if (pulseEffect != null)
                pulseEffect.StartPulse();

            Debug.Log("✅ PressEIndicator MOSTRADO!");
        }
    }

    public void Hide()
    {
        if (indicatorObject != null)
        {
            indicatorObject.SetActive(false);

            if (pulseEffect != null)
                pulseEffect.StopPulse();

            Debug.Log("❌ PressEIndicator ESCONDIDO!");
        }
    }
}