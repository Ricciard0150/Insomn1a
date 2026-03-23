using TMPro;
using UnityEngine;
using System.Collections;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance;

    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public float typingSpeed = 0.04f;

    private string[] lines;
    private int currentLine;
    private bool isTyping;
    private bool dialogueActive;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialogueBox.SetActive(false);
    }

    void Update()
    {
        if (!dialogueActive) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = lines[currentLine];
                isTyping = false;
            }
            else
            {
                currentLine++;

                if (currentLine < lines.Length)
                {
                    StartCoroutine(TypeLine(lines[currentLine]));
                }
                else
                {
                    EndDialogue();
                }
            }
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        StopAllCoroutines();

        lines = dialogueLines;
        currentLine = 0;
        isTyping = false;
        dialogueActive = true;

        dialogueBox.SetActive(false); // força reset
        dialogueText.text = "";
        dialogueBox.SetActive(true);

        StartCoroutine(TypeLine(lines[currentLine]));
    }

    void EndDialogue()
    {
        StopAllCoroutines();
        isTyping = false;
        dialogueActive = false;
        dialogueBox.SetActive(false);
    }

    IEnumerator TypeLine(string line)
    {
        dialogueText.text = "";
        isTyping = true;

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}