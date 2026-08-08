using UnityEngine;

public class TextManager : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private KeyCode teclaInteragir = KeyCode.E;

    [Header("References - Arraste no Inspector")]
    [SerializeField] private InteractionDetector[] npcs; 

    void Update()
    {
        if (Input.GetKeyDown(teclaInteragir))
        {
            foreach (var npc in npcs)
            {
                if (npc != null && npc.PlayerPerto)
                {
                    npc.Interact();
                    break;
                }
            }
        }
    }
}