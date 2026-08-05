using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterviewQuests : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;

    [Header("Character")]
    public Image characterImage;

    [Header("Config")]
    public float textSpeed = 0.03f;

    public KeyCode skipKey = KeyCode.Space;

    [Header("Final Dialogues (opcional)")]
    public string[] victoryLines;
    public string[] defeatLines;

    private bool isRunning;
    private Coroutine currentDialogue;

    public bool IsRunning => isRunning;
    public void StartDialogue(string[] lines)
    {
        if (lines == null)
            return;

        DialogueLine[] converted =
            new DialogueLine[lines.Length];


        for (int i = 0; i < lines.Length; i++)
        {
            converted[i] = new DialogueLine();

            converted[i].text = lines[i];
            converted[i].characterSprite = null;
        }

        StartDialogue(converted);
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Diálogo vazio!");
            return;
        }
        

        if (currentDialogue != null)
            StopCoroutine(currentDialogue);

        currentDialogue =
            StartCoroutine(DisplayDialogue(lines));
    }
    IEnumerator DisplayDialogue(DialogueLine[] lines)
    {
        isRunning = true;
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        foreach (DialogueLine line in lines)
        {
            if (characterImage != null)
            {
                characterImage.sprite =
                    line.characterSprite;


                characterImage.gameObject.SetActive(
                    line.characterSprite != null
                );
            }
            dialogueText.text = "";
            bool skipped = false;
            foreach (char letter in line.text)
            {
                if (Input.GetKeyDown(skipKey))
                {
                    dialogueText.text = line.text;

                    skipped = true;

                    break;
                }
                dialogueText.text += letter;
                yield return new WaitForSeconds(textSpeed);
            }

            if (!skipped)
            {
                yield return new WaitUntil(
                    () => Input.GetButtonDown("Interact")
                );
            }

            yield return null;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isRunning = false;

        currentDialogue = null;
    }

    public void SkipAllDialogue()
    {
        if (currentDialogue != null)
        {
            StopCoroutine(currentDialogue);

            currentDialogue = null;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        isRunning = false;
    }
}