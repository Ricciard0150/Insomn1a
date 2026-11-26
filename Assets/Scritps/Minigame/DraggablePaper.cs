using UnityEngine;

public class DraggablePaper : MonoBehaviour
{
    public Vector3 TamanhoOriginal { get; private set; }

    private PaperMinigameManager manager;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisor;

    void Awake()
    {
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

            // ✅ Escuta o evento estático da mão
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
        // Libera o papel para ser arrastado
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (colisor != null) colisor.enabled = true;

        // Evita que ele fique escutando múltiplas vezes
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