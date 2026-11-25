using UnityEngine;

public class PlaceholderController : MonoBehaviour
{
    // ✅ Arraste o Placeholder para cá no Inspector
    [SerializeField] private GameObject placeholderVisual;

    void Start()
    {
        // Se você quiser que ele comece escondido (opcional)
        if (placeholderVisual != null)
            placeholderVisual.SetActive(true);
    }

    // Esta função vai ser chamada pela Mão quando ela soltar o papel
    public void EsconderPlaceholder()
    {
        if (placeholderVisual != null)
            placeholderVisual.SetActive(false);
    }
}