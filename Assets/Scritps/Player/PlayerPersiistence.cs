using UnityEngine;

public class PlayerPersistense : MonoBehaviour
{
    private static PlayerPersistense instance;

    void Awake()
    {
        // Evita duplicatas se você esquecer de remover o player da cena nova
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
