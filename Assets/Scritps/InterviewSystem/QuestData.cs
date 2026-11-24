using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public Sprite characterSprite;
    public string characterName;
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

[Serializable]
public class QuestData
{
    public string questId;
    public string questName;
    public string npcName;

    [Header("Intro")]
    // ✅ Agora a Intro usa DialogueLine (aceita Sprites e Nomes)
    public DialogueLine[] introDialogue;

    [Header("Victory")]
    // ✅ Agora a Vitória usa DialogueLine
    public DialogueLine[] victoryDialogue;

    [Header("Defeat")]
    // ✅ Agora a Derrota usa DialogueLine
    public DialogueLine[] defeatDialogue;

    public QuestionRPG[] questions;

    public int maxLives = 3;
    public int minCorrect = 2;

    public string objectToActivate;
    public string objectToDeactivate;
}