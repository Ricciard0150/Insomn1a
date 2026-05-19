using System.Collections;
using UnityEngine;
using TMPro;

public class ThoughtsDialogue : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Config")]
    public string[] falas;
    public float velocidadeTexto = 0.05f;
    public KeyCode teclaInteragir = KeyCode.E;
    public KeyCode teclaFechar = KeyCode.Q;

    private int index = 0;
    private bool terminouFrase = false;
    private bool dialogoAtivo = false;
    private bool jaAtivado = false; // 👈 NOVO

    private Coroutine typingCoroutine;

    public TopDownMovement tdm;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!dialogoAtivo) return;

        // 🔥 FECHAR COM Q
        if (Input.GetKeyDown(teclaFechar))
        {
            EncerrarDialogo();
            return;
        }

        if (Input.GetKeyDown(teclaInteragir))
        {
            if (!terminouFrase)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                dialogueText.text = falas[index];
                terminouFrase = true;
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
                    EncerrarDialogo();
                    Debug.Log("aaa");
        

                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (jaAtivado) return; // 👈 impede repetir

        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            jaAtivado = true;
            IniciarDialogo();
        }
    }

    void IniciarDialogo()
    {
        tdm.canMove = false;
        dialogoAtivo = true;
        dialoguePanel.SetActive(true);

        index = 0;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(EscreverTexto());
    }

    void EncerrarDialogo()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialoguePanel.SetActive(false);
        dialogoAtivo = false;

        // 💥 DESTRÓI O OBJETO DEPOIS DE USAR
        Destroy(gameObject);
        tdm.canMove = true;

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