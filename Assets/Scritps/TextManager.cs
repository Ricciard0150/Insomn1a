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


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);


        dialogueBox.SetActive(false);
    }


    void Update()
    {
        if (dialogueBox.activeSelf && Input.GetKeyDown(KeyCode.E))
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
                    dialogueBox.SetActive(false);
                }
            }
        }
    }


    public void StartDialogue(string[] dialogueLines)
    {
        lines = dialogueLines;
        currentLine = 0;
        dialogueBox.SetActive(true);
        StartCoroutine(TypeLine(lines[currentLine]));
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
