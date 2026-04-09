using System.Collections;
using UnityEngine;
using TMPro;

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

    void Start()
    {
        dialoguePanel.SetActive(false);
        pressingE.SetActive(false);
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

                StartTyping();

                pressingE.SetActive(false);
            }
            else
            {
                if (dialogueText.text != falas[index])
                {
                    // COMPLETA TEXTO
                    StopTyping();
                    dialogueText.text = falas[index];
                }
                else
                {
                    index++;

                    if (index < falas.Length)
                    {
                        StartTyping();
                    }
                    else
                    {
                        FecharDialogo();
                    }
                }
            }
        }
    }

    void StartTyping()
    {
        StopTyping(); // 🔥 garante que não duplica
        typingCoroutine = StartCoroutine(EscreverTexto());
    }

    void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
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

    void FecharDialogo()
    {
        StopTyping();

        dialoguePanel.SetActive(false);
        dialogoAtivo = false;
        pressingE.SetActive(true);
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
            playerPerto = false;
            pressingE.SetActive(false);

            FecharDialogo(); // 🔥 fecha limpo
        }
    }
}