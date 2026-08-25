using UnityEngine;
using UnityEngine.UI;

public class PulseEffect : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float minScale = 0.8f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private bool playOnStart = false;

    [Header("Color Settings (Opcional)")]
    [SerializeField] private bool useColorChange = true;
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = Color.yellow;

    private RectTransform rectTransform;
    private Image image;
    private SpriteRenderer spriteRenderer;
    private bool isPulsing = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            rectTransform = gameObject.AddComponent<RectTransform>();

        image = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (playOnStart)
            StartPulse();
    }

    void OnDisable()
    {
        StopPulse();
        ResetVisuals();
    }

    void Update()
    {
        if (!isPulsing) return;

        float pulse = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        rectTransform.localScale = new Vector3(pulse, pulse, 1f);

        if (useColorChange)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            Color currentColor = Color.Lerp(colorA, colorB, t);

            if (image != null)
                image.color = currentColor;
            else if (spriteRenderer != null)
                spriteRenderer.color = currentColor;
        }
    }

    public void StartPulse()
    {
        isPulsing = true;
        ResetVisuals();
    }

    public void StopPulse()
    {
        isPulsing = false;
        ResetVisuals();
    }

    private void ResetVisuals()
    {
        if (rectTransform != null)
            rectTransform.localScale = Vector3.one;

        if (image != null)
            image.color = Color.white;
        else if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    public void SetPulseSpeed(float speed)
    {
        pulseSpeed = speed;
    }

    public void SetPulseRange(float min, float max)
    {
        minScale = min;
        maxScale = max;
    }
}