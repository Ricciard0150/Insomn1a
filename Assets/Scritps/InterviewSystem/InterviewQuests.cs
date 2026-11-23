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
    [Tooltip("AudioSource que toca o som de digitação (loop). Play On Awake DESMARCADO.")]
    [SerializeField] private AudioSource typingAudioSource;
    [Tooltip("Som de digitação (um 'tick' curto que será repetido em loop)")]
    [SerializeField] private AudioClip typingClip;
    [Tooltip("Volume do som de digitação")]
    [Range(0f, 1f)]
    [SerializeField] private float typingVolume = 0.5f;
    [Tooltip("Toca o som a cada N letras (1 = toda letra, 2 = a cada 2 letras, etc.)")]
    [SerializeField] private int letterIntervalForSound = 1;

    [Header("Final Dialogues")]
    [SerializeField] private DialogueLine[] victoryLines;
    [SerializeField] private DialogueLine[] defeatLines;

    [Header("References")]
    [SerializeField] private QuestData currentQuestData;

    private bool isRunning;
    private Coroutine currentDialogue;
    private int currentDialogueIndex;
    private bool isTextComplete = false;
    private bool waitingForNext = false;

    public bool IsRunning => isRunning;

    void Awake()
    {
        // ✅ Configura o AudioSource de digitação (loop, sem Play On Awake)
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
                StopTypingSound(); // ✅ para o som imediatamente ao pular
            }
            else if (isTextComplete && !waitingForNext)
            {
                waitingForNext = true;
            }
        }
    }

    // ✅ Liga o som de digitação
    private void StartTypingSound()
    {
        if (typingAudioSource == null || typingClip == null) return;
        if (!typingAudioSource.isPlaying)
        {
            typingAudioSource.Play();
        }
    }

    // ✅ Desliga o som de digitação
    private void StopTypingSound()
    {
        if (typingAudioSource != null && typingAudioSource.isPlaying)
        {
            typingAudioSource.Stop();
        }
    }

    public void StartDialogue(string[] lines, string[] names = null)
    {
        if (lines == null || lines.Length == 0) return;

        DialogueLine[] converted = new DialogueLine[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            converted[i] = new DialogueLine();
            converted[i].text = lines[i];
            converted[i].characterSprite = null;

            if (names != null && i < names.Length && !string.IsNullOrEmpty(names[i]))
                converted[i].characterName = names[i];
            else
                converted[i].characterName = "";
        }

        StartDialogue(converted);
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

            if (characterNameText != null)
                characterNameText.text = line.characterName;

            dialogueText.text = "";
            isTextComplete = false;
            waitingForNext = false;

            // ✅ Liga o som de digitação no início de cada linha
            StartTypingSound();

            for (int i = 0; i < line.text.Length; i++)
            {
                if (isTextComplete)
                {
                    dialogueText.text = line.text;
                    break;
                }

                dialogueText.text += line.text[i];

                // (opcional) a cada N letras — mas como o som está em loop, isso não afeta.
                yield return new WaitForSeconds(textSpeed);
            }

            // ✅ Terminou de digitar → para o som
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

            currentDialogueIndex++;
            waitingForNext = false;
            isTextComplete = false;
            yield return null;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // ✅ Garante que o som pare ao fechar o diálogo
        StopTypingSound();

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

        // ✅ Garante que o som pare ao pular
        StopTypingSound();

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
        {
            StartDialogue(victoryLines);
        }
        else if (currentQuestData != null && currentQuestData.victoryLines != null && currentQuestData.victoryLines.Length > 0)
        {
            DialogueLine[] converted = ConvertToDialogueLines(currentQuestData.victoryLines, currentQuestData.victoryNames);
            StartDialogue(converted);
        }
    }

    public void ShowDefeatDialogue()
    {
        if (defeatLines != null && defeatLines.Length > 0)
        {
            StartDialogue(defeatLines);
        }
        else if (currentQuestData != null && currentQuestData.defeatLines != null && currentQuestData.defeatLines.Length > 0)
        {
            DialogueLine[] converted = ConvertToDialogueLines(currentQuestData.defeatLines, currentQuestData.defeatNames);
            StartDialogue(converted);
        }
    }

    private DialogueLine[] ConvertToDialogueLines(string[] strings, string[] names)
    {
        DialogueLine[] result = new DialogueLine[strings.Length];
        for (int i = 0; i < strings.Length; i++)
        {
            result[i] = new DialogueLine();
            result[i].text = strings[i];
            result[i].characterSprite = null;

            if (names != null && i < names.Length)
                result[i].characterName = names[i];
            else
                result[i].characterName = "";
        }
        return result;
    }
}