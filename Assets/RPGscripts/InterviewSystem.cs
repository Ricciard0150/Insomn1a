using System;

[Serializable]
public class QuestionRPG
{
    public string question;
    public string[] options;
    public int correctAnswer;
    public string correctFeedback;
    public string wrongFeedback;
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

        var question = questions[round];
        OnRoundChanged?.Invoke(question, question.options.Length);
    }

    public int GetCurrentRound()
    {
        return round;
    }

    public void ProcessChoice(int choice)
    {
        if (!isActive) return;

        var question = questions[round];
        bool isCorrect = choice == question.correctAnswer;

        if (isCorrect)
            points++;
        else
            currentLives--;

 
        string feedback = isCorrect ? question.correctFeedback :
                         question.wrongFeedback + $"\n❤️ Lives: {currentLives}";
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