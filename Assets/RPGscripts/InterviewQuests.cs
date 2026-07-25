using System.Collections;
using UnityEngine;
using TMPro;

public class InterviewQuests : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;

    [Header("Config")]
    public float textSpeed = 0.03f;
    public KeyCode skipKey = KeyCode.Space;
    public KeyCode continueKey = KeyCode.E;

    [Header("Dialogue Lines")]
    public string[] victoryLines;
    public string[] defeatLines;

    private bool isRunning;
    private Coroutine currentDialogue;
    private bool isSkipping;

    public bool IsRunning => isRunning;

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Diálogo vazio!");
            return;
        }

        if (currentDialogue != null)
            StopCoroutine(currentDialogue);

        currentDialogue = StartCoroutine(DisplayDialogue(lines));
    }

    IEnumerator DisplayDialogue(string[] lines)
    {
        isRunning = true;
        isSkipping = false;
        dialoguePanel.SetActive(true);

        foreach (string line in lines)
        {
            dialogueText.text = "";

            foreach (char letter in line)
            {
                if (Input.GetKeyDown(skipKey))
                {
                    isSkipping = true;
                    dialogueText.text = line;
                    break;
                }

                dialogueText.text += letter;
                yield return new WaitForSeconds(textSpeed);
            }

            isSkipping = false;
            yield return new WaitUntil(() => Input.GetKeyDown(continueKey));
            yield return null;
        }

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

        dialoguePanel.SetActive(false);
        isRunning = false;
    }
}