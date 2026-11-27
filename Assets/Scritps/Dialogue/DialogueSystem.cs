using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private Image characterImage;

    [Header("Player")]
    [SerializeField] private TopDownMovement playerMovement; // ✅ Referência para travar o player

    [Header("Typing")]
    [SerializeField] private float textSpeed = 0.05f;

    public bool DialogoAtivo { get; private set; } = false;

    private string[] currentLines;
    private Sprite[] currentSprites;
    private string[] currentNames;
    private bool[] currentShowSprite;
    private string npcFallbackName;

    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingRoutine;

    public void SetNPCDialogue(string[] lines, Sprite[] sprites, string[] names, string fallbackName, bool[] showSprite = null)
    {
        currentLines = lines;
        currentSprites = sprites;
        currentNames = names;
        npcFallbackName = fallbackName;
        currentShowSprite = showSprite;
    }

    public void StartDialogue()
    {
        Debug.Log($"[DS] Start. ID={GetInstanceID()} linhas={currentLines?.Length}");

        if (currentLines == null || currentLines.Length == 0) return;

        DialogoAtivo = true;
        currentIndex = 0;
        dialoguePanel.SetActive(true);

        // ✅ Trava o player ao abrir o diálogo
        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        ShowLine(currentIndex);
    }

    public void AdvanceDialogue()
    {
        if (!DialogoAtivo) return;

        if (isTyping)
        {
            StopAllCoroutines();   // ⚠️ para TUDO, não só a digitação
            dialogueText.text = currentLines[currentIndex];
            isTyping = false;
            return;
        }

        currentIndex++;

        if (currentIndex < currentLines.Length)
        {
            ShowLine(currentIndex);
        }
        else
        {
            CloseDialogue();
        }
    }

    private void ShowLine(int index)
    {
        // 1. Nome
        if (characterNameText != null)
        {
            string nome = npcFallbackName;
            if (currentNames != null && index < currentNames.Length && !string.IsNullOrEmpty(currentNames[index]))
                nome = currentNames[index];

            characterNameText.text = nome;
        }

        // 2. Sprite
        if (characterImage != null)
        {
            bool deveMostrar = true;

            if (currentShowSprite != null && index < currentShowSprite.Length)
            {
                deveMostrar = currentShowSprite[index];
            }

            if (!deveMostrar || currentSprites == null || index >= currentSprites.Length || currentSprites[index] == null)
            {
                characterImage.gameObject.SetActive(false);
            }
            else
            {
                characterImage.sprite = currentSprites[index];
                characterImage.gameObject.SetActive(true);
            }
        }

        // 3. Texto
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeText(currentLines[index]));
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    private void CloseDialogue()
    {
        DialogoAtivo = false;
        dialoguePanel.SetActive(false);

        if (characterImage != null)
            characterImage.gameObject.SetActive(false);

        // ✅ Destrava o player ao fechar o diálogo
        if (playerMovement != null)
            playerMovement.SetCanMove(true);
    }
}