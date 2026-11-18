using UnityEngine;

public class PaperSpot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spotRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow; // Cor do brilho

    public bool IsHighlighted { get; private set; } = false;

    void Awake()
    {
        if (spotRenderer == null)
            spotRenderer = GetComponent<SpriteRenderer>();

        SetHighlight(false);
    }

    public void SetHighlight(bool active)
    {
        IsHighlighted = active;
        if (spotRenderer != null)
        {
            spotRenderer.color = active ? highlightColor : normalColor;
        }
    }
}