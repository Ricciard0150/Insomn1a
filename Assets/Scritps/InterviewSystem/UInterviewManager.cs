using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class UInterviewManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text questionText;
    public TMP_Text[] optionsText;
    public TMP_Text feedbackText;

    [Header("Config")]
    public float textSpeed = 0.03f;
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    [Header("Visual Effects")]
    public float selectedScale = 1.1f;
    public bool enableScale = true;
    public string selectedPrefix = "⭐ ";
    public string normalPrefix = "▶ ";

    [Header("Transitions")]
    public bool hideOptionsOnAnswer = true;
    public bool hideQuestionOnAnswer = true;
    public float fadeSpeed = 0.3f;

    [Header("Navigation")]
    public float inputCooldown = 0.12f;

    private int selectedOption;
    private int totalOptions;
    private bool canChoose;
    private bool isTyping;
    private Action<int> onChoice;
    private Coroutine currentCoroutine;
    private bool canNavigate = true;

    void Update()
    {
        if (!canChoose) return;
        HandleNavigation();
    }

    void HandleNavigation()
    {
        if (!canNavigate) return;

        if (Input.GetButtonDown("Interact"))
        {
            Select();
            return;
        }

        
    if (Input.GetButtonDown("Up"))
    {
        Navigate(-1);
        StartCoroutine(NavigationCooldown());
    }
    else if (Input.GetButtonDown("Down"))
    {
        Navigate(1);
        StartCoroutine(NavigationCooldown());
    }
    }

    IEnumerator NavigationCooldown()
    {
        canNavigate = false;
        yield return new WaitForSeconds(inputCooldown);
        canNavigate = true;
    }

    public void ShowQuestion(QuestionRPG question, int totalOpt, Action<int> callback)
    {
        onChoice = callback;
        selectedOption = 0;
        totalOptions = Mathf.Min(totalOpt, optionsText.Length);
        canChoose = false;
        canNavigate = true;

        feedbackText.text = "";
        feedbackText.color = Color.white;

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(DisplayQuestion(question));
    }

    IEnumerator DisplayQuestion(QuestionRPG question)
    {
        questionText.gameObject.SetActive(true);
        questionText.text = "";

        foreach (var txt in optionsText)
        {
            txt.text = "";
            txt.gameObject.SetActive(false);
            txt.transform.localScale = Vector3.one;
            txt.color = normalColor;
        }

        yield return TypeWriter(questionText, question.question);

        for (int i = 0; i < totalOptions; i++)
        {
            optionsText[i].text = (i == 0 ? selectedPrefix : normalPrefix) + question.options[i];
            optionsText[i].gameObject.SetActive(true);
            optionsText[i].color = i == 0 ? selectedColor : normalColor;

            if (enableScale)
                optionsText[i].transform.localScale = i == 0 ? Vector3.one * selectedScale : Vector3.one;
        }

        canChoose = true;
    }

    void Navigate(int direction)
    {
        optionsText[selectedOption].transform.localScale = Vector3.one;
        optionsText[selectedOption].color = normalColor;

        string text = optionsText[selectedOption].text;
        if (text.StartsWith(selectedPrefix))
            text = text.Substring(selectedPrefix.Length);
        else if (text.StartsWith(normalPrefix))
            text = text.Substring(normalPrefix.Length);
        optionsText[selectedOption].text = normalPrefix + text;

        int newOption = selectedOption + direction;
        if (newOption < 0) newOption = totalOptions - 1;
        else if (newOption >= totalOptions) newOption = 0;

        selectedOption = newOption;

        optionsText[selectedOption].color = selectedColor;
        if (enableScale)
            optionsText[selectedOption].transform.localScale = Vector3.one * selectedScale;

        string newText = optionsText[selectedOption].text;
        if (newText.StartsWith(selectedPrefix))
            newText = newText.Substring(selectedPrefix.Length);
        else if (newText.StartsWith(normalPrefix))
            newText = newText.Substring(normalPrefix.Length);
        optionsText[selectedOption].text = selectedPrefix + newText;
    }

    void Select()
    {
        if (!canChoose) return;
        canChoose = false;

        if (hideQuestionOnAnswer)
            StartCoroutine(FadeOut(questionText));

        if (hideOptionsOnAnswer)
            StartCoroutine(FadeOutOptions());

        onChoice?.Invoke(selectedOption);
    }

    IEnumerator FadeOutOptions()
    {
        foreach (var txt in optionsText)
        {
            if (txt.gameObject.activeSelf)
            {
                yield return StartCoroutine(FadeOut(txt));
            }
        }
    }

    IEnumerator FadeOut(TMP_Text text)
    {
        float elapsed = 0;
        Color originalColor = text.color;

        while (elapsed < fadeSpeed)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / fadeSpeed);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.gameObject.SetActive(false);
        text.color = originalColor;
    }

    IEnumerator TypeWriter(TMP_Text text, string phrase)
    {
        isTyping = true;
        text.text = "";

        foreach (char letter in phrase)
        {
            text.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    public void ShowFeedback(string message)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(TypeWriter(feedbackText, message));
    }

    public void ShowFeedback(string message, bool isCorrect)
    {
        feedbackText.color = isCorrect ? correctColor : wrongColor;
        ShowFeedback(message);
        StartCoroutine(ResetFeedbackColor());
    }

    IEnumerator ResetFeedbackColor()
    {
        yield return new WaitForSeconds(2f);
        feedbackText.color = Color.white;
    }

    public void ResetUI()
    {
        questionText.gameObject.SetActive(true);
        questionText.color = Color.white;
        questionText.text = "";

        foreach (var txt in optionsText)
        {
            txt.gameObject.SetActive(true);
            txt.color = normalColor;
            txt.transform.localScale = Vector3.one;
            txt.text = "";
        }

        feedbackText.text = "";
        feedbackText.color = Color.white;
    }
}