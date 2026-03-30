using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Config")]
    public string[] falas;
    public float velocidadeTexto = 0.05f;
    public KeyCode teclaInteragir = KeyCode.E;

    private bool playerPerto = false;
    private bool dialogoAtivo = false;
    private int index = 0;
    private Coroutine typingCoroutine;

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(teclaInteragir))
        {
            if (!dialogoAtivo)
            {
                dialogoAtivo = true;
                dialoguePanel.SetActive(true);
                index = 0;
                typingCoroutine = StartCoroutine(EscreverTexto());
            }
            else
            {
                if (dialogueText.text != falas[index])
                {
                    StopCoroutine(typingCoroutine);
                    dialogueText.text = falas[index];
                }
                else
                {
                    index++;

                    if (index < falas.Length)
                    {
                        typingCoroutine = StartCoroutine(EscreverTexto());
                    }
                    else
                    {
                        dialoguePanel.SetActive(false);
                        dialogoAtivo = false;
                    }
                }
            }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
      
        if (other.GetComponent<IStatusPlayer>() != null)
        {
            playerPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IStatusPlayer>() != null)
        {
            playerPerto = false;
            dialoguePanel.SetActive(false);
            dialogoAtivo = false;
        }
    }
}