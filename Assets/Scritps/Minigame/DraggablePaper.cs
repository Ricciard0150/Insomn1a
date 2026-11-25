using UnityEngine;

public class DraggablePaper : MonoBehaviour
{
    public Vector3 TamanhoOriginal { get; private set; } // ✅ Guarda o tamanho do Prefab

    private PaperMinigameManager manager;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisor;

    void Awake()
    {
        // ✅ Salva o tamanho que você configurou no Prefab antes de qualquer coisa
        TamanhoOriginal = transform.localScale;
    }

    void Start()
    {
        manager = FindAnyObjectByType<PaperMinigameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        colisor = GetComponent<Collider2D>();

        bool esperandoMao = FindAnyObjectByType<PaperHandAnimation>() != null;

        if (esperandoMao)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (colisor != null) colisor.enabled = false;
            PaperHandAnimation.OnPaperDropped += MostrarPapelNoSpot;
        }
        else
        {
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (colisor != null) colisor.enabled = true;
        }
    }

    void OnDestroy()
    {
        PaperHandAnimation.OnPaperDropped -= MostrarPapelNoSpot;
    }

    private void MostrarPapelNoSpot(GameObject placeholder)
    {
        if (placeholder != null)
        {
            transform.position = placeholder.transform.position;
            originalPosition = placeholder.transform.position;

            // ✅ Força o tamanho original do prefab, não o do placeholder
            transform.localScale = TamanhoOriginal;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (colisor != null) colisor.enabled = true;

        PaperHandAnimation.OnPaperDropped -= MostrarPapelNoSpot;
    }

    void OnMouseDown()
    {
        if (manager == null) return;
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        if (!isDragging) return;
        isDragging = false;

        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        PaperSpot spotEncontrado = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            if (hit.CompareTag("Spot"))
            {
                spotEncontrado = hit.GetComponent<PaperSpot>();
                break;
            }
        }

        if (spotEncontrado != null)
        {
            if (spotEncontrado.IsHighlighted)
                manager.OnPaperOrganizedCorrectly();
            else
                manager.OnPaperOrganizedWrongly();

            Destroy(gameObject);
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
    }
}