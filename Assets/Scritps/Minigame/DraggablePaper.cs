using UnityEngine;

public class DraggablePaper : MonoBehaviour
{
    private PaperMinigameManager manager;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;
    private Collider2D colisor;

    void Start()
    {
        originalPosition = transform.position;
        manager = FindAnyObjectByType<PaperMinigameManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        colisor = GetComponent<Collider2D>();

        // ✅ Começa invisível (esperando a mão soltar)
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (colisor != null) colisor.enabled = false;

        // ✅ Se inscreve no evento da mão
        PaperHandAnimation.OnPaperDropped += MostrarPapel;
    }

    void OnDestroy()
    {
        // Remove a inscrição para não dar erro
        PaperHandAnimation.OnPaperDropped -= MostrarPapel;
    }

    private void MostrarPapel()
    {
        // ✅ Quando a mão solta, o papel aparece!
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (colisor != null) colisor.enabled = true;

        // Remove a inscrição para não ficar aparecendo várias vezes
        PaperHandAnimation.OnPaperDropped -= MostrarPapel;
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