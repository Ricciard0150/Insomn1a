using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class TextManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject dialoguePanel;
    [SerializeField] GameObject pressingE;
    public TMP_Text dialogueText;

    [Header("Config")]
    public string[] falas;
    public float velocidadeTexto = 0.05f;
    public KeyCode teclaInteragir = KeyCode.E;

    private bool playerPerto = false;
    private bool dialogoAtivo = false;
    private int index = 0;
    private Coroutine typingCoroutine;

    private void Start()
    {
        
    }
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
                pressingE.SetActive(false);
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
                        pressingE.SetActive(true);
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
            pressingE.SetActive(true);
            playerPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IStatusPlayer>() != null)
        {
            pressingE.SetActive(false);
            playerPerto = false;
            dialoguePanel.SetActive(false);
            dialogoAtivo = false;
        }
    }
}