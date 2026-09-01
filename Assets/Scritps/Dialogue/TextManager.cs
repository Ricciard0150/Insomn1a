using UnityEngine;
using System.Collections.Generic;

public class TextManager : MonoBehaviour
{
    private static TextManager _instance;
    public static TextManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("TextManager");
                _instance = go.AddComponent<TextManager>();
                DontDestroyOnLoad(go);
                Debug.Log("🆕 TextManager CRIADO!");
            }
            return _instance;
        }
    }

    private List<InteractionDetector> npcs = new List<InteractionDetector>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ TextManager SINGLETON inicializado!");
    }

    public void RegisterNPC(InteractionDetector npc)
    {
        if (npc == null) return;

        if (!npcs.Contains(npc))
        {
            npcs.Add(npc);
            Debug.Log($"➕ NPC registrado: {npc.gameObject.name} (Total: {npcs.Count})");
        }
    }

    public void UnregisterNPC(InteractionDetector npc)
    {
        if (npc == null) return;

        if (npcs.Contains(npc))
        {
            npcs.Remove(npc);
            Debug.Log($"➖ NPC removido: {npc.gameObject.name} (Total: {npcs.Count})");
        }
    }

    public void InteractWithNPCs()
    {
        npcs.RemoveAll(n => n == null);

        Debug.Log($"🖱️ Interact! NPCs disponíveis: {npcs.Count}");

        foreach (var npc in npcs)
        {
            if (npc != null && npc.PlayerPerto)
            {
                npc.Interact();
                return;
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            InteractWithNPCs();
        }
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        npcs.RemoveAll(n => n == null);
        Debug.Log($"📂 Cena carregada! NPCs ativos: {npcs.Count}");
    }

    void OnEnable() => UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
}