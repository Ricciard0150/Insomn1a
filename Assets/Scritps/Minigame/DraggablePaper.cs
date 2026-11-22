using UnityEngine;

public class DraggablePaper : MonoBehaviour
{
    private PaperMinigameManager manager;
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private float zOriginal; // ✅ Guarda o Z original

    void Start()
    {
        originalPosition = transform.position;
        zOriginal = transform.position.z; // Salva o Z que o papel nasceu
        manager = FindAnyObjectByType<PaperMinigameManager>();
    }

    void OnMouseDown()
    {
        if (manager == null) return;
        isDragging = true;

        // Pega a posição do mouse no mundo
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Força o Z do mouse para o Z do papel (para o cálculo do offset ficar certo)
        mouseWorldPos.z = zOriginal;

        offset = transform.position - mouseWorldPos;
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

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ✅ Força o Z de destino a ser o mesmo do papel, ignorando o Z da câmera
        transform.position = new Vector3(
            mouseWorldPos.x + offset.x,
            mouseWorldPos.y + offset.y,
            zOriginal // Mantém o Z original intacto
        );
    }
}