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
                Debug.Log("savenotifimanager created ");
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
        Debug.Log("savenotificationamanager appeared");
    }

    public void RegisterNotification(SaveNotification notif)
    {
        if (notification == null)
        {
            notification = notif;
            Debug.Log("save not registered in manager");
        }
    }


}