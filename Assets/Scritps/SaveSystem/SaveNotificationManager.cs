using UnityEngine;

public class SaveNotificationManager : MonoBehaviour
{
    private static SaveNotificationManager _instance;
    public static SaveNotificationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("SaveNotificationManager");
                _instance = go.AddComponent<SaveNotificationManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [SerializeField] private SaveNotification notification; // ✅ REFERÊNCIA DIRETA

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Show(string msg = null)
    {
        if (notification != null)
            notification.ShowNotification(msg ?? "💾 Jogo Salvo!");
        else
            Debug.LogError("❌ SaveNotification não referenciado! Arraste no Inspector.");
    }
    public void RegisterNotification(SaveNotification notif)
    {
        notification = notif;
        Debug.Log("✅ SaveNotification registrada!");
    }
}