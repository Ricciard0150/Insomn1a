using UnityEngine;
using UnityEngine.EventSystems;

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

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (colisor != null) colisor.enabled = true;

        originalPosition = transform.position;
    }

    void OnMouseDown()
    {
        if (manager == null || !manager.IsMinigameRunning) return;

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

        // ❌ REMOVIDO: EventSystem.current.IsPointerOverGameObject()
        // Não precisamos mais disso, o OnMouseDrag já ignora a UI por padrão no Unity

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
    }
}