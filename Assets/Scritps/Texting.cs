using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogue;
    private bool playerNear;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            TextManager.Instance.StartDialogue(dialogue);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}