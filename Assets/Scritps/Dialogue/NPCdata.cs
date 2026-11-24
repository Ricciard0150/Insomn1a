using UnityEngine;

public class NPCData : MonoBehaviour
{
    public string npcName;

    [Header("Falas Originais")]
    [TextArea(3, 5)] public string[] falas;
    public Sprite[] sprites;

    [Header("Falas ANTES da Nightmare")]
    [TextArea(3, 5)] public string[] falasBeforeNightmare;
    public Sprite[] spritesBeforeNightmare;
    public bool[] mostrarSpriteBefore; // ✅ NOVO

    [Header("Falas DEPOIS da Nightmare")]
    [TextArea(3, 5)] public string[] falasAfterNightmare;
    public Sprite[] spritesAfterNightmare;
    public bool[] mostrarSpriteAfter; // ✅ NOVO

    [Header("Nomes por Fala")]
    public string[] nomesBeforeNightmare;
    public string[] nomesAfterNightmare;
}