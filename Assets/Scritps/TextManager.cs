using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] public GameObject dialoguePanel;
    [SerializeField] GameObject pressingE;
    public TMP_Text dialogueText;
    [SerializeField] private Image images;
    [SerializeField] private Sprite[] sprites;

    [Header("Config")]
    public string[] falas;
    public float velocidadeTexto = 0.05f;
    public KeyCode teclaInteragir = KeyCode.E;
    public KeyCode closeKey = KeyCode.Q;

    private bool playerPerto = false;
    private bool dialogoAtivo = false;
    private int index = 0;
    private Coroutine typingCoroutine;
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
                tdm.canMove = false;
                dialogoAtivo = true;
                dialoguePanel.SetActive(true);
                index = 0;

                
                ChangeSprite();
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
                        ChangeSprite();
                        StartTyping();
                    }
                    else 
                    {
                        FecharDialogo();
                        tdm.canMove = true;
                    }
                }
            }
        }
        
    }

    void ChangeSprite()
    {
        if (index < sprites.Length)
        {
            images.sprite = sprites[index];
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
    }
        
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() != null)
        {
            pressingE.SetActive(true);
            playerPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IStatusPlayer>() == null)
            return;
        else
        {
            playerPerto = false;
            pressingE.SetActive(false);
        }
    }
}