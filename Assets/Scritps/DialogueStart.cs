using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class DialogueStart : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Config")]
    public string[] falas;
    public float velocidadeTexto = 0.05f;
    public KeyCode teclaInteragir = KeyCode.Q;
    public KeyCode teclaFechar = KeyCode.Space;

    private bool dialogoAtivo = false;
    private int index = 0;
    private Coroutine typingCoroutine;

    public TopDownMovement tdm;

    void Update()
    { 
            if (!dialogoAtivo)
            {
            tdm.canMove = false;
            dialogoAtivo = true;
                dialoguePanel.SetActive(true);
                index = 0;
                typingCoroutine = StartCoroutine(EscreverTexto());
            }
            else
            {
                if (Input.GetKeyDown(teclaInteragir))
                {
                    if (dialogueText.text != falas[index])
                    {
                        StopCoroutine(typingCoroutine);
                        dialogueText.text = falas[index];
                    }
                }
            }
            if (Input.GetKeyDown(teclaFechar) && dialogueText.text == falas[index])
            {
                dialoguePanel.SetActive(false);
                dialogoAtivo = true;
            tdm.canMove = true;
            }
        
    }

    IEnumerator EscreverTexto()
    {
        dialogueText.text = "";

        foreach (char letra in falas[index])
        {
            dialogueText.text += letra;
            yield return new WaitForSeconds(velocidadeTexto);
        }
    }
}