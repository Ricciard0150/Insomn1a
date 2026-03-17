using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    [Header("Di�logo deste NPC")]
    [TextArea(3, 6)]
    public string[] falas;

    [Header("Configura��o de Intera��o")]
    public KeyCode teclaInteracao = KeyCode.E;

    private bool playerInRange = false;
    private bool dialogueStarted = false;

    private void Update()
    {
        if (playerInRange && !dialogueStarted && Input.GetKeyDown(teclaInteracao))
        {
            IniciarDialogo();
        }
    }

    private void IniciarDialogo()
    {
        if (TextManager.Instance != null)
        {
            dialogueStarted = true;
            TextManager.Instance.StartDialogue(falas);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out IStatusPlayer statuys))
            return;
        playerInRange = true;

    }
   
}