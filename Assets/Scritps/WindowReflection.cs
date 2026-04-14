using System.Collections;
using TMPro;
using UnityEngine;

public class WindowReflection : MonoBehaviour
{
    public GameObject window;
    public GameObject dialogueBox;
    public TMP_Text texto;

    [SerializeField] private KeyItem ki;

    public KeyCode teclaAvancar = KeyCode.E;
    public KeyCode teclaFechar = KeyCode.Q;

    public string[] lines;
    int index = 0;
    public float velocidade = 0.03f;

    private bool isColliding = false;
    private bool dialogoAtivo = false;
    private bool escrevendo = false;

    private void Start()
    {
        window.SetActive(false);
        dialogueBox.SetActive(false);
    }

    void Update()
    {
        // 🔥 Se já pegou a chave, destrói tudo
        if (ki != null && ki.playerHasKey)
        {
            Destroy(window);
            Destroy(this);
            return;
        }

        if (!isColliding) return;

        // 🔥 ABRIR AUTOMÁTICO AO PRESSIONAR E
        if (Input.GetKeyDown(teclaAvancar))
        {
            if (!dialogoAtivo)
            {
                window.SetActive(true);
                StartCoroutine(Sequencia());
                return;
            }

            // 🔥 CONTROLE DO TEXTO
            if (escrevendo)
            {
                StopAllCoroutines();
                texto.text = lines[index];
                escrevendo = false;
            }
            else
            {
                ProximaLinha();
            }
        }

        // 🔥 FECHAR COM Q
        if (dialogoAtivo && Input.GetKeyDown(teclaFechar))
        {
            FecharDialogo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            isColliding = true;

            // opcional: pegar o KeyItem do player automaticamente
            if (ki == null)
                ki = collision.GetComponent<KeyItem>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            isColliding = false;
        }
    }

    IEnumerator Sequencia()
    {
        dialogoAtivo = true;

        yield return new WaitForSeconds(1f); // menor delay pra ficar melhor

        dialogueBox.SetActive(true);

        index = 0;
        StartCoroutine(EscreverLinha());
    }

    void ProximaLinha()
    {
        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(EscreverLinha());
        }
        else
        {
            // acabou o diálogo
            FecharDialogo();
        }
    }

    void FecharDialogo()
    {
        StopAllCoroutines();

        dialogoAtivo = false;
        escrevendo = false;

        dialogueBox.SetActive(false);
        window.SetActive(false);
    }

    IEnumerator EscreverLinha()
    {
        texto.text = "";
        escrevendo = true;

        foreach (char letra in lines[index])
        {
            texto.text += letra;
            yield return new WaitForSeconds(velocidade);
        }

        escrevendo = false;
    }
}