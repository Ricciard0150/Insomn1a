using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueStart : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Config")]
    public string[] falas;
    public float velocidadeTexto = 0.05f;
    public KeyCode teclaInteragir = KeyCode.E;

    private int index = 0;
    private bool terminouFrase = false;
    private Coroutine typingCoroutine;

    public TopDownMovement tdm;
    void Start()
    {
        dialoguePanel.SetActive(true);
        index = 0;

        typingCoroutine = StartCoroutine(EscreverTexto());
    }

    void Update()
    {
        if (Input.GetKeyDown(teclaInteragir))
        {
            if (!terminouFrase)
            {
                // 👉 completa a frase instantaneamente
                StopCoroutine(typingCoroutine);
                dialogueText.text = falas[index];
                terminouFrase = true;
            }
            else
            {
                // 👉 vai pro próximo element
                index++;

                if (index < falas.Length)
                {
                    typingCoroutine = StartCoroutine(EscreverTexto());
                }
                else
                {
                    // 👉 acabou tudo
                    dialoguePanel.SetActive(false);
                    tdm.canMove = true;
                }
            }
        }
    }

    IEnumerator EscreverTexto()
    {
        terminouFrase = false;
        dialogueText.text = "";

        foreach (char letra in falas[index])
        {
            dialogueText.text += letra;
            yield return new WaitForSeconds(velocidadeTexto);
        }

        terminouFrase = true;
    }
}