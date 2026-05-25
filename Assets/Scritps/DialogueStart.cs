using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueStart : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    [SerializeField] private Image imageSprite;
    [SerializeField] private Sprite[] sprites;


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
        tdm.canMove = false;

        typingCoroutine = StartCoroutine(EscreverTexto());
        ChangeSprite();

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
                    ChangeSprite();
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

    private void ChangeSprite()
    {
        if (index < sprites.Length)
        {
            imageSprite.sprite = sprites[index];
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