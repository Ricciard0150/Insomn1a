using System;

[Serializable]
public class QuestData
{
    public string questId;
    public string questName;
    public string npcName;

    public string[] introLines;
    public string[] victoryLines;
    public string[] defeatLines;

    public QuestionRPG[] questions;

    public int maxLives = 3;
    public int minCorrect = 2;

    public string objectToActivate;
    public string objectToDeactivate;
}