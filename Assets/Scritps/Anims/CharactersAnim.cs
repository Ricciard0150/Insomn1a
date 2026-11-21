using UnityEngine;
using System.Collections;

public class CharactersAnim : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesCharacter;

    void Start()
    {
        // Se a lista estiver vazia ou nula, nem começa a animação
        if (spritesCharacter == null || spritesCharacter.Length == 0)
        {
            Debug.LogWarning($"[CharactersAnim] Nenhum sprite configurado no objeto {gameObject.name}. A animação foi desativada.");
            return;
        }

        StartCoroutine(AnimateCharacter());
    }

    private IEnumerator AnimateCharacter()
    {
        int index = 0;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        // Se o objeto não tiver SpriteRenderer, evita erro
        if (spriteRenderer == null)
        {
            Debug.LogError($"[CharactersAnim] O objeto {gameObject.name} não tem um SpriteRenderer!");
            yield break;
        }

        while (true)
        {
            spriteRenderer.sprite = spritesCharacter[index];
            index = (index + 1) % spritesCharacter.Length;
            yield return new WaitForSeconds(0.5f);
        }
    }
}