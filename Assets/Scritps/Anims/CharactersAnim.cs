using UnityEngine;
using System.Collections;

public class CharactersAnim : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesCharacter;
    void Start()
    {
        StartCoroutine(AnimateCharacter());
    }

    private IEnumerator AnimateCharacter()
    {
        int index = 0;
        while (true)
        {
            GetComponent<SpriteRenderer>().sprite = spritesCharacter[index];
            index = (index + 1) % spritesCharacter.Length;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
