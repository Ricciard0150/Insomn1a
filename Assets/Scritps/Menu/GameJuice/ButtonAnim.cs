using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

public class ButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.black;
    public Color pressColor = new Color(0.2f, 0.2f, 0.2f);
    public Color textNormalColor = Color.white;
    public Color textHoverColor = Color.white;

    [Header("Scale")]
    public float hoverScale = 1.15f;
    public float pressScale = 0.9f;

    [Header("Animation")]
    public float animationDuration = 0.15f;

    [Header("Click Event")]
    public UnityEvent onClick; // Appears in the Inspector

    private Image image;
    private Text text;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Coroutine currentAnimation;
    private bool isHovering = false;

    void Start()
    {
        // Get components
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        // If no image, create one
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
            image.sprite = CreateWhiteSprite();
        }

        // Configure Button for Color Tint
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.transition = Selectable.Transition.ColorTint;
            btn.targetGraphic = image;
        }
    }

    // ===== MOUSE EVENTS =====

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        StopAnimation();
        currentAnimation = StartCoroutine(Animate(
            originalScale,
            Vector3.one * hoverScale,
            normalColor,
            hoverColor,
            textNormalColor,
            textHoverColor
        ));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        StopAnimation();
        currentAnimation = StartCoroutine(Animate(
            rectTransform.localScale,
            originalScale,
            image.color,
            normalColor,
            text != null ? text.color : textNormalColor,
            textNormalColor
        ));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isHovering) return;
        StopAnimation();
        currentAnimation = StartCoroutine(Animate(
            rectTransform.localScale,
            Vector3.one * pressScale,
            image.color,
            pressColor,
            text != null ? text.color : textNormalColor,
            textHoverColor
        ));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAnimation();

        Vector3 targetScale = isHovering ? Vector3.one * hoverScale : originalScale;
        Color targetColor = isHovering ? hoverColor : normalColor;
        Color targetTextColor = isHovering ? textHoverColor : textNormalColor;

        currentAnimation = StartCoroutine(Animate(
            rectTransform.localScale,
            targetScale,
            image.color,
            targetColor,
            text != null ? text.color : textNormalColor,
            targetTextColor
        ));

        // ===== TRIGGER CLICK EVENT =====
        onClick.Invoke();
    }

    // ===== ANIMATION =====

    IEnumerator Animate(Vector3 startScale, Vector3 endScale,
                       Color startColor, Color endColor,
                       Color startTextColor, Color endTextColor)
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            t = t * t * (3f - 2f * t); // SmoothStep

            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            image.color = Color.Lerp(startColor, endColor, t);

            if (text != null)
                text.color = Color.Lerp(startTextColor, endTextColor, t);

            yield return null;
        }

        rectTransform.localScale = endScale;
        image.color = endColor;
        if (text != null)
            text.color = endTextColor;
    }

    void StopAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
    }

    Sprite CreateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
    }
}