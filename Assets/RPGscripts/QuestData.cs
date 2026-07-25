using System;

[Serializable]
public class QuestData
{
    public string questId;
    public string questName;
    public string npcName;

    // Diálogos
    public string[] introLines;
    public string[] victoryLines;
    public string[] defeatLines;

    // Perguntas
    public QuestionRPG[] questions;

    // Configurações
    public int maxLives = 3;
    public int minCorrect = 2;

    // Recompensas (opcional)
    public string objectToActivate;
    public string objectToDeactivate;
}