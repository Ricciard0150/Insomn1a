using UnityEngine;

public class TextManager : MonoBehaviour
{ 
    [SerializeField] private InteractionDetector[] npcs; 

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
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