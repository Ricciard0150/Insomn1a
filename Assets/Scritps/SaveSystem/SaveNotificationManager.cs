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
                Debug.Log("🆕 SaveNotificationManager criado!");
            }
            return _instance;
        }
    }

    private SaveNotification notification;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ SaveNotificationManager inicializado!");
    }

    public void RegisterNotification(SaveNotification notif)
    {
        if (notification == null)
        {
            notification = notif;
            Debug.Log("✅ SaveNotification registrada no Manager!");
        }
    }

    //public void Show(string msg = null)
    //{
    //    if (notification != null)
    //    {
    //        notification.ShowNotification(msg ?? "💾 Jogo Salvo!");
    //    }
    //    else
    //    {
    //        Debug.LogError("❌ SaveNotification não registrado!");
    //    }
    //}
}