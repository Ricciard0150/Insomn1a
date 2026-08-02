using System;
using UnityEngine;


[Serializable]
public class DialogueLine
{
    public Sprite characterSprite;

    [TextArea(1, 3)]
    public string text;
}



[Serializable]
public class AnswerDialogue
{
    public DialogueLine[] dialogue;
}



[Serializable]
public class QuestionRPG
{
    public string question;

    public string[] options;

    public int correctAnswer;


    [Header("Feedback")]
    public string correctFeedback;
    public string wrongFeedback;


    [Header("Dialogo por resposta")]
    public AnswerDialogue[] optionDialogues;
}
public class InterviewSystem
{
    public event Action<QuestionRPG, int> OnRoundChanged;
    public event Action<bool, int, int> OnFinished;
    public event Action<string> OnFeedback;

    private QuestionRPG[] questions;

    private int maxLives;
    private int minCorrect;

    private int currentLives;
    private int points;
    private int round;

    private bool isActive;

    public bool IsActive => isActive;

    public InterviewSystem(QuestionRPG[] questions, int maxLives, int minCorrect)
    {
        this.questions = questions;
        this.maxLives = maxLives;
        this.minCorrect = minCorrect;
    }

    public void Start()
    {
        isActive = true;

        round = 0;
        points = 0;
        currentLives = maxLives;

        NextRound();
    }

    void NextRound()
    {
        if (round >= questions.Length)
        {
            Finish();
            return;
        }

        QuestionRPG question = questions[round];

        OnRoundChanged?.Invoke(question, question.options.Length);
    }

    public int GetCurrentRound()
    {
        return round;
    }

    public void ProcessChoice(int choice)
    {
        if (!isActive)
            return;

        QuestionRPG question = questions[round];

        bool correct = choice == question.correctAnswer;

        if (correct)
            points++;
        else
            currentLives--;

        string feedback = correct
            ? question.correctFeedback
            : question.wrongFeedback + $"\n Vidas: {currentLives}";

        OnFeedback?.Invoke(feedback);

        if (currentLives <= 0)
        {
            Finish();
            return;
        }

        round++;

        NextRound();
    }

    void Finish()
    {
        isActive = false;

        bool victory = points >= minCorrect && currentLives > 0;

        OnFinished?.Invoke(victory, points, currentLives);
    }
}