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

    public bool IsTyping => isTyping;

    public void Initialize(TMP_Text text, float speed)
    {
        dialogueText = text;
        velocidadeTexto = speed;
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