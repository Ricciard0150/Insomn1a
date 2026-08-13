using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartCutescene : MonoBehaviour
{
    public Image blackScreen; // imagem preta na UI
    public float fadeDuration = 3f;

    public TopDownMovement playerMovement;

    void Start()
    {
        playerMovement.canMove = false;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0f;
        Color color = blackScreen.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = 1 - (time / fadeDuration);
            blackScreen.color = color;
            yield return null;
        }

        color.a = 0;
        blackScreen.color = color;

        // libera movimento
        playerMovement.canMove = true;
    }
}
