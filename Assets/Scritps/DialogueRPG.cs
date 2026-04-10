using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueRPG : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    public float velocidadeTexto = 0.05f;
    public KeyCode teclaAvancar = KeyCode.E;
    public KeyCode teclaFechar = KeyCode.Space;

    private string[] falas;
    private int index;
    private bool terminouFrase;
    private bool dialogoAtivo;
    private Coroutine coroutine;

    
   [SerializeField] public GameObject target;

    void Update()

    {
 
        if (!dialogoAtivo) return;

        // 🔥 FECHAR DIRETO COM SPACE
        if (Input.GetKeyDown(teclaFechar))
        {
            Encerrar();
            return;
        }

        // Avançar diálogo
        if (Input.GetKeyDown(teclaAvancar))
        {
            if (!terminouFrase)
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);

                dialogueText.text = falas[index];
                terminouFrase = true;
            }
            else
            {
                index++;

                if (index < falas.Length)
                {
                    coroutine = StartCoroutine(Escrever());
                }
                else
                {
                    Encerrar();
                   
                }
            }
        }
    }

    public void IniciarDialogo(string[] novasFalas)
    {
        if (novasFalas == null || novasFalas.Length == 0)
        {
            Debug.LogWarning("Sem falas!");
            return;
        }

        falas = novasFalas;
        index = 0;

        dialoguePanel.SetActive(true);
        dialogoAtivo = true;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(Escrever());


        InteractionRPGnormal rpg = target.GetComponent<InteractionRPGnormal>();
        if (rpg == null)
        {
            Debug.Log("oebs");
        }
        else
        {
            Destroy(rpg);
        }
    }

    IEnumerator Escrever()
    {
        terminouFrase = false;
        dialogueText.text = "";

        foreach (char c in falas[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(velocidadeTexto);
        }

        terminouFrase = true;
    }

    void Encerrar()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);

        dialoguePanel.SetActive(false);
        dialogoAtivo = false;
        
    }

    public bool EstaRodando()
    {
        return dialogoAtivo;
    }
}