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
    private bool usarTypewriter = true;

    [Header("Audio")]
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip typingSound;

    [Header("Configuração de Som")]
    [Tooltip("Toca o som a cada X letras. Se for 1, toca sempre. Se for 3, pula 2 letras.")]
    [SerializeField] private int tocarSomACadaLetras = 3;

    public bool IsTyping => isTyping;

    public void Initialize(TMP_Text text, float speed)
    {
        dialogueText = text;
        velocidadeTexto = speed;
    }

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
        StopTypingSound();
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
            StopTypingSound();
        }
    }

    private IEnumerator EscreverTexto(int index)
    {
        isTyping = true;
        dialogueText.text = "";

        if (falas != null && index < falas.Length)
        {
            int letrasDigitadas = 0;

            foreach (char letra in falas[index])
            {
                dialogueText.text += letra;
                letrasDigitadas++;

                // ✅ Só toca o som se for a hora (ex: a cada 3 letras)
                // E o `!typingAudioSource.isPlaying` impede que os sons se acumulem
                if (typingAudioSource != null && typingSound != null)
                {
                    if (letrasDigitadas % tocarSomACadaLetras == 0 && !typingAudioSource.isPlaying)
                    {
                        typingAudioSource.PlayOneShot(typingSound);
                    }
                }

                // Não toca som em espaços em branco para não ficar estranho
                if (letra == ' ')
                {
                    letrasDigitadas--;
                }

                yield return new WaitForSeconds(velocidadeTexto);
            }
        }

        isTyping = false;
        StopTypingSound();
    }

    private void StopTypingSound()
    {
        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }
    }
}