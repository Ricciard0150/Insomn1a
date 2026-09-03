using UnityEngine;
using TMPro;
using System.Collections;

public class NightmareLines: MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textDisplay;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private string advanceButton = "Interact";

    private string[] currentLines;
    private int currentIndex = 0;
    private bool isTyping = false;
    private bool isFinished = false;

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentIndex = 0;
        isFinished = false;
        dialogueBox.SetActive(true);
        ShowLine();
    }

    private void ShowLine()
    {
        if (currentIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TypeLine(currentLines[currentIndex]));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textDisplay.text = "";

        foreach (char letter in line)
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void Update()
    {
        if (!dialogueBox.activeSelf) return;

        if (Input.GetButtonDown(advanceButton))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                textDisplay.text = currentLines[currentIndex];
                isTyping = false;
            }
            else
            {
                currentIndex++;
                ShowLine();
            }
        }
    }

    private void EndDialogue()
    {
        dialogueBox.SetActive(false);
        isFinished = true;
    }

    public bool IsFinished() => isFinished;

    public void ShowText(string text, bool autoClose = true)
    {
        dialogueBox.SetActive(true);
        textDisplay.text = text;

        if (autoClose)
            StartCoroutine(AutoClose());
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(2f);
        dialogueBox.SetActive(false);
    }

    public void Close()
    {
        dialogueBox.SetActive(false);
    }
}