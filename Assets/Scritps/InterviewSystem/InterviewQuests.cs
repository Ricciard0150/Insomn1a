using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InterviewQuests : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text characterNameText;

    [Header("Character")]
    [SerializeField] private Image characterImage;

    [Header("Config")]
    [SerializeField] private float textSpeed = 0.03f;

    [Header("Typing Sound")]
    [SerializeField] private AudioSource typingAudioSource;
    [SerializeField] private AudioClip typingClip;
    [Range(0f, 1f)][SerializeField] private float typingVolume = 0.5f;

    [Header("Final Dialogues (Fallback)")]
    [SerializeField] private DialogueLine[] victoryLines;
    [SerializeField] private DialogueLine[] defeatLines;

    [Header("References")]
    [SerializeField] private QuestData currentQuestData;

    private bool isRunning;
    private Coroutine currentDialogue;
    private bool isTextComplete = false;
    private bool waitingForNext = false;

    public bool IsRunning => isRunning;

    void Awake()
    {
        if (typingAudioSource != null && typingClip != null)
        {
            typingAudioSource.clip = typingClip;
            typingAudioSource.loop = true;
            typingAudioSource.playOnAwake = false;
            typingAudioSource.volume = typingVolume;
        }
    }

    void Update()
    {
        if (!isRunning) return;

        if (Input.GetButtonDown("Interact"))
        {
            if (!isTextComplete && !waitingForNext)
            {
                isTextComplete = true;
                StopTypingSound();
            }
            else if (isTextComplete && !waitingForNext)
            {
                waitingForNext = true;
            }
        }
    }

    private void StartTypingSound()
    {
        if (typingAudioSource == null || typingClip == null) return;
        if (!typingAudioSource.isPlaying) typingAudioSource.Play();
    }

    private void StopTypingSound()
    {
        if (typingAudioSource != null && typingAudioSource.isPlaying) typingAudioSource.Stop();
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        if (currentDialogue != null)
        {
            StopCoroutine(currentDialogue);
            currentDialogue = null;
        }

        currentDialogue = StartCoroutine(DisplayDialogue(lines));
    }

    private IEnumerator DisplayDialogue(DialogueLine[] lines)
    {
        isRunning = true;
        isTextComplete = false;
        waitingForNext = false;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        foreach (DialogueLine line in lines)
        {
            // ✅ TROCA O SPRITE E O NOME AQUI
            if (characterImage != null)
            {
                characterImage.sprite = line.characterSprite;
                characterImage.gameObject.SetActive(line.characterSprite != null);
            }

            if (characterNameText != null)
                characterNameText.text = line.characterName;

            dialogueText.text = "";
            isTextComplete = false;
            waitingForNext = false;

            StartTypingSound();

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

            StopTypingSound();

            if (!isTextComplete)
            {
                dialogueText.text = line.text;
                isTextComplete = true;
            }

            while (!waitingForNext)
            {
                yield return null;
            }

            waitingForNext = false;
            isTextComplete = false;
            yield return null;
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        StopTypingSound();

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

        StopTypingSound();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isRunning = false;
    }

    public void ShowVictoryDialogue()
    {
        if (victoryLines != null && victoryLines.Length > 0)
            StartDialogue(victoryLines);
        else if (currentQuestData != null && currentQuestData.victoryDialogue != null && currentQuestData.victoryDialogue.Length > 0)
            StartDialogue(currentQuestData.victoryDialogue);
    }

    public void ShowDefeatDialogue()
    {
        if (defeatLines != null && defeatLines.Length > 0)
            StartDialogue(defeatLines);
        else if (currentQuestData != null && currentQuestData.defeatDialogue != null && currentQuestData.defeatDialogue.Length > 0)
            StartDialogue(currentQuestData.defeatDialogue);
    }
}