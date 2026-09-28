using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionBed : MonoBehaviour
{
    [Header("Config")]
    public string sceneName;
    public KeyCode interactKey = KeyCode.E;
    public GameObject pressE;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 2f;

    private Vector3 playerPositionOnExit;
    private string lastSceneName;
    private bool playerNear = false;
    private bool transitionActive = false;

    void Update()
    {
        if (playerNear && Input.GetKeyDown(interactKey) && !transitionActive)
        {
            StartCoroutine(Transition());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerNear = true;
            if (pressE != null) 
                pressE.SetActive(true);

            playerPositionOnExit = collision.transform.position;
            lastSceneName = SceneManager.GetActiveScene().name;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IStatusPlayer status))
        {
            playerNear = false;
            if (pressE != null) 
                pressE.SetActive(false);
        }
    }

    IEnumerator Transition()
    {
        transitionActive = true;

        
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = time / fadeDuration;
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 1f);
        SceneManager.LoadScene(sceneName);
    }
}