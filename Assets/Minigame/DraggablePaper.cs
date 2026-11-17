using UnityEngine;

public class DraggablePaper : MonoBehaviour
{
    private PaperMinigameManager manager;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
        manager = FindAnyObjectByType<PaperMinigameManager>();
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

        // ✅ Pega TODOS os colisores no ponto onde o mouse soltou
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        PaperSpot spotEncontrado = null;

        foreach (Collider2D hit in hits)
        {
            // Ignora o colisor do próprio papel que estamos arrastando
            if (hit.gameObject == gameObject) continue;

            // Verifica se é o Spot pela Tag
            if (hit.CompareTag("Spot"))
            {
                spotEncontrado = hit.GetComponent<PaperSpot>();
                break;
            }
        }

        if (spotEncontrado != null)
        {
            Debug.Log($"[Papel] Soltou no Spot correto: {spotEncontrado.name}");

            if (spotEncontrado.IsHighlighted)
            {
                manager.OnPaperOrganizedCorrectly();
            }
            else
            {
                manager.OnPaperOrganizedWrongly();
            }
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("[Papel] Soltou fora do Spot ou no vazio.");
            transform.position = originalPosition; // Volta pra origem
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x + offset.x, mousePos.y + offset.y, transform.position.z);
    }
}