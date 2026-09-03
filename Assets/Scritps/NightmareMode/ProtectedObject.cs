using UnityEngine;

public class ProtectedObject : MonoBehaviour
{
    [Header("Proteção")]
    [SerializeField] private bool isProtected = true;
    [SerializeField] private bool logWarning = true;
    [SerializeField] private bool destroyWhenActivated = true;

    void Start()
    {
        if (isProtected && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (isProtected)
        {
            if (logWarning)

            Invoke("Reativar", 0.001f);
        }
    }

    void Reativar()
    {
        if (isProtected && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            if (logWarning)
                Debug.Log($"{gameObject.name} were reactived by protection!");
        }
    }

    public void DestruirProtecao()
    {
        if (logWarning)
            Debug.Log($"pprotection destroyed");

        isProtected = false;
        Destroy(this);
    }

    public void DeactiveProtection()
    {
        if (logWarning)
        isProtected = false;
    }

    public void ToggleProtection(bool protect)
    {
        isProtected = protect;
    }
}