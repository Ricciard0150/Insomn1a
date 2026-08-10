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

    [Header("Final Dialogues (opcional)")]
    public string[] victoryLines;
    public string[] defeatLines;

    [Header("References")]
    public QuestData currentQuestData;

    private bool isRunning;
    private Coroutine currentDialogue;
    private int currentDialogueIndex;
    private bool isTextComplete = false;
    private bool waitingForNext = false;

    public bool IsRunning => isRunning;
    public int CurrentDialogueIndex => currentDialogueIndex;

    void Update()
    {
        if (!isRunning) return;

        if (Input.GetButtonDown("Interact"))
        {
            if (!isTextComplete && !waitingForNext)
            {
                isTextComplete = true;
            }
            else if (isTextComplete && !waitingForNext)
            {
                waitingForNext = true;
            }
        }
    }

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0)
        {
            Debug.LogWarning("Diálogo vazio!");
            return;
        }

        DialogueLine[] converted = new DialogueLine[lines.Length];
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
        {
            StopCoroutine(currentDialogue);
            currentDialogue = null;
        }

        currentDialogue = StartCoroutine(DisplayDialogue(lines));
    }

    IEnumerator DisplayDialogue(DialogueLine[] lines)
    {
        isRunning = true;
        currentDialogueIndex = 0;
        isTextComplete = false;
        waitingForNext = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        foreach (DialogueLine line in lines)
        {
            if (characterImage != null)
            {
                characterImage.sprite = line.characterSprite;
                characterImage.gameObject.SetActive(line.characterSprite != null);
            }

            dialogueText.text = "";
            isTextComplete = false;
            waitingForNext = false;

            for (int i = 0; i < line.text.Length; i++)
            {
                if (isTextComplete)
                {
                    dialogueText.text = line.text;
                    break;
                }

                dialogueText.text += line.text[i];
                yield return new WaitForSeconds(textSpeed);
            }

            if (!isTextComplete)
            {
                dialogueText.text = line.text;
                isTextComplete = true;
            }

            while (!waitingForNext)
            {
                yield return null;
            }

            currentDialogueIndex++;
            waitingForNext = false;
            isTextComplete = false;
            yield return null;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isRunning = false;
        currentDialogue = null;
        currentDialogueIndex = 0;
        isTextComplete = false;
        waitingForNext = false;
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
        currentDialogueIndex = 0;
        isTextComplete = false;
        waitingForNext = false;
    }

    public void ShowVictoryDialogue()
    {
        if (victoryLines != null && victoryLines.Length > 0)
            StartDialogue(victoryLines);
        else if (currentQuestData != null && currentQuestData.victoryLines.Length > 0)
            StartDialogue(currentQuestData.victoryLines);
    }

    public void ShowDefeatDialogue()
    {
        if (defeatLines != null && defeatLines.Length > 0)
            StartDialogue(defeatLines);
        else if (currentQuestData != null && currentQuestData.defeatLines.Length > 0)
            StartDialogue(currentQuestData.defeatLines);
    }
}