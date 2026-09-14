using UnityEngine;
using System.Collections;

public class AnimTest : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesCharacter;
    void Start()
    {
        StartCoroutine(AnimateCharacter());
    }

    // Update is called once per frame
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
