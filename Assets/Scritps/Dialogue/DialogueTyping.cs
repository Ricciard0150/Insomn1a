using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueTyping : MonoBehaviour
{
    private TMP_Text dialogueText;
    private string[] falas;
    private float velocidadeTexto = 0.05f;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool usarTypewriter = true; // ✅ novo

    public bool IsTyping => isTyping;

    // ✅ Mantém a assinatura antiga (2 args) para não quebrar nada
    public void Initialize(TMP_Text text, float speed)
    {
        dialogueText = text;
        velocidadeTexto = speed;
    }

    // ✅ Nova sobrecarga (3 args) usada pelo DialogueNightmare
    public void Initialize(TMP_Text text, float speed, bool useTypewriter)
    {
        Initialize(text, speed);
        usarTypewriter = useTypewriter;
    }

    public void UpdateDialogue(string[] newFalas)
    {
        falas = newFalas;
    }

    public void StartTyping(int index)
    {
        StopTyping();

        if (falas == null || index >= falas.Length)
            return;

        // ✅ Se typewriter desligado, mostra tudo de uma vez
        if (!usarTypewriter)
        {
            if (dialogueText != null)
                dialogueText.text = falas[index];
            isTyping = false;
            return;
        }

        typingCoroutine = StartCoroutine(EscreverTexto(index));
    }

    public void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        isTyping = false;
    }

    public void CompleteTyping(int index)
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;

            if (dialogueText != null && falas != null && index < falas.Length)
                dialogueText.text = falas[index];

            isTyping = false;
        }
    }

    private IEnumerator EscreverTexto(int index)
    {
        isTyping = true;
        dialogueText.text = "";

        if (falas != null && index < falas.Length)
        {
            foreach (char letra in falas[index])
            {
                dialogueText.text += letra;
                yield return new WaitForSeconds(velocidadeTexto);
            }
        }

        isTyping = false;
    }
}