using UnityEngine;

public class ProtectedObject : MonoBehaviour
{
    [Header("Proteção")]
    [SerializeField] private bool isProtected = true;
    [SerializeField] private bool logWarning = true;
    [SerializeField] private bool destroyWhenActivated = true; // ← NOVO!

    void Start()
    {
        if (isProtected && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            Debug.Log($"🛡️ {gameObject.name} foi FORÇADO a ativar!");
        }
    }

    void OnDisable()
    {
        if (isProtected)
        {
            if (logWarning)
                Debug.LogWarning($"🛡️ {gameObject.name} NÃO PODE ser desativado! REATIVANDO...");

            Invoke("Reativar", 0.001f);
        }
    }

    void Reativar()
    {
        if (isProtected && !gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            if (logWarning)
                Debug.Log($"✅ {gameObject.name} foi REATIVADO pela proteção!");
        }
    }

    // ✅ MÉTODO PARA DESTRUIR A PROTEÇÃO QUANDO A WINDOW FOR ATIVADA
    public void DestruirProtecao()
    {
        if (logWarning)
            Debug.Log($"🔥 Proteção de {gameObject.name} DESTRUÍDA!");

        isProtected = false;

        // DESTROI O COMPONENTE
        Destroy(this);
    }

    // ✅ MÉTODO PARA DESATIVAR A PROTEÇÃO (MAS MANTER O COMPONENTE)
    public void DesativarProtecao()
    {
        if (logWarning)
            Debug.Log($"🔓 Proteção de {gameObject.name} DESATIVADA (mas componente mantido)");
        isProtected = false;
    }

    public void ToggleProtection(bool protect)
    {
        isProtected = protect;
        Debug.Log($"🛡️ Proteção de {gameObject.name}: {(protect ? "ATIVADA" : "DESATIVADA")}");
    }
}