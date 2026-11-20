using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class NightmareLines : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textDisplay;
    [SerializeField] private CanvasGroup dialogueCanvasGroup; // opcional, recomendado

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float slideOffset = 80f;
    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Settings")]
    [SerializeField] private string advanceButton = "Interact";
    [SerializeField] private float advanceCooldown = 0.15f;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerMovement; // arraste o script de movimento (ex: PlayerMovement)

    private string[] currentLines;
    private int currentIndex = 0;
    private bool isFinished = true;
    private bool isOpen = false;
    private float lastAdvanceTime;
    private Vector2 originalAnchoredPos;
    private RectTransform boxRect;

    public bool DialogoAtivo => isOpen;

    private void Awake()
    {
        if (dialogueBox == null) return;

        boxRect = dialogueBox.GetComponent<RectTransform>();
        if (boxRect != null) originalAnchoredPos = boxRect.anchoredPosition;

        // Garante que exista um CanvasGroup para animar
        if (dialogueCanvasGroup == null)
        {
            dialogueCanvasGroup = dialogueBox.GetComponent<CanvasGroup>();
            if (dialogueCanvasGroup == null)
                dialogueCanvasGroup = dialogueBox.AddComponent<CanvasGroup>();
        }

        // Começa escondido
        dialogueCanvasGroup.alpha = 0f;
        dialogueBox.SetActive(false);
        if (textDisplay != null) textDisplay.text = "";
    }

    public void StartDialogue(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentIndex = 0;
        isFinished = false;

        // Limpa texto ANTES de abrir (resolve o "New Text" aparecendo)
        if (textDisplay != null) textDisplay.text = "";

        if (!isOpen)
            StartCoroutine(OpenRoutine());
        else
            ShowLineImmediate();
    }

    private IEnumerator OpenRoutine()
    {
        isOpen = true;
        dialogueBox.SetActive(true);

        // Bloqueia movimento
        SetPlayerMovement(false);

        // Reset de estado
        dialogueCanvasGroup.alpha = 0f;
        if (boxRect != null)
            boxRect.anchoredPosition = originalAnchoredPos + Vector2.down * slideOffset;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = easeCurve.Evaluate(Mathf.Clamp01(t / fadeDuration));

            dialogueCanvasGroup.alpha = p;
            if (boxRect != null)
                boxRect.anchoredPosition = Vector2.Lerp(
                    originalAnchoredPos + Vector2.down * slideOffset,
                    originalAnchoredPos, p);

            yield return null;
        }

        dialogueCanvasGroup.alpha = 1f;
        if (boxRect != null) boxRect.anchoredPosition = originalAnchoredPos;

        ShowLineImmediate();
    }

    private void ShowLineImmediate()
    {
        if (currentLines == null || currentIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        // Texto aparece de uma vez (sem typewriter)
        if (textDisplay != null)
            textDisplay.text = currentLines[currentIndex];
    }

    private void Update()
    {
        if (!isOpen || isFinished) return;
        if (Time.unscaledTime - lastAdvanceTime < advanceCooldown) return;

        if (Input.GetButtonDown(advanceButton))
        {
            lastAdvanceTime = Time.unscaledTime;
            Advance();
        }
    }

    private void Advance()
    {
        currentIndex++;
        if (currentIndex >= currentLines.Length)
            EndDialogue();
        else
            ShowLineImmediate();
    }

    private void EndDialogue()
    {
        if (!isOpen) return;
        StartCoroutine(CloseRoutine());
    }

    private IEnumerator CloseRoutine()
    {
        float t = 0f;
        float startAlpha = dialogueCanvasGroup.alpha;
        Vector2 startPos = boxRect != null ? boxRect.anchoredPosition : Vector2.zero;
        Vector2 endPos = originalAnchoredPos + Vector2.down * slideOffset;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = easeCurve.Evaluate(Mathf.Clamp01(t / fadeDuration));

            dialogueCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, p);
            if (boxRect != null)
                boxRect.anchoredPosition = Vector2.Lerp(startPos, endPos, p);

            yield return null;
        }

        dialogueCanvasGroup.alpha = 0f;
        dialogueBox.SetActive(false);

        if (textDisplay != null) textDisplay.text = "";

        isOpen = false;
        isFinished = true;

        // Libera movimento
        SetPlayerMovement(true);
    }

    private void SetPlayerMovement(bool canMove)
    {
        if (playerMovement == null) return;

        // Tenta setar propriedade/field "canMove" via reflection (compatível com seu sistema)
        var type = playerMovement.GetType();
        var prop = type.GetProperty("canMove");
        if (prop != null && prop.CanWrite) { prop.SetValue(playerMovement, canMove); return; }

        var field = type.GetField("canMove");
        if (field != null) { field.SetValue(playerMovement, canMove); }
    }

    public bool IsFinished() => isFinished;

    public void ShowText(string text, bool autoClose = true)
    {
        dialogueBox.SetActive(true);
        dialogueCanvasGroup.alpha = 1f;
        if (boxRect != null) boxRect.anchoredPosition = originalAnchoredPos;

        if (textDisplay != null) textDisplay.text = text;

        if (autoClose) StartCoroutine(AutoClose());
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSecondsRealtime(2f);
        Close();
    }

    public void Close()
    {
        if (isOpen) { EndDialogue(); return; }
        dialogueCanvasGroup.alpha = 0f;
        dialogueBox.SetActive(false);
        if (textDisplay != null) textDisplay.text = "";
    }
}