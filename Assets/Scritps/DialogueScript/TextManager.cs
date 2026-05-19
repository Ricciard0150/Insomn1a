using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject pressingE;

    [SerializeField] private TMP_Text dialogueText;

    [Header("Imagem do Personagem")]
    [SerializeField] private Image personagemImage;
    [SerializeField] private Sprite[] spritesDialogo;

    [Header("Config")]
    [TextArea]
    public string[] falas;

    public float velocidadeTexto = 0.05f;

    public KeyCode teclaInteragir = KeyCode.E;

    private bool playerPerto = false;
    private bool dialogoAtivo = false;

    private int index = 0;

    private Coroutine typingCoroutine;

    [Header("Player")]
    public TopDownMovement tdm;

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

                tdm.canMove = false;

                dialoguePanel.SetActive(true);

                pressingE.SetActive(false);

                index = 0;

                AtualizarSprite();

                StartTyping();
            }

            else
            {
                if (dialogueText.text != falas[index])
                {
                    StopTyping();

                    dialogueText.text = falas[index];
                }
                else
                {
                    index++;

                    if (index < falas.Length)
                    {
                        AtualizarSprite();

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

    void AtualizarSprite()
    {
        if (index < spritesDialogo.Length)
        {
            personagemImage.sprite = spritesDialogo[index];
        }
    }

    void StartTyping()
    {
        StopTyping();

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

        tdm.canMove = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() != null)
        {
            playerPerto = true;

            pressingE.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() != null)
        {
            playerPerto = false;

            pressingE.SetActive(false);

            FecharDialogo();
        }
    }
}