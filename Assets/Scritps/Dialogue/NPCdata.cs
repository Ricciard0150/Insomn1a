using UnityEngine;

public class NPCData : MonoBehaviour
{
    public string npcName;

    [Header("Falas Originais (Usadas pelo ThoughtsD)")]
    [TextArea(3, 5)] public string[] falas;
    public Sprite[] sprites;

    [Header("Falas ANTES da Nightmare (Usadas pelo InteractionDetector)")]
    [TextArea(3, 5)] public string[] falasBeforeNightmare;
    public Sprite[] spritesBeforeNightmare;

    [Header("Falas DEPOIS da Nightmare (Usadas pelo InteractionDetector)")]
    [TextArea(3, 5)] public string[] falasAfterNightmare;
    public Sprite[] spritesAfterNightmare;

    // ✅ NOVO: Nomes correspondentes a cada fala
    [Header("Nomes por Fala (Opcional)")]
    [Tooltip("Preencha com o nome de quem fala em cada linha. Se ficar vazio, usa o 'npcName'.")]
    public string[] nomesBeforeNightmare;
    public string[] nomesAfterNightmare;
}