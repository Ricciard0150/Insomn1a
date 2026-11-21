using System;
using UnityEngine;

[Serializable]
public class QuestData
{
    public string questId;
    public string questName;
    public string npcName;

    [Header("Intro")]
    public string[] introLines;
    // ✅ NOVO: Nomes para cada linha de introdução (opcional)
    public string[] introNames;

    [Header("Victory")]
    public string[] victoryLines;
    // ✅ NOVO: Nomes para cada linha de vitória (opcional)
    public string[] victoryNames;

    [Header("Defeat")]
    public string[] defeatLines;
    // ✅ NOVO: Nomes para cada linha de derrota (opcional)
    public string[] defeatNames;

    public QuestionRPG[] questions;

    public int maxLives = 3;
    public int minCorrect = 2;

    public string objectToActivate;
    public string objectToDeactivate;
}